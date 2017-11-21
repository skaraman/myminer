using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour
{

    private static readonly float ZoomSpeedTouch = 0.1f;
    private static readonly float ZoomSpeedMouse = 0.5f;

    private bool wasZoomingLastFrame;     // Touch mode only
    private Vector2[] lastZoomPositions;     // Touch mode only

    private bool mouseInitiatied = false;
    private float initMousePositionY;

    public GameObject theCube;

    void Update()
    {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else if (Input.GetMouseButton(0))
        {
            HandleMouse();
        }
        else
        {
            mouseInitiatied = false;
        }
    }

    void HandleTouch()
    {
        switch (Input.touchCount)
        {
            case 2:     // Zooming
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;
                    ZoomCube(ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void HandleMouse()
    {
        if (mouseInitiatied == false)
        {
            mouseInitiatied = true;
            initMousePositionY = Input.mousePosition.y;
        }

        ZoomCube(ZoomSpeedMouse);
    }

    void ZoomCube(float speed)
    {
        float currMousePositionY = Input.mousePosition.y;
        Vector3 cubePosition = theCube.transform.position;
        float diff = (initMousePositionY - currMousePositionY) * 0.001f;
        float newZ = cubePosition.z + (diff * speed);
        theCube.transform.position = new Vector3(cubePosition.x, cubePosition.y, newZ);
    }
}
