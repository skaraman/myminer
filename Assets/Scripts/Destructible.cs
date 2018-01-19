using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {
    // Use this for initialization
    void Start () {
    }
    // Update is called once per frame
    void Update () { }

    public void TryThis (GameObject piecesSurface, GameObject cubelete) {
        StartCoroutine(
            FollowThrough(piecesSurface, cubelete)
        );
    }

    IEnumerator FollowThrough (GameObject piecesSurface, GameObject cubelete) {
        transform.localPosition = cubelete.transform.localPosition;
        yield return new WaitForSeconds(0.1f);
        transform.SetParent(piecesSurface.transform, true);
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}