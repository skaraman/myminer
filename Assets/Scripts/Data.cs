using System;
using UnityEngine;
using System.IO;
//using System.Web.Script.Serialization;

public class Data : ISerializationCallbackReceiver
{
	DoubleData data = new DoubleData ();
	//JavaScriptSerializer jss = new JavaScriptSerializer ();
	public string MakeFromDefaultData (int x, int y)
	{
		
		data.coordinates = new double[x][];
		for (int i = 0; i < x; i++) {
			data.coordinates [i] = new double[y];
			for (int j = 0; j < y; j++) {
				data.coordinates [i] [j] = 1;
			}
		}		
		//string js = jss.Serialize (data);
		string json = "";
		for (int k = 0; k < data.coordinates.Length; k++) {
			json += JsonHelper.arrayToJson (data.coordinates[k]);
		}
		//string json = JsonUtility.ToJson (data); // string json = JsonUtility.ToJson (js);
		StreamWriter writer = new StreamWriter ("Assets/Scripts/Data/cube.json");
		writer.Write (json);
		writer.Close ();
		StreamReader reader = new StreamReader ("Assets/Scripts/Data/cube.json");
		string jsonR = reader.ReadToEnd ();
		reader.Close ();
		return JsonUtility.FromJson<string> (jsonR);
	}

	public void OnBeforeSerialize()
	{
		Debug.Log (this);
	}

	public void OnAfterDeserialize()
	{
		Debug.Log (this);
	}
}

[Serializable]
public class DoubleData
{
	public double[][] coordinates;
}


public class JsonHelper
{
	//Usage:
	//YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
	public static T[] getJsonArray<T>(string json)
	{
		string newJson = "{ \"array\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
		return wrapper.array;
	}

	//Usage:
	//string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
	public static string arrayToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T> { array = array };
		string json = JsonUtility.ToJson(wrapper);
		var pos = json.IndexOf(":");
		json = json.Substring(pos+1); // cut away "{ \"array\":"
		pos = json.LastIndexOf('}');
		json = json.Substring(0, pos-1); // cut away "}" at the end
		return json;
	}
		
	[Serializable]
	private class Wrapper<T>
	{
		public T[] array;
	}

}