using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CSharp;

public class TheCube : MonoBehaviour {
    public GameObject cubelete;
    public GameObject surface;
    public GameObject fakeSurface;
    public GameObject fakeVolume;
    public float spawnMinDist;
    public float yMax;
    public float xMax;
    public float zMax;

    private List<List<CubeleteObject>> surfaceCubeletes = new List<List<CubeleteObject>>();

    Vector3 cubeleteBasePosition;
    Vector3 cubeTrackPosition;

    private bool faded = false;
    //private bool tweaked = false;

    private float xTracker;
    private float yTracker;
    private float xCubes;
    private float yCubes;

    private int Left;
    private int Right;
    private int Top;
    private int Bottom;

    //private Data data = new Data ();
    //private string cubedata;

    void Start () {
        cubeleteBasePosition = cubelete.transform.localPosition;
        cubeTrackPosition = transform.localPosition;
        MakeInteractibleSurface();
    }

    void Update () {
        Vector3 currPos = transform.localPosition;

        if (currPos.z > 0.51 && faded == false) {
            FadeCube(true);
        }
        else if (currPos.z <= 0.51 && faded == true) {
            FadeCube(false);
        }

        xTracker += (currPos.x - cubeTrackPosition.x);
        yTracker += (currPos.y - cubeTrackPosition.y);
        cubeTrackPosition = currPos;

        xCubes = xTracker / 0.02f;
        yCubes = yTracker / 0.02f;

        if (Mathf.Abs(xCubes) > 1 || Mathf.Abs(yCubes) > 1) {
            var xTakeaway = Mathf.Round(xCubes);
            var yTakeaway = Mathf.Round(yCubes);
            xTracker -= xTakeaway * 0.02f;
            yTracker -= yTakeaway * 0.02f;
            UpdateInteractibleSurface(xTakeaway, yTakeaway);
        }

        //	 else if (currPos.z < 0.3 && tweaked == false) {
        //			TweakSurface (true);
        //		} else if (currPos.z >= 0.3 && tweaked == true) {
        //			TweakSurface (false);
        //		}
    }

    void FadeCube (bool fade) {
        faded = fade;
        fakeSurface.SetActive(fade);
        surface.SetActive(!fade);
    }

    void TweakSurface (bool tweak) {
        // tweaking surface has an inherent problem - moving position of cubeletes changes where they are positioned >< 
        // maybe what i need to do is change spawnMinDist, or better yet, scale down the cubelete so it looks like there is space added betwixt
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

    public void MakeInteractibleSurface () {

        // every 0.02 of parent position is 1 cubelete width/height
        // so i have to create all cubeletes based on current parent position

        var xStart = -xMax / 2;
        var yStart = -yMax / 2;

        Top = (int)-yStart;
        Bottom = (int)yStart;
        Left = (int)xStart;
        Right = (int)-xStart;

        for (int i = 0; i < xMax; i++) {
            surfaceCubeletes.Add(new List<CubeleteObject>());
            for (int j = 0; j < yMax; j++) {
                for (int k = 0; k < zMax; k++) {
                    CubeleteObject newCubelete = new CubeleteObject();
                    newCubelete.gameObject = Instantiate(cubelete, surface.transform);
                    newCubelete.gameObject.transform.localPosition = new Vector3(
                        cubeleteBasePosition.x + (spawnMinDist * xStart),
                        cubeleteBasePosition.y + (spawnMinDist * yStart) - 0.38f,
                        cubeleteBasePosition.z + (spawnMinDist * k)
                    );
                    newCubelete.cubeX = (int)xStart;
                    newCubelete.cubeY = (int)yStart;
                    surfaceCubeletes[i].Add(newCubelete);
                }
                yStart++;
            }
            xStart++;
            yStart = -yMax / 2;
        }
    }

    public void UpdateInteractibleSurface (float xCubes, float yCubes) {

        Debug.Log(string.Format("X: {0}, Y: {1}", xCubes, yCubes));
        var yCubesAbs = (int)Mathf.Abs(yCubes);
        var xCubesAbs = (int)Mathf.Abs(xCubes);

        Right = xCubesAbs > 0 ? (int)(Right - xCubes) : Right;
        Left = xCubesAbs > 0 ? (int)(Left - xCubes) : Left;
        Top = yCubesAbs > 0 ? (int)(Top - yCubes) : Top;
        Bottom = yCubesAbs > 0 ? (int)(Bottom - yCubes) : Bottom;

        bool direction = true;
        var xDeleteOffset = 0;
        var yAddOffset = yCubesAbs;
        if (yCubes > 0 || xCubes > 0) {
            direction = false;
            yAddOffset = 0;
            xDeleteOffset = surfaceCubeletes.Count - 1;
        }

        var yDeleteStart = yCubesAbs - 1;
        var xDeleteEnd = (int)xMax - 1;
        var xAddStart = Left;
        var xAddEnd = Right;
        var yAddStart = 0;
        var yAddEnd = yCubesAbs;

        float addY = (yCubes > 0 ? (yCubes > 1 ? Bottom + (yCubes - 1) : Bottom) : (yCubes < -1 ? Top - (yCubes + 1) : Top)) - yAddOffset;
        float addX = -1;

        if (xCubesAbs > 0) {
            yDeleteStart = (int)yMax - 1;
            xDeleteEnd = xCubesAbs - 1;
            xAddStart = 0;
            xAddEnd = xCubesAbs;
            yAddStart = Bottom;
            yAddEnd = Top;

            addY = -1;
            addX = (xCubes > 0 ? (xCubes > 1 ? Left + (xCubes - 1) : Left) : (xCubes < -1 ? Right - (xCubes + 1) : Right));
            //direction = !direction;
        }

        //delete
        for (int x = 0; x <= xDeleteEnd; x++) {
            List<CubeleteObject> layer = surfaceCubeletes[x];
            int insertAt = xDeleteOffset - x;
            for (int y = yDeleteStart; y >= 0; y--) {
                if (direction) {
                    Destroy(layer[y].gameObject);
                    layer.RemoveAt(y);
                }
                else {
                    insertAt = x;
                    layer = surfaceCubeletes[xDeleteOffset - x];
                    Destroy(layer[layer.Count - 1].gameObject);
                    layer.RemoveAt(layer.Count - 1);
                }
            }
            if (layer.Count == 0) {
                surfaceCubeletes.Remove(layer);
                surfaceCubeletes.Insert(insertAt, new List<CubeleteObject>());
            }
        }

        //add
        var xAddEndMinus = 0;
        var insertAt = 0;
        if (xCubesAbs > 0) {
            direction = !direction;
            xAddEndMinus = 1;
        }
        for (int xP = xAddStart; xP < xAddEnd; xP++) {
            for (int yP = yAddStart; yP < yAddEnd; yP++) {
                CubeleteObject newCubelete = new CubeleteObject();
                newCubelete.gameObject = Instantiate(cubelete, surface.transform);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    cubeleteBasePosition.x + (spawnMinDist * (addX - xP)),
                    cubeleteBasePosition.y + (spawnMinDist * (addY - yP)) - 0.38f,
                    cubeleteBasePosition.z
                );
                // newCubelete.cubeX 
                // newCubelete.cubeY

                if (!direction) {
                    surfaceCubeletes[xP - xAddStart].Insert(yAddStart, newCubelete);
                }
                else {
                    surfaceCubeletes[xAddEnd - xAddEndMinus - xP].Add(newCubelete);
                }
            }
        }
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


    static float Operator (float x, float multiplier, bool op) {
        float results = 0;
        if (op == true) {
            results = x + multiplier;
        }
        else if (op == false) {
            results = x - multiplier;
        }
        return results;
    }
}


public class CubeleteObject {
    public GameObject gameObject;
    public int cubeX;
    public int cubeY;
}