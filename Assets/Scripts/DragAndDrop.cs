using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject DraggedInstance;
    Vector3 _startPosition;
    Vector3 _offsetToMouse;
    float _zDistanceToCamera;
    Vector3 temp;
    public TheCube cube;
    public GameObject piecesSurface;

    public void OnBeginDrag (PointerEventData eventData) {
        DraggedInstance = gameObject;
        _startPosition = transform.position;
        _zDistanceToCamera = Mathf.Abs(_startPosition.z - Camera.main.transform.position.z);
        _offsetToMouse = _startPosition - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, _zDistanceToCamera)
        );
        _offsetToMouse.z = 0;
    }

    public void OnDrag (PointerEventData eventData) {
        if (Input.touchCount > 1)
            return;
        temp = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, _startPosition.z)
        ) + _offsetToMouse;
        //var savedPosition = transform.position;
        transform.position = new Vector3(temp.x, temp.y, _startPosition.z);
        //piecesSurface.transform.position = new Vector3(temp.x, temp.y, _startPosition.z);
        //transform.position = new Vector3(temp.x, _startPosition.y, _startPosition.z);
        //		var xCubes = Mathf.RoundToInt ((savedPosition.x - temp.x) / 0.02f);
        //		var yCubes = Mathf.RoundToInt ((savedPosition.y - temp.y) / 0.02f);
        //		if (xCubes > 0 || yCubes > 0) {
        //			cube.UpdateInteractibleSurface (xCubes, yCubes);
        //		}

    }

    public void OnEndDrag (PointerEventData eventData) {
        DraggedInstance = null;
        _offsetToMouse = Vector3.zero;
        cube.cubeSaveNeeded = true;
    }
}