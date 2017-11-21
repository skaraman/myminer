using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]

public class Data
{
	public double[][] coordinates;
	//JavaScriptSerializer js = new JavaScriptSerializer ();
	BinaryFormatter bf = new BinaryFormatter ();

	public double[][] MakeFromDefaultData (int x, int y)
	{
		coordinates = new double[x][];
		for (int i = 0; i < x; i++) {
			coordinates [i] = new double[y];
			for (int j = 0; j < y; j++) {
				coordinates [i] [j] = 1;
			}
		}
		
		//string jss = js.Serialize (coordinates);
		//string json = JsonUtility.ToJson (jss);
		StreamWriter writer = new StreamWriter ("Assets/Scripts/Data/cube.json");
		//writer.Write (json);
		writer.Close ();
		StreamReader reader = new StreamReader ("Assets/Scripts/Data/cube.json");
		string jsonR = reader.ReadToEnd ();
		reader.Close ();
		return JsonUtility.FromJson<double[][]> (jsonR);
	}

}
