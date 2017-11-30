using System;
using UnityEngine;
using System.IO;

public class Data : ISerializationCallbackReceiver
{
	IntegerData data = new IntegerData ();
	string json;

	public string MakeWithDefaultData (int x, int y)
	{
		data.coordinates = new int[x][];
		for (int i = 0; i < x; i++) {
			data.coordinates [i] = new int[y];
			for (int j = 0; j < y; j++) {
				data.coordinates [i] [j] = 1;
			}
		}
		json = "{ \"coordinates\": [ ";
		for (int k = 0; k < data.coordinates.Length; k++) {
			json += JsonHelper.arrayToJson (data.coordinates [k]);
			json += k != data.coordinates.Length - 1 ? "," : "]}";
		}
		SaveData ();
		json = LoadData ();
		return JsonUtility.FromJson<string> (json);
	}

	public void SaveData ()
	{
		var writer = new StreamWriter ("Assets/Scripts/Data/cube.json");
		writer.Write (json);
		writer.Close ();
	}

	public string LoadData ()
	{
		var reader = new StreamReader ("Assets/Scripts/Data/cube.json");
		string mystring;

		string results = JsonHelper.getJsonArray<string[]> (reader.ReadToEnd ());
		reader.Close ();
		return mystring;
	}

	public void OnBeforeSerialize ()
	{
		Debug.Log (this);
	}

	public void OnAfterDeserialize ()
	{
		Debug.Log (this);
	}
}

[Serializable]
public class IntegerData
{
	public int[][] coordinates;
}


public static class JsonHelper
{
	//Usage:
	//YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
	public static T[] getJsonArray<T> (string json)
	{
		string newJson = "{ \"array\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
		return wrapper.array;
	}

	//Usage:
	//string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
	public static string arrayToJson<T> (T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T> { array = array };
		string piece = JsonUtility.ToJson (wrapper);
		var pos = piece.IndexOf (":");
		piece = piece.Substring (pos + 1); // cut away "{ \"array\":"
		pos = piece.LastIndexOf ('}');
		piece = piece.Substring (0, pos); // cut away "}" at the end
		return piece;
	}

	[Serializable]
	private class Wrapper<T>
	{
		public T[] array;
	}
}