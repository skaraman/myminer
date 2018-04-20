using UnityEngine;
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
    public Grams grams;

    private bool faded = false;
    private bool remake = false;
    private bool loaded = false;

    private float xTracker;
    private float yTracker;
    private float xCubes;
    private float yCubes;
    private float xTakeaway;
    private float yTakeaway;

    private float xStart;
    private float yStart;

    private int Left;
    private int Right;
    private int Top;
    private int Bottom;

    private int LeftMax = -500;
    private int RightMax = 499;
    private int TopMax = 519;
    private int BottomMax = -480;

    private int ogLeft;
    private int ogRight;
    private int ogTop;
    private int ogBottom;

    private float minCubeYZoomed = -10f;
    private float maxCubeYZoomed = 10f;
    private float minCubeXZoomed = -10f;
    private float maxCubeXZoomed = 10f;

    private float minCubeYFaded = -10f;
    private float maxCubeYFaded = 10f;
    private float minCubeXFaded = -10f;
    private float maxCubeXFaded = 10f;

    public List<GameObject> destros;
    public GameObject spriteHolder;
    public GameObject spriteGO;

    public Dictionary<string, bool> deletedCubeletes = new Dictionary<string, bool>();
    public Dictionary<string, bool> screensList = new Dictionary<string, bool>();

    public Color spriteShaderColor = new Color(0.66f, 0.47f, 0.09f, 1);
    private Color white = new Color(1, 1, 1, 1);
    private Color transp = new Color(1, 1, 1, 0);
    private Rect rect;

    private List<List<CubeleteObject>> surfaceCubeletes = new List<List<CubeleteObject>>();
    private List<CubeleteObject> temporaryFreedCubeletes = new List<CubeleteObject>();
    private Dictionary<string, GameObject> spriteChildren = new Dictionary<string, GameObject>();

    public int cubeSaveLimit = 30;
    private int cubeSaveIterator = 0;
    public bool cubeSaveNeeded = false;

    private int captureWidth;
    private int captureHeight;

    public Material ssMat;
    CubeData data;

    bool colorSet = false;
    bool attemptToLoadGrams = false;

    void Start () {
        captureWidth = (int)xMax;
        captureHeight = (int)yMax;
        rect = new Rect(0, 0, captureWidth, captureHeight);
        xStart = -xMax / 2;
        yStart = -yMax / 2;
        ogLeft = (int)xStart;
        ogRight = -(int)xStart;
        ogTop = -(int)yStart;
        ogBottom = (int)yStart;
        data = new CubeData();
        cubeleteBasePosition = cubelete.transform.localPosition;
        cubeTrackPosition = transform.localPosition;
        if (File.Exists((Application.persistentDataPath + "/bew.wyco"))) {
            LoadData();
        }
        MakeInteractibleSurface();
    }

    void Awake () {
        //iOS serializer fix // supposed
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
    }

    void Update () {
        if (!colorSet && grams && grams.started) {
            grams.SetColor(spriteShaderColor);
            colorSet = true;
        }
        if (attemptToLoadGrams && grams && grams.started) {
            grams.LoadGrams(data.grams);
        }
        if (cubeSaveIterator > cubeSaveLimit && cubeSaveNeeded == true) {
            cubeSaveIterator = 0;
            SaveData("cubeletes_screenslist_maincubelocation");
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
        if (faded == false && currPos != cubeTrackPosition) {
            if (currPos.y < minCubeYZoomed) {
                currPos.y = minCubeYZoomed;
            }
            else if (currPos.y > maxCubeYZoomed) {
                currPos.y = maxCubeYZoomed;
            }
            if (currPos.x < minCubeXZoomed) {
                currPos.x = minCubeXZoomed;
            }
            else if (currPos.x > maxCubeXZoomed) {
                currPos.x = maxCubeXZoomed;
            }
            transform.localPosition = new Vector3(currPos.x, currPos.y, currPos.z);
            xTracker += (currPos.x - cubeTrackPosition.x);
            yTracker += (currPos.y - cubeTrackPosition.y);
            cubeTrackPosition = currPos;
            xCubes = xTracker / 0.02f;
            yCubes = yTracker / 0.02f;
            if (Mathf.Abs(xCubes) > 1 || Mathf.Abs(yCubes) > 1) {
                xTakeaway = Mathf.Round(xCubes);
                yTakeaway = Mathf.Round(yCubes);
                xTracker -= xTakeaway * 0.02f;
                yTracker -= yTakeaway * 0.02f;
                if (remake) {
                    RemakeInteractibleSurfaceByZoom(xTakeaway, yTakeaway);
                }
                else {
                    //if (Left <= -500 || Right >= 500 || Top >= 520 || Bottom <= -480) {
                    //    return;
                    //}
                    UpdateInteractibleSurface(xTakeaway, yTakeaway);
                }
            }
            else if (remake) {
                RemakeInteractibleSurfaceByZoom(0, 0);
            }
        }
        else if (faded == true) {
            if (currPos.y < minCubeYFaded) {
                currPos.y = minCubeYFaded;
            }
            else if (currPos.y > maxCubeYFaded) {
                currPos.y = maxCubeYFaded;
            }
            if (currPos.x < minCubeXFaded) {
                currPos.x = minCubeXFaded;
            }
            else if (currPos.x > maxCubeXFaded) {
                currPos.x = maxCubeXFaded;
            }
            transform.localPosition = new Vector3(currPos.x, currPos.y, currPos.z);
        }
    }

    public void PutScreenshot (Sprite file, string name, Texture2D ss) {
        GameObject newss;
        RawImage newssS;
        PsuedoMono newssP;
        //byte[] d = ss.GetRawTextureData();
        if (!screensList.ContainsKey(name)) {
            screensList.Add(name, true);
        }
        if (spriteChildren != null && spriteChildren.ContainsKey(name)) {
            newss = spriteChildren[name];
            newssS = newss.GetComponent<RawImage>();
            newssP = newss.GetComponent<PsuedoMono>();
        }
        else {
            newss = Instantiate(spriteGO, spriteHolder.transform);
            spriteChildren.Add(name, newss);
            newssS = newss.AddComponent<RawImage>();
            newssP = newss.AddComponent<PsuedoMono>();
        }
        var rectT = newss.GetComponent<RectTransform>();
        rectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, captureWidth);
        rectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, captureHeight);
        rectT.localScale = new Vector3(0.00784f, 0.00784f, 1);
        newssS.texture = ss;
        newssS.material = ssMat;
        newssP.color = spriteShaderColor;
        newss.name = name;
        var x = (float)Math.Round(0.0078125f * -(ogLeft - Left), 4);
        var y = (float)Math.Round(0.0078125f * -(ogBottom - Bottom), 3);
        //Debug.Log(string.Format("x - {0}, y - {1}", x, y));
        newss.transform.localPosition = new Vector3(
            x,
            y,
            0
        );
    }

    void RemakeScreenshots () {
        foreach (var s in screensList) {
            var st = s.Key.Split('_');
            Left = int.Parse(st[0]);
            Right = int.Parse(st[1]);
            Bottom = int.Parse(st[2]);
            Top = int.Parse(st[3]);
            var screenname = string.Format("{0}_{1}_{2}_{3}", Left, Right, Bottom, Top);
            CaptureScreenshot(screenname);
        }
    }

    void CaptureScreenshot (string ssname) {
        Sprite sp = null;
        Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.ARGB32, false);
        Color[] pixels = screenShot.GetPixels(0, 0, screenShot.width, screenShot.height, 0);
        var yKeeper = 0;
        var yMulti = 1;
        int y = Bottom;
        for (int p = 0; p < pixels.Length; p++) {
            var x = Left + (p % screenShot.width);
            if (yKeeper >= screenShot.width) {
                y = Bottom + yMulti;
                yMulti++;
                yKeeper = 0;
            }
            yKeeper++;
            var key = string.Format("{0},{1}", x, y);
            if (deletedCubeletes.ContainsKey(key)) {
                pixels[p] = white;
            }
            else {
                pixels[p] = transp;
            }
        }
        screenShot.SetPixels(0, 0, screenShot.width, screenShot.height, pixels, 0);
        screenShot.Apply();
        screenShot.filterMode = FilterMode.Point;
        sp = Sprite.Create(screenShot, rect, new Vector2(0.5f, 0.5f), 128f);
        PutScreenshot(sp, ssname, screenShot);
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
        cubeSaveNeeded = true;
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
                    RoundToExactlyTwoDecimals(cubeleteBasePosition.x + (spawnMinDist * (xP)) - GlobalXOffset),
                    RoundToExactlyTwoDecimals(cubeleteBasePosition.y + (spawnMinDist * (yP)) - GlobalYOffset),
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
                if (deletedCubeletes.ContainsKey(string.Format("{0},{1}", cubeComp.x, cubeComp.y)) ||
                cubeComp.x > RightMax ||
                cubeComp.x < LeftMax ||
                cubeComp.y > TopMax ||
                cubeComp.y < BottomMax) {
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
                        RoundToExactlyTwoDecimals(cubeleteBasePosition.x + (spawnMinDist * i) - GlobalXOffset),
                        RoundToExactlyTwoDecimals(cubeleteBasePosition.y + (spawnMinDist * j) - GlobalYOffset),
                        RoundToExactlyTwoDecimals(cubeleteBasePosition.z + (spawnMinDist * k))
                    );
                    var cubeComp = newCubelete.gameObject.GetComponent<Cubelete>();
                    cubeComp.x = i;
                    cubeComp.y = j;
                    cubeComp.piecesSurface = piecesSurface;
                    newCubelete.x = cubeComp.x;
                    newCubelete.y = cubeComp.y;
                    cubeComp.cube = this;
                    surfaceCubeletes[i - Left].Add(newCubelete);
                    if (deletedCubeletes.ContainsKey(string.Format("{0},{1}", cubeComp.x, cubeComp.y)) ||
                    cubeComp.x > RightMax ||
                    cubeComp.x < LeftMax ||
                    cubeComp.y > TopMax ||
                    cubeComp.y < BottomMax) {
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
                CubeleteObject newCubelete = temporaryFreedCubeletes[0];
                temporaryFreedCubeletes.RemoveAt(0);
                newCubelete.gameObject.transform.localPosition = new Vector3(
                    RoundToExactlyTwoDecimals(cubeleteBasePosition.x + (spawnMinDist * (yCubesAbs > 0 ? addX + xP : addX - xP)) - GlobalXOffset),
                    RoundToExactlyTwoDecimals(cubeleteBasePosition.y + (spawnMinDist * (xCubesAbs > 0 ? addY + yP : addY - yP)) - GlobalYOffset),
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
                if (deletedCubeletes.ContainsKey(string.Format("{0},{1}", cubeComp.x, cubeComp.y)) ||
                cubeComp.x > RightMax ||
                cubeComp.x < LeftMax ||
                cubeComp.y > TopMax ||
                cubeComp.y < BottomMax) {
                    newCubelete.gameObject.SetActive(false);
                }
            }
        }
        if (post) {
            UpdateInteractibleSurface(0, postYCubes);
        }
    }

    public void addDeletedCubelete (int x, int y) {
        deletedCubeletes.Add(string.Format("{0},{1}", x, y), true);
        cubeSaveNeeded = true;
        var screenname = string.Format("{0}_{1}_{2}_{3}", Left, Right, Bottom, Top);
        CaptureScreenshot(screenname);
        grams.AddGrams(ssMat.name);
    }

    public void SaveData (string mode) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/bew.wyco");
        if (mode == "cubeletes" ||
            mode == "cubeletes_screenslist" ||
            mode == "cubeletes_maincubelocation" ||
            mode == "cubeletes_screenslist_maincubelocation"
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
        //data.cubeName = ssMat.name;
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
            { "z", transform.localPosition.z }
        };
    }
    public void _SaveGrams (GramsKeeper values) {
        data.grams = values;
    }
    public void LoadData () {
        if (!File.Exists((Application.persistentDataPath + "/bew.wyco"))) {
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/bew.wyco", FileMode.Open);
        data = (CubeData)bf.Deserialize(file);
        file.Close();
        deletedCubeletes = data.deletedCubeletesData != null ? data.deletedCubeletesData : deletedCubeletes;
        screensList = data.screensList != null ? data.screensList : screensList;
        if (deletedCubeletes.Count > 0) {
            RemakeScreenshots();
        }
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
        if (data.grams != null) {
            attemptToLoadGrams = true;
        }

    }

    public void DeleteSave () {
        if (File.Exists(Application.persistentDataPath + "/bew.wyco")) {
            File.Delete(Application.persistentDataPath + "/bew.wyco");
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

    static float RoundToExactlyTwoDecimals (float value) {
        float result = (float)Math.Round(value, 2) + 0.0001f;
        return result;
    }
}

public class CubeleteObject {
    public GameObject gameObject;
    public int x;
    public int y;
}

[Serializable]
public class CubeData {
    public string cubeName;
    public Dictionary<string, bool> deletedCubeletesData;
    public Dictionary<string, float> mainCubeLocation;
    public Dictionary<string, bool> screensList;
    public GramsKeeper grams;
}

public class PsuedoMono : MonoBehaviour {
    public Color color;
    void Awake () {
        GetComponent<RawImage>().color = color;
    }
}