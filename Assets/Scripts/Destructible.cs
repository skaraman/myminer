using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {
    private bool dissolve;
    private float amount;
    private Renderer[] children;
    private int updateDelayer;
    // Use this for initialization

    void Start () {
        dissolve = false;
        amount = 0;
        updateDelayer = 0;
        children = gameObject.GetComponentsInChildren<Renderer>();
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
        dissolve = true;
    }

    private void LateUpdate () {
        if (dissolve && updateDelayer > 3) {
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
}