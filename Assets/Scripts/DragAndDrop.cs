using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject DraggedInstance;
    Vector3 _startPosition;
    Vector3 _offsetToMouse;
    float _zDistanceToCamera;
    Vector3 temp;
    public void OnBeginDrag(PointerEventData eventData)
    {
        DraggedInstance = gameObject;
        _startPosition = transform.position;
        _zDistanceToCamera = Mathf.Abs(_startPosition.z - Camera.main.transform.position.z);
        _offsetToMouse = _startPosition - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, _zDistanceToCamera)
        );
        _offsetToMouse.z = 0;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1)
            return;
        temp = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, _startPosition.z)
        ) + _offsetToMouse;

        transform.position = new Vector3(temp.x, temp.y, _startPosition.z);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        DraggedInstance = null;
        _offsetToMouse = Vector3.zero;
    }
}