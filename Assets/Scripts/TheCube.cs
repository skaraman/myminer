using System.Collections;
using UnityEngine;

public class TheCube : MonoBehaviour
{
	public GameObject cubelete;
	public GameObject surface;
	public GameObject fakeSurface;
	public GameObject fakeVolume;
	public float spawnMinDist;
	public float yMax;
	public float xMax;
	public float zMax;

	Vector3 cubeleteBasePosition;
	Vector3 cubeTrackPosition;

	private bool faded = false;
	private bool tweaked = false;
	private float xTracker;
	private float yTracker;
	private float xCubes;
	private float yCubes;

	//private Data data = new Data ();
	//private string cubedata;

	void Start ()
	{
		cubeleteBasePosition = cubelete.transform.localPosition;
		cubeTrackPosition = transform.localPosition;
	}

	void Update ()
	{
		Vector3 currPos = transform.localPosition;

		if (currPos.z > 0.51 && faded == false) {
			FadeCube (true);
		} else if (currPos.z <= 0.51 && faded == true) {
			FadeCube (false);
		}

		xTracker += (currPos.x - cubeTrackPosition.x);
		yTracker += (currPos.y - cubeTrackPosition.y);
		cubeTrackPosition = currPos;

		xCubes = xTracker / 0.02f;
		yCubes = yTracker / 0.02f;

		if (Mathf.Abs (xCubes) > 1 || Mathf.Abs (yCubes) > 1) {
			var xTakeaway = Mathf.Round (xCubes);
			var yTakeaway = Mathf.Round (yCubes);
			xTracker -= xTakeaway * 0.02f;
			yTracker -= yTakeaway * 0.02f;
			UpdateInteractibleSurface (xTakeaway, yTakeaway);
		}

		//	 else if (currPos.z < 0.3 && tweaked == false) {
		//			TweakSurface (true);
		//		} else if (currPos.z >= 0.3 && tweaked == true) {
		//			TweakSurface (false);
		//		}
	}

	void FadeCube (bool fade)
	{
		faded = fade;
		fakeSurface.SetActive (fade);
		surface.SetActive (!fade);
	}

	void TweakSurface (bool tweak)
	{
		//		tweaked = tweak;
		//		var children = surface.GetComponentsInChildren<Transform> ();x
		//		for (var c = 1; c < children.Length; c++) {
		//			var child = children [c];
		//
		//			float newX = 0.0f;
		//			float newY = -0.38f;
		//			float spacer;
		//			if (tweak) {
		//				spacer = 0.02f;
		//			} else {
		//				spacer = 0.04f;
		//			}
		//			int xit = Mathf.RoundToInt (Mathf.Abs (child.localPosition.x / spacer));
		//			int yit = Mathf.RoundToInt (Mathf.Abs ((child.localPosition.y + 0.38f) / spacer));
		//
		//			if (child.localPosition.x > 0) {
		//				newX = Operator (child.localPosition.x, 0.02f * xit, tweak);
		//			} else if (child.localPosition.x < 0) {
		//				newX = Operator (child.localPosition.x, 0.02f * xit, !tweak);
		//			}
		//			if (child.localPosition.y + 0.38f > 0) {
		//				newY = Operator (child.localPosition.y, 0.02f * yit, tweak);
		//			} else if (child.localPosition.y + 0.38f < 0) {
		//				newY = Operator (child.localPosition.y, 0.02f * yit, !tweak);
		//			}
		//			child.localPosition = new Vector3 (newX, newY, child.localPosition.z);
		//		}

	}

	public void MakeInteractibleSurface ()
	{
		var parentX = transform.position.x;
		var parentY = transform.position.y;
		// every 0.02 of parent position is 1 cubelete width/height
		// so i have to create all cubeletes based on current parent position

		var xStart = -xMax / 2;
		var yStart = -yMax / 2;

		for (int i = 0; i < xMax; i++) {
			for (int j = 0; j < yMax; j++) {
				for (int k = 0; k < zMax; k++) {
					GameObject newCubelete = Instantiate (cubelete, surface.transform);
					newCubelete.transform.localPosition = new Vector3 (
						cubeleteBasePosition.x + (spawnMinDist * xStart),
						cubeleteBasePosition.y + (spawnMinDist * yStart) - 0.38f,
						cubeleteBasePosition.z + (spawnMinDist * k)
					);
				}
				yStart++;
			}
			xStart++;
			yStart = -yMax / 2;
		}
	}

	public void UpdateInteractibleSurface (float xCubes, float yCubes)
	{
		Debug.Log (string.Format ("xcubes: {0} ycubes: {1}", xCubes, yCubes));
		Debug.Log (string.Format ("current Cube: {0},{1}", transform.localPosition.x, transform.localPosition.y));

	}

	//public void SaveData ()
	//{
	//  string json = data.GetJsonFromCurrentCube (surface.GetComponentsInChildren<Transform> ());
	//  data.SaveData (json);
	//}

	//public void LoadData ()
	//{
	//  cubedata = data.LoadData ();
	//}


	static float Operator (float x, float multiplier, bool op)
	{
		float results = 0;
		if (op == true) {
			results = x + multiplier;
		} else if (op == false) {
			results = x - multiplier;
		}
		return results;
	}
}
