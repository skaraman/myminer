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

    Vector3 startPOS;

    private bool faded = false;
    private bool tweaked = false;

    //private Data data = new Data ();
    //private string cubedata;   

    void Start()
    {
        startPOS = cubelete.transform.localPosition;
    }

    void Update()
    {
        Vector3 currPos = transform.localPosition;

        if (currPos.z > 0.51 && faded == false)
        {
            FadeCube(true);
        }
        else
        if (currPos.z <= 0.51 && faded == true)
        {
            FadeCube(false);
        }
        //else
        //if (currPos.z < 0.3 && tweaked == false)
        //{
        //    TweakSurface(true);
        //}
        //else
        //if (currPos.z >= 0.3 && tweaked == true)
        //{
        //    TweakSurface(false);
        //}
    }

    void FadeCube(bool fade)
    {
        faded = fade;
        fakeSurface.SetActive(fade);
        surface.SetActive(!fade);
    }

    void TweakSurface(bool tweak)
    {
        tweaked = tweak;
        Component[] children = surface.GetComponentsInChildren<Transform>();
        float it;
        foreach (Transform child in children)
        {
            float newX = 0.0f;
            float newY = 0.0f;

            if (child.localPosition.x > 0)
            {
                it = child.localPosition.x / 0.02f;
                newX = Operator(child.localPosition.x, (0.001f * it), tweak);
            }
            if (child.localPosition.y > 0)
            {
                it = child.localPosition.y / 0.02f;
                newY = Operator(child.localPosition.y, (0.001f * it), tweak);
            }
            //iTween.MoveBy(child.gameObject, iTween.Hash("x", newX, "y", newY, "easeType", "easeInOutExpo", "delay", .1));

            child.localPosition = new Vector3(newX, newY, child.localPosition.z);
        }
    }

    public void MakeInteractibleSurface()
    {
        var parentX = gameObject.transform.position.x;
        var parentY = gameObject.transform.position.y;
        // every 0.02 of parent position is 1 cubelete width/height
        // so i have to create all cubeletes based on current parent position


        for (int i = 0; i < xMax; i++)
        {
            for (int j = 0; j < yMax; j++)
            {
                for (int k = 0; k < zMax; k++)
                {
                    GameObject newCubelete = Instantiate(cubelete, surface.transform);
                    newCubelete.transform.localPosition = new Vector3(
                        startPOS.x + (spawnMinDist * i),
                        startPOS.y + (spawnMinDist * j),
                        startPOS.z + (spawnMinDist * k)
                    );
                }
            }
        }
    }

    //public void MakeInteractibleSurface(int[][][] data)
    //{
    //    for (int i = 0; i < xMax; i++)
    //    {
    //        for (int j = 0; j < yMax; j++)
    //        {
    //            for (int k = 0; k < zMax; k++)
    //            {
    //                if (data[i][j][k] == 1)
    //                {
    //                    GameObject newCubelete = Instantiate(cubelete, surface.transform);
    //                    newCubelete.transform.localPosition = new Vector3(
    //                        startPOS.x + (spawnMinDist * i),
    //                        startPOS.y + (spawnMinDist * j),
    //                        startPOS.z + (spawnMinDist * k)
    //                    );
    //                }
    //            }
    //        }
    //    }
    //}

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
