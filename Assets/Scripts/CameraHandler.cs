using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {
    private static readonly float ZoomSpeedTouch = 0.1f;
    //private static readonly float ZoomSpeedMouse = 0.5f;
    public bool wasZoomingLastFrame;     // Touch mode only
    Vector2[] lastZoomPositions;     // Touch mode only
    //private bool mouseInitiatied = false;
    //private float initMousePositionY;
    public GameObject theCube;

    void Update () {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer) {
            HandleTouch();
        }
    }

    void HandleTouch () {
        switch (Input.touchCount) {
            case 2:     // Zooming

            Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
            if (!wasZoomingLastFrame) {
                lastZoomPositions = newPositions;
                wasZoomingLastFrame = true;
            }
            else {
                // Zoom based on the distance between the new positions compared to the
                // distance between the previous positions.
                float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                float offset = newDistance - oldDistance;
                ZoomCube((ZoomSpeedTouch * -offset) * 0.01f);
                lastZoomPositions = newPositions;
            }
            break;
            default:
            wasZoomingLastFrame = false;
            break;
        }
    }

    void ZoomCube (float speed) {
        float currMousePositionY = Input.mousePosition.y;
        Vector3 cubePosition = new Vector3(theCube.transform.position.x, theCube.transform.position.y, theCube.transform.position.z);
        float diff = (currMousePositionY) * 0.001f;
        float newZ = cubePosition.z + (diff * speed * cubePosition.z);
        if (newZ < 0.15f) {
            newZ = 0.15f;
        }
        theCube.transform.position = new Vector3(cubePosition.x, cubePosition.y, newZ);
    }

    public void ZoomButton (float direction) {
        Vector3 cubePosition = theCube.transform.position;
        float newZ = cubePosition.z + (direction * 0.01f);
        if (newZ < 0.15f) {
            newZ = 0.15f;
        }
        if (newZ > 15f) {
            newZ = 15f;
        }
        theCube.transform.position = new Vector3(cubePosition.x, cubePosition.y, newZ);
    }
}
