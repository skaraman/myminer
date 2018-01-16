using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TheCube : MonoBehaviour {
    public GameObject cubelete;
    public GameObject surface;
    public GameObject fakeSurface;
    public GameObject fakeVolume;
    public float spawnMinDist;
    public float GlobalYOffset;
    public float yMax;
    public float xMax;
    public float zMax;
    public float zZoomLimit;
    public GameObject zoomPlus;
    public GameObject zoomMinus;
    Vector3 cubeleteBasePosition;
    Vector3 cubeTrackPosition;
    public bool zoomIn = true;
    public bool zoomOut = true;
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
    private float minCubeY = -9.93f;
    private float maxCubeY = 9.225f;
    private float minCubeX = -9.6f;
    private float maxCubeX = 9.6f;
    //private Data data = new Data ();
    //private string cubedata;
    public Dictionary<string, bool> deletedCubeletes = new Dictionary<string, bool>();
    private List<List<CubeleteObject>> surfaceCubeletes = new List<List<CubeleteObject>>();

    void Start () {
        cubeleteBasePosition = cubelete.transform.localPosition;
        cubeTrackPosition = transform.localPosition;
        if (File.Exists((Application.persistentDataPath + "/clobber.sem"))) {
            LoadData();
        }
        MakeInteractibleSurface();

    }

    void Awake () {
        //iOS serializer fix
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
    }

    void Update () {
        Vector3 currPos = transform.localPosition;
        if (currPos.z > zZoomLimit && faded == false) {
            FadeCube(true);
        }
        else if (currPos.z <= zZoomLimit && faded == true) {
            FadeCube(false);
        }
        if (currPos.y < minCubeY) {
            currPos.y = minCubeY;
        }
        else if (currPos.y > maxCubeY) {
            currPos.y = maxCubeY;
        }
        if (currPos.x < minCubeX) {
            currPos.x = minCubeX;
        }
        else if (currPos.x > maxCubeX) {
            currPos.x = maxCubeX;
        }
        transform.localPosition = new Vector3(currPos.x, currPos.y, currPos.z);
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
        //			float newY = -GlobalYOffset;
        //			float spacer;
        //			if (tweak) {
        //				spacer = 0.02f;
        //			} else {
        //				spacer = 0.04f;
        //			}
        //			int xit = Mathf.RoundToInt (Mathf.Abs (child.localPosition.x / spacer));
        //			int yit = Mathf.RoundToInt (Mathf.Abs ((child.localPosition.y + GlobalYOffset) / spacer));
        //
        //			if (child.localPosition.x > 0) {
        //				newX = Operator (child.localPosition.x, 0.02f * xit, tweak);
        //			} else if (child.localPosition.x < 0) {
        //				newX = Operator (child.localPosition.x, 0.02f * xit, !tweak);
        //			}
        //			if (child.localPosition.y + GlobalYOffset > 0) {
        //				newY = Operator (child.localPosition.y, 0.02f * yit, tweak);
        //			} else if (child.localPosition.y + GlobalYOffset < 0) {
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
                        cubeleteBasePosition.y + (spawnMinDist * yStart) - GlobalYOffset,
                        cubeleteBasePosition.z + (spawnMinDist * k)
                    );
                    var cubeComp = newCubelete.gameObject.GetComponent<Cubelete>();
                    var version = cubeComp.GetVersion();
                    cubeComp.animatingDestructible = Instantiate(version, surface.transform);
                    cubeComp.animatingDestructible.SetActive(false);
                    cubeComp.animatingDestructible.transform.localPosition = newCubelete.gameObject.transform.localPosition;
                    cubeComp.x = (int)xStart;
                    cubeComp.y = (int)yStart;
                    newCubelete.x = cubeComp.x;
                    newCubelete.y = cubeComp.y;
                    cubeComp.cube = this;
                    surfaceCubeletes[i].Add(newCubelete);
                    try {
                        if (deletedCubeletes[string.Format("{0},{1}", cubeComp.x, cubeComp.y)]) {
                            newCubelete.gameObject.SetActive(false);
                        }
                    }
                    catch { }
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
        var postYCubes = yCubes;
        bool post = false;
        if (xCubesAbs > 0 && yCubesAbs > 0) {
            post = true;
            yCubes = 0;
            yCubesAbs = 0;
        }
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
        float addX = 0;
        if (xCubesAbs > 0) {
            yDeleteStart = (int)yMax - 1;
            xDeleteEnd = xCubesAbs - 1;
            xAddStart = 0;
            xAddEnd = xCubesAbs;
            yAddStart = Bottom;
            yAddEnd = Top;
            addY = 0;
            addX = (xCubes > 0 ? (xCubes > 1 ? Left + (xCubes - 1) : Left) : Right - 1);
        }
        //delete
        for (int x = 0; x <= xDeleteEnd; x++) {
            List<CubeleteObject> layer = surfaceCubeletes[yCubesAbs > 0 ? x : 0];
            int insertAtDel = xDeleteOffset - x;
            for (int y = yDeleteStart; y >= 0; y--) {
                if (direction) {
                    Destroy(layer[y].gameObject);
                    layer.RemoveAt(y);
                }
                else {
                    insertAtDel = x;
                    layer = surfaceCubeletes[xCubesAbs > 0 ? xDeleteOffset : xDeleteOffset - x];
                    Destroy(layer[layer.Count - 1].gameObject);
                    layer.RemoveAt(layer.Count - 1);
                }
            }
            if (layer.Count == 0 && xCubesAbs > 0) {
                surfaceCubeletes.Remove(layer);
                if (direction) {
                    surfaceCubeletes.Add(new List<CubeleteObject>());
                }
                else {
                    surfaceCubeletes.Insert(insertAtDel, new List<CubeleteObject>());
                }
            }
        }
        //add
        if (xCubesAbs > 0) {
            direction = !direction;
        }
        for (int xP = xAddStart; xP < xAddEnd; xP++) {
            int insertAtAdd = xP - xAddStart;
            if (xCubesAbs > 0) {
                insertAtAdd = xP;
            }
            var layer = surfaceCubeletes[insertAtAdd];
            var currentCount = layer.Count;
            for (int yP = yAddStart; yP < yAddEnd; yP++) {
                CubeleteObject newCubelete = new CubeleteObject();
                newCubelete.gameObject = Instantiate(cubelete, surface.transform);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    cubeleteBasePosition.x + (spawnMinDist * (yCubesAbs > 0 ? addX + xP : addX - xP)),
                    cubeleteBasePosition.y + (spawnMinDist * (xCubesAbs > 0 ? addY + yP : addY - yP)) - GlobalYOffset,
                    cubeleteBasePosition.z
                );
                var cubeComp = newCubelete.gameObject.GetComponent<Cubelete>();
                cubeComp.cube = this;
                var version = cubeComp.GetVersion();
                cubeComp.animatingDestructible = Instantiate(version, surface.transform);
                cubeComp.animatingDestructible.SetActive(false);
                cubeComp.animatingDestructible.transform.localPosition = newCubelete.gameObject.transform.localPosition;
                if (!direction) {
                    if (xCubesAbs > 0) {
                        cubeComp.x = Right - 1 - xP;
                        cubeComp.y = yP;
                        surfaceCubeletes[surfaceCubeletes.Count - 1 - xP].Add(newCubelete);
                    }
                    if (yCubesAbs > 0) {
                        cubeComp.x = xP;
                        cubeComp.y = (Bottom + (yCubesAbs - 1)) - yP;
                        surfaceCubeletes[xP - xAddStart].Insert(/*xCubesAbs > 0 ? xP :*/ yAddStart, newCubelete);
                    }
                }
                else {
                    if (xCubesAbs > 0) {
                        cubeComp.x = (Left + (xCubesAbs - 1)) - xP;
                        cubeComp.y = yP;
                        surfaceCubeletes[xAddEnd - 1 - xP].Add(newCubelete);
                    }
                    if (yCubesAbs > 0) {
                        cubeComp.x = xP;
                        cubeComp.y = Top - 1 - yP;
                        layer.Insert(/*xCubesAbs > 0 ? 0 :*/ currentCount, newCubelete);
                    }
                }
                newCubelete.x = cubeComp.x;
                newCubelete.y = cubeComp.y;
                try {
                    if (deletedCubeletes[string.Format("{0},{1}", cubeComp.x, cubeComp.y)]) {
                        newCubelete.gameObject.SetActive(false);
                    }
                }
                catch { }
            }
        }
        if (post) {
            UpdateInteractibleSurface(0, postYCubes);
        }
    }

    public void addDeletedCubelete (int x, int y) {
        deletedCubeletes.Add(string.Format("{0},{1}", x, y), true);
        Debug.Log(string.Format("{0},{1}", x, y));
    }

    public void SaveData () {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/clobber.sem");
        CubeData data = new CubeData();
        data.deletedCubeletesData = deletedCubeletes;
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadData () {
        if (File.Exists((Application.persistentDataPath + "/clobber.sem"))) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/clobber.sem", FileMode.Open);
            CubeData data = (CubeData)bf.Deserialize(file);
            file.Close();
            deletedCubeletes = data.deletedCubeletesData;
        }
    }

    public void DeleteSave () {
        if (File.Exists(Application.persistentDataPath + "/clobber.sem")) {
            File.Delete(Application.persistentDataPath + "/clobber.sem");
        }
    }

    public void DeleteDeletedCubeletes () {
        deletedCubeletes = new Dictionary<string, bool>();
    }

    public void DeleteInteractibleSurface () {
        var aMax = surfaceCubeletes.Count;
        for (var a = 0; a < aMax; a++) {
            var bMax = surfaceCubeletes[0].Count;
            for (var b = 0; b < bMax; b++) {
                Destroy(surfaceCubeletes[0][0].gameObject);
                surfaceCubeletes[0].RemoveAt(0);
            }
            surfaceCubeletes.RemoveAt(0);
        }
    }

    public void SetZoomIn (bool value) {
        zoomIn = value;
    }

    public void SetZoomOut (bool value) {
        zoomOut = value;
    }

    static float Operator (float basse, float adjustment, bool op) {
        float results = 0;
        if (op == true) {
            results = basse + adjustment;
        }
        else if (op == false) {
            results = basse - adjustment;
        }
        return results;
    }
}


public class CubeleteObject {
    public GameObject gameObject;
    public int x;
    public int y;
}

[System.Serializable]
public class CubeData {
    public Dictionary<string, bool> deletedCubeletesData;
    public Dictionary<string, float> mainCubeLocation;
}