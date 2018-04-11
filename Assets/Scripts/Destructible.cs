using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Destructible : MonoBehaviour {
    private bool dissolve;
    private float amount;
    private Renderer[] children;
    private int updateDelayer;
    private int updateDelayLimit = 1;
    // Use this for initialization

    void Start () {
        dissolve = false;
        amount = 0;
        updateDelayer = 0;
        children = gameObject.GetComponentsInChildren<Renderer>();
        DestroyHalf();
    }

    private Renderer[] RemoveIndices (Renderer[] indicesArray, int[] keepAt) {
        Renderer[] newIndicesArray = new Renderer[keepAt.Length];
        int i = 0;
        int j = 0;
        while (i < indicesArray.Length) {
            if (keepAt.Contains(i)) {
                newIndicesArray[j] = indicesArray[i];
                j++;
            }
            else {
                Destroy(indicesArray[i].gameObject);
            }
            i++;
        }
        return newIndicesArray;
    }

    void DestroyHalf () {
        var childrenToDestroy = RandomRange(children.Count() / 2, children.Count());
        children = RemoveIndices(children, childrenToDestroy);
    }

    public void PublicReceiver (GameObject piecesSurface, GameObject cubelete) {
        StartCoroutine(
            FollowThrough(piecesSurface, cubelete)
        );
    }

    IEnumerator FollowThrough (GameObject piecesSurface, GameObject cubelete) {
        transform.localPosition = cubelete.transform.localPosition;
        yield return new WaitForSeconds(0.1f);
        transform.SetParent(piecesSurface.transform, true);
        yield return new WaitForSeconds(1f);
        dissolve = true;
    }

    private void LateUpdate () {
        if (dissolve && updateDelayer > updateDelayLimit) {
            amount += 0.1f;
            updateDelayer = 0;
            foreach (var c in children) {
                c.material.SetFloat("_DisAmount", amount);
            }
            if (amount > 1) {
                Destroy(gameObject);
            }
        }
        updateDelayer++;
    }

    private int[] RandomRange (int max, int range) {
        System.Random randNum = new System.Random();
        int[] test2 = Enumerable
            .Range(0, range)
            .OrderBy(i => randNum.Next())
            .Take(max)
            .ToArray();
        return test2;
    }

}

