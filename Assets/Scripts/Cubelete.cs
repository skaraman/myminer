using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubelete : MonoBehaviour {
    public float turnSpeed = 20f;
    public TheCube cube;
    public int x;
    public int y;

    public GameObject[] destructibleVersions;

    public GameObject animatingDestructible;
    public bool active = false;

    private bool alreadyClicked = false;

    void Start () {
    }

    void LateUpdate () {
        transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
        if (active) {
            TestToRemove();
        }
    }

    void OnMouseDown () {

        if (cube.zoomIn && cube.zoomOut && alreadyClicked == false) {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            cube.addDeletedCubelete(x, y);
            alreadyClicked = true;
            animatingDestructible.SetActive(true);
        }
    }

    public GameObject GetVersion () {
        int rnd = Random.Range(0, destructibleVersions.Length);
        GameObject version = destructibleVersions[rnd];
        return version;
    }

    void TestToRemove () {
        //foreach (var child in animatingDestructible.GetComponentInChildren<Transform>()) {
        //    //Debug.Log(child);
        //}
    }

    void Remove () {
        gameObject.SetActive(false);
    }
}
