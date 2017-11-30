using System.Collections;
using System.Collections.Generic;
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

	Vector3 startPOS;

	private bool faded = false;
	private bool tweaked = false;

	private Data data = new Data ();
	private string cubedata;
	// Use this for initialization
	void Start ()
	{
		startPOS = cubelete.transform.localPosition;

		LoadData ();

		cubedata = data.MakeWithDefaultData (31, 41);
		Debug.Log (cubedata);
		// generate the touchable surface
		for (int i = 0; i < xMax; i++) {
			for (int j = 0; j < yMax; j++) {
				GameObject newCubelete = Instantiate (cubelete, surface.transform);
				newCubelete.transform.localPosition = new Vector3 (
					startPOS.x + (spawnMinDist * i),
					startPOS.y + (spawnMinDist * j),
					0
				);
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Vector3 currPos = transform.localPosition;
		if (currPos.z > 8 && faded == false) {
			FadeCube (true);
		} else if (currPos.z <= 8 && faded == true) {
			FadeCube (false);
		} else if (currPos.z < 3 && tweaked == false) {
			TweakSurface (true);
		} else if (currPos.z >= 3 && tweaked == true) {
			TweakSurface (false);
		}
	}

	void FadeCube (bool fade)
	{
		faded = fade;
		fakeSurface.SetActive (fade);
		surface.SetActive (!fade);
	}

	void TweakSurface (bool tweak)
	{
		tweaked = tweak;
		Component[] children = surface.GetComponentsInChildren<Transform> ();
		float it;
		foreach (Transform child in children) {
			float newX = 0.0f;
			float newY = 0.0f;

			if (child.localPosition.x > 0) {
				it = child.localPosition.x / 0.02f;
				newX = Operator (child.localPosition.x, (0.001f * it), tweak);
			}
			if (child.localPosition.y > 0) {
				it = child.localPosition.y / 0.02f;
				newY = Operator (child.localPosition.y, (0.001f * it), tweak);
			}
			//iTween.MoveBy(child.gameObject, iTween.Hash("x", newX, "y", newY, "easeType", "easeInOutExpo", "delay", .1));

			child.localPosition = new Vector3 (newX, newY, child.localPosition.z);
		}
	}

	public void SaveData ()
	{
		data.SaveData ();
	}

	public void LoadData ()
	{
		cubedata = data.LoadData ();
	}

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
