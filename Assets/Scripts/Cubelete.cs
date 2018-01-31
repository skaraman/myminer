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

    public GameObject piecesSurface;

    private bool alreadyClicked = false;
    private MeshRenderer mesh;
    private bool rotate = false;

    void Start () {
        mesh = GetComponent<MeshRenderer>();
    }

    public void EnableMeshR (bool en) {
        mesh = mesh != null ? mesh : GetComponent<MeshRenderer>();
        mesh.enabled = en;
    }

    void LateUpdate () {
        if (rotate) {
            transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
        }
    }

    void OnMouseDown () {
        if (cube.zoomIn && cube.zoomOut && alreadyClicked == false) {
            EnableMeshR(false);
            cube.addDeletedCubelete(x, y);
            alreadyClicked = true;
            animatingDestructible = Instantiate(GetVersion(), gameObject.transform.parent);
            cube.destros.Add(animatingDestructible);
            animatingDestructible.GetComponent<Destructible>().PublicReceiver(piecesSurface, gameObject);
            Remove();
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
