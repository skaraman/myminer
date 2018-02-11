﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class TheCube : MonoBehaviour {
    public GameObject cubelete;
    public GameObject surface;
    public GameObject fakeSurface;
    public GameObject fakeVolume;
    public float spawnMinDist;
    public float GlobalYOffset;
    public float GlobalXOffset;
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
    public GameObject piecesSurface;

    private bool faded = false;
    private bool remake = false;
    private bool loaded = false;

    private float xTracker;
    private float yTracker;
    private float xCubes;
    private float yCubes;

    private float xStart;
    private float yStart;

    private int Left;
    private int Right;
    private int Top;
    private int Bottom;

    private float minCubeY = -9.93f;
    private float maxCubeY = 9.225f;
    private float minCubeX = -9.6f;
    private float maxCubeX = 9.6f;

    public ScreenRecorder screenShotCamera;
    public List<GameObject> destros;
    public GameObject spriteHolder;
    public GameObject spriteGO;

    public Dictionary<string, bool> deletedCubeletes = new Dictionary<string, bool>();
    public Dictionary<string, string> screensList = new Dictionary<string, string>();

    private Color screenColor = new Color(1, .83f, .83f, 1);

    private List<List<CubeleteObject>> surfaceCubeletes = new List<List<CubeleteObject>>();
    private List<CubeleteObject> temporaryFreedCubeletes = new List<CubeleteObject>();
    private Dictionary<string, GameObject> spriteChildren = new Dictionary<string, GameObject>();

    private int cubeSaveLimit = 30;
    private int cubeSaveIterator = 0;
    private bool cubeSaveNeeded = false;

    private float mainXTracker;
    private float mainYTracker;

    CubeData data;
    void Start () {
        xStart = -xMax / 2;
        yStart = -yMax / 2;
        data = new CubeData();
        cubeleteBasePosition = cubelete.transform.localPosition;
        cubeTrackPosition = transform.localPosition;
        if (File.Exists((Application.persistentDataPath + "/bew.wyco"))) {
            LoadData();
        }
        MakeInteractibleSurface();
    }

    void Awake () {
        //iOS serializer fix
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
    }

    void Update () {
        if (cubeSaveIterator > cubeSaveLimit && cubeSaveNeeded == true) {
            cubeSaveIterator = 0;
            SaveData("cubeletes");
            cubeSaveNeeded = false;
        }
        else {
            cubeSaveIterator++;
        }
        if (destros.Count > 4) {
            var removeDestro = destros[0];
            destros.Remove(removeDestro);
            Destroy(removeDestro);
        }
        Vector3 currPos = transform.localPosition;
        if (currPos.z > zZoomLimit && faded == false) {
            FadeCube(true);
        }
        else if (currPos.z <= zZoomLimit && faded == true) {
            FadeCube(false);
            remake = true;
        }
        if (faded == false) {
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
                if (remake) {
                    RemakeInteractibleSurfaceByZoom(xTakeaway, yTakeaway);
                }
                else {
                    UpdateInteractibleSurface(xTakeaway, yTakeaway);
                }
                mainXTracker += xTakeaway;
                mainYTracker += yTakeaway;
                Debug.Log(string.Format("{0}, {1}", mainXTracker, mainYTracker));
                SaveData("maincubelocation");
            }
            else if (remake) {
                RemakeInteractibleSurfaceByZoom(0, 0);
            }
        }
    }

    void SaveScreenshot (bool fade, string ssname) {
        screenShotCamera.CaptureScreenshot(fade, ssname);
    }

    public void PutScreenshot (Sprite file, string name, Texture2D ss) {
        GameObject newss;
        SpriteRenderer newssS;
        PsuedoMono newssP;
        byte[] d = ss.GetRawTextureData();
        if (screensList.ContainsKey(name)) {
            screensList.Remove(name);
        }
        screensList.Add(name, d.ToString());
        if (spriteChildren != null && spriteChildren.ContainsKey(name)) {
            newss = spriteChildren[name];
            newssS = newss.GetComponent<SpriteRenderer>();
            newssP = newss.GetComponent<PsuedoMono>();
        }
        else {
            newss = Instantiate(spriteGO, spriteHolder.transform);
            spriteChildren.Add(name, newss);
            newssS = newss.AddComponent<SpriteRenderer>();
            newssP = newss.AddComponent<PsuedoMono>();
        }
        newssP.color = screenColor;
        newssS.sprite = file;
        newss.name = name;
        var x = (0.01f * -(mainXTracker));

        var y = (0.01f * -(mainYTracker));

        Debug.Log(string.Format("x - {0}, y - {1}", x, y));
        newss.transform.localPosition = new Vector3(
            x,
            y,
            0
        );
    }

    void FadeCube (bool fade) {
        faded = fade;
        if (fade) {
            FadeCubeCallback(fade);
        }
        else {
            fakeSurface.SetActive(fade);
            surface.SetActive(!fade);
        }
    }

    public void FadeCubeCallback (bool fade) {
        string mode = "screenslist";
        if (fade) {
            mode = "cubeletes_screenslist";
        }
        SaveData(mode);
        fakeSurface.SetActive(fade);
        surface.SetActive(!fade);
        FreeInteractibleSurfaceByZoom();
        remake = false;
    }

    void FreeInteractibleSurfaceByZoom () {
        var xLen = surfaceCubeletes.Count;
        for (int x = 0; x < xLen; x++) {
            List<CubeleteObject> layer = surfaceCubeletes[x];
            var yLen = layer.Count;
            for (int y = 0; y < yLen; y++) {
                temporaryFreedCubeletes.Add(layer[0]);
                layer[0].gameObject.SetActive(false);
                layer.RemoveAt(0);
            }
        }
    }

    void RemakeInteractibleSurfaceByZoom (float xc, float yc) {
        var yCubesAbs = (int)Mathf.Abs(yc);
        var xCubesAbs = (int)Mathf.Abs(xc);
        Right = xCubesAbs > 0 ? (int)(Right - xc) : Right;
        Left = xCubesAbs > 0 ? (int)(Left - xc) : Left;
        Top = yCubesAbs > 0 ? (int)(Top - yc) : Top;
        Bottom = yCubesAbs > 0 ? (int)(Bottom - yc) : Bottom;
        for (int xP = Left; xP < Right; xP++) {
            var layer = surfaceCubeletes[xP - Left];
            var currentCount = layer.Count;
            for (int yP = Bottom; yP < Top; yP++) {
                CubeleteObject newCubelete = temporaryFreedCubeletes[0];
                temporaryFreedCubeletes.RemoveAt(0);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    cubeleteBasePosition.x + (spawnMinDist * (xP)) - GlobalXOffset,
                    cubeleteBasePosition.y + (spawnMinDist * (yP)) - GlobalYOffset,
                    cubeleteBasePosition.z
                );
                var cubeComp = newCubelete.gameObject.GetComponent<Cubelete>();
                cubeComp.cube = this;
                layer.Add(newCubelete);
                cubeComp.x = xP;
                cubeComp.y = yP;
                newCubelete.x = cubeComp.x;
                newCubelete.y = cubeComp.y;
                newCubelete.gameObject.SetActive(true);
                cubeComp.EnableMeshR(true);
                if (deletedCubeletes.ContainsKey(string.Format("{0},{1}", cubeComp.x, cubeComp.y))) {
                    newCubelete.gameObject.SetActive(false);
                }

            }
        }
        remake = false;
    }

    public void MakeInteractibleSurface () {
        // every 0.02 of parent position is 1 cubelete width/height
        // so i have to create all cubeletes based on current parent position
        if (loaded == false) {
            Top = (int)-yStart;
            Bottom = (int)yStart;
            Left = (int)xStart;
            Right = (int)-xStart;
        }
        for (int i = Left; i < Right; i++) {
            surfaceCubeletes.Add(new List<CubeleteObject>());
            for (int j = Bottom; j < Top; j++) {
                for (int k = 0; k < zMax; k++) {
                    CubeleteObject newCubelete = new CubeleteObject();
                    newCubelete.gameObject = Instantiate(cubelete, surface.transform);
                    newCubelete.gameObject.transform.localPosition = new Vector3(
                        cubeleteBasePosition.x + (spawnMinDist * i) - GlobalXOffset,
                        cubeleteBasePosition.y + (spawnMinDist * j) - GlobalYOffset,
                        cubeleteBasePosition.z + (spawnMinDist * k)
                    );
                    var cubeComp = newCubelete.gameObject.GetComponent<Cubelete>();
                    cubeComp.x = i;
                    cubeComp.y = j;
                    cubeComp.piecesSurface = piecesSurface;
                    newCubelete.x = cubeComp.x;
                    newCubelete.y = cubeComp.y;
                    cubeComp.cube = this;
                    surfaceCubeletes[i - Left].Add(newCubelete);
                    if (deletedCubeletes.ContainsKey(string.Format("{0},{1}", cubeComp.x, cubeComp.y))) {
                        newCubelete.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void UpdateInteractibleSurface (float xCubes, float yCubes) {
        if (xCubes > 40f) {
            xCubes = 40f;
        }
        if (yCubes > 48f) {
            yCubes = 48f;
        }
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
                    temporaryFreedCubeletes.Add(layer[y]);
                    layer[y].gameObject.SetActive(false);
                    layer.RemoveAt(y);
                }
                else {
                    insertAtDel = x;
                    layer = surfaceCubeletes[xCubesAbs > 0 ? xDeleteOffset : xDeleteOffset - x];
                    temporaryFreedCubeletes.Add(layer[layer.Count - 1]);
                    layer[layer.Count - 1].gameObject.SetActive(false);
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
        //ad
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
                CubeleteObject newCubelete = temporaryFreedCubeletes[0];
                temporaryFreedCubeletes.RemoveAt(0);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    cubeleteBasePosition.x + (spawnMinDist * (yCubesAbs > 0 ? addX + xP : addX - xP)) - GlobalXOffset,
                    cubeleteBasePosition.y + (spawnMinDist * (xCubesAbs > 0 ? addY + yP : addY - yP)) - GlobalYOffset,
                    cubeleteBasePosition.z
                );
                var cubeComp = newCubelete.gameObject.GetComponent<Cubelete>();
                cubeComp.cube = this;
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
                newCubelete.gameObject.SetActive(true);
                cubeComp.EnableMeshR(true);
                if (deletedCubeletes.ContainsKey(string.Format("{0},{1}", cubeComp.x, cubeComp.y))) {
                    newCubelete.gameObject.SetActive(false);
                }
            }
        }
        if (post) {
            UpdateInteractibleSurface(0, postYCubes);
        }
    }

    public void addDeletedCubelete (int x, int y) {
        Debug.Log(string.Format("delete - {0}, {1}", x, y));
        deletedCubeletes.Add(string.Format("{0},{1}", x, y), true);
        cubeSaveNeeded = true;
        var screenname = string.Format("{0}_{1}_{2}_{3}", Left, Right, Top, Bottom);
        SaveScreenshot(false, screenname);
    }

    public void SaveData (string mode) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/bew.wyco");
        if (mode == "cubeletes" ||
            mode == "cubeletes_screenslist" ||
            mode == "cubeletes_maincubelocation" ||
            mode == "cubeletes_screenslis_maincubelocation"
           ) {
            _SaveDeletedCubeletesData();
        }
        if (mode == "screenslist" ||
            mode == "cubeletes_screenslist" ||
            mode == "screenslist_maincubelocation" ||
            mode == "cubeletes_screenslist_maincubelocation"
           ) {
            _SaveScreensList();
        }
        if (mode == "maincubelocation" ||
            mode == "screenslist_maincubelocation" ||
            mode == "cubeletes_maincubelocation" ||
            mode == "cubeletes_screenslist_maincubelocation"
           ) {
            _SaveMainCubeLocation();
        }
        bf.Serialize(file, data);
        file.Close();
    }
    private void _SaveDeletedCubeletesData () {
        data.deletedCubeletesData = deletedCubeletes;
    }
    private void _SaveScreensList () {
        data.screensList = screensList;
    }
    private void _SaveMainCubeLocation () {
        data.mainCubeLocation = new Dictionary<string, float>() {
            { "x", transform.localPosition.x },
            { "y", transform.localPosition.y },
            { "z", 0.5f }
        };
    }

    public void LoadData () {
        if (File.Exists((Application.persistentDataPath + "/bew.wyco"))) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/bew.wyco", FileMode.Open);
            data = (CubeData)bf.Deserialize(file);
            file.Close();
            deletedCubeletes = data.deletedCubeletesData != null ? data.deletedCubeletesData : deletedCubeletes;
            screensList = data.screensList != null ? data.screensList : screensList;
            if (data.mainCubeLocation != null) {
                transform.localPosition = new Vector3(
                    data.mainCubeLocation["x"],
                    data.mainCubeLocation["y"],
                    data.mainCubeLocation["z"]
                );
                var oldTrack = cubeTrackPosition;
                cubeTrackPosition = transform.localPosition;
                var tempXTracker = (cubeTrackPosition.x - oldTrack.x);
                var tempYTracker = (cubeTrackPosition.y - oldTrack.y);
                var tempXCubes = tempXTracker / 0.02f;
                var tempYCubes = tempYTracker / 0.02f;
                Left = (int)xStart - (int)tempXCubes;
                Right = (int)-xStart - (int)tempXCubes;
                Top = (int)-yStart - (int)tempYCubes;
                Bottom = (int)yStart - (int)tempYCubes;
                loaded = true;
            }
        }
    }

    public void DeleteSave () {
        if (File.Exists(Application.persistentDataPath + "/bew.wyco")) {
            File.Delete(Application.persistentDataPath + "/bew.wyco");
        }
        var keys = screensList.Keys;
        foreach (var key in keys) {
            if (File.Exists(key)) {
                File.Delete(key);
            }
            screensList.Remove(key);
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

[Serializable]
public class CubeData {
    public Dictionary<string, bool> deletedCubeletesData;
    public Dictionary<string, float> mainCubeLocation;
    public Dictionary<string, string> screensList;
}

public class PsuedoMono : MonoBehaviour {
    public Color color;
    void Awake () {
        GetComponent<SpriteRenderer>().color = color;
    }
}