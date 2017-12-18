using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CSharp;

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

    void Start()
    {
        cubeleteBasePosition = cubelete.transform.localPosition;
        cubeTrackPosition = transform.localPosition;
    }

    void Update()
    {
        Vector3 currPos = transform.localPosition;

        if (currPos.z > 0.51 && faded == false)
        {
            FadeCube(true);
        }
        else if (currPos.z <= 0.51 && faded == true)
        {
            FadeCube(false);
        }

        xTracker += (currPos.x - cubeTrackPosition.x);
        yTracker += (currPos.y - cubeTrackPosition.y);
        cubeTrackPosition = currPos;

        xCubes = xTracker / 0.02f;
        yCubes = yTracker / 0.02f;

        if (Mathf.Abs(xCubes) > 1 || Mathf.Abs(yCubes) > 1)
        {
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

    void FadeCube(bool fade)
    {
        faded = fade;
        fakeSurface.SetActive(fade);
        surface.SetActive(!fade);
    }

    void TweakSurface(bool tweak)
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

    public void MakeInteractibleSurface()
    {

        // every 0.02 of parent position is 1 cubelete width/height
        // so i have to create all cubeletes based on current parent position

        var xStart = -xMax / 2;
        var yStart = -yMax / 2;

        Top = (int)-yStart;
        Bottom = (int)yStart;
        Left = (int)xStart;
        Right = (int)-xStart;

        for (int i = 0; i < xMax; i++)
        {
            surfaceCubeletes.Add(new List<CubeleteObject>());
            for (int j = 0; j < yMax; j++)
            {
                for (int k = 0; k < zMax; k++)
                {
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

    public void UpdateInteractibleSurface(float xCubes, float yCubes)
    {
        var yCubesAbs = (int)Mathf.Abs(yCubes);
        var xCubesAbs = (int)Mathf.Abs(xCubes);

        Right = xCubesAbs > 0 ? (int)(Right - xCubes) : Right;
        Left = xCubesAbs > 0 ? (int)(Left - xCubes) : Left;

        Top = yCubesAbs > 0 ? (int)(Top - yCubes) : Top;
        Bottom = yCubesAbs > 0 ? (int)(Bottom - yCubes) : Bottom;

        //delete Y
        for (int x = 0; x < (int)xMax; x++)
        {
            for (int y = yCubesAbs - 1; y >= 0; y--)
            {
                if (yCubes < 0)
                {
                    Destroy(surfaceCubeletes[x][y].gameObject);
                    surfaceCubeletes[x].RemoveAt(y);
                }
                else
                {
                    Destroy(surfaceCubeletes[x][surfaceCubeletes[x].Count - 1].gameObject);
                    surfaceCubeletes[x].RemoveAt(surfaceCubeletes[x].Count - 1);
                }
            }
        }

        //add Y
        var yOffset = 0;
        if (yCubes > 0)
        {
            yOffset = 0;
        }
        else
        {
            yOffset = yCubesAbs;
        }
        for (int xP = Left; xP < Right; xP++)
        {
            for (int yP = 0; yP < yCubesAbs; yP++)
            {
                CubeleteObject newCubelete = new CubeleteObject();
                newCubelete.gameObject = Instantiate(cubelete, surface.transform);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    cubeleteBasePosition.x + (spawnMinDist * xP),
                    cubeleteBasePosition.y + (spawnMinDist * ((yCubes > 0 ? Bottom : Top) - (yOffset - yP))) - 0.38f,
                    cubeleteBasePosition.z
                );

                // newCubelete.cubeX 
                // newCubelete.cubeY
                if (yCubes > 0)
                {
                    surfaceCubeletes[xP - Left].Insert(yP, newCubelete);
                }
                else
                {
                    surfaceCubeletes[xP - Left].Add(newCubelete);
                }
            }
        }

        //delete X
        for (int x = 0; x < xCubesAbs; x++)
        {
            for (int y = (int)yMax - 1; y >= 0; y--)
            {
                if (xCubes < 0)
                {
                    Destroy(surfaceCubeletes[x][y].gameObject);
                    surfaceCubeletes[x].RemoveAt(y);
                }
                else
                {
                    Destroy(surfaceCubeletes[x][surfaceCubeletes[x].Count - 1].gameObject);
                    surfaceCubeletes[x].RemoveAt(surfaceCubeletes[x].Count - 1);
                }
            }
        }

        //add X & delete previous Y? 
        var xOffset = 0;
        if (xCubes > 0)
        {
            xOffset = 0;
        }
        else
        {
            xOffset = xCubesAbs;
        }
        for (int xP = 0; xP < xCubesAbs; xP++)
        {
            for (int yP = Bottom; yP < Top; yP++)
            {
                CubeleteObject newCubelete = new CubeleteObject();
                newCubelete.gameObject = Instantiate(cubelete, surface.transform);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    cubeleteBasePosition.x + (spawnMinDist * ((xCubes > 0 ? Left : Right) - (xOffset - xP))),
                    cubeleteBasePosition.y + (spawnMinDist * yP) - 0.38f,
                    cubeleteBasePosition.z
                );

                // newCubelete.cubeX 
                // newCubelete.cubeY
                if (yCubes > 0)
                {
                    surfaceCubeletes[xP - Left].Insert(yP, newCubelete);
                }
                else
                {
                    surfaceCubeletes[xP - Left].Add(newCubelete);
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


    static float Operator(float x, float multiplier, bool op)
    {
        float results = 0;
        if (op == true)
        {
            results = x + multiplier;
        }
        else if (op == false)
        {
            results = x - multiplier;
        }
        return results;
    }
}


public class CubeleteObject
{
    public GameObject gameObject;
    public int cubeX;
    public int cubeY;
}