using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonProcessor : MonoBehaviour {
    public bool isPlusPressed = false;
    public bool isMinusPressed = false;
    public CameraHandler camHandler;
    public GameObject topUIButton;
    private int pIt = 0;
    private int mIt = 0;

    private Animator top;

    void Start () {
    }

    void Update () {
        if (isPlusPressed) {
            pIt++;
            camHandler.ZoomButton(-1.0f * pIt);
        }
        else
        if (isMinusPressed) {
            mIt++;
            camHandler.ZoomButton(1.0f * mIt);
        }
    }

    public void onPointerDownButton (string buttonType) {
        if (buttonType == "plus") {
            isPlusPressed = true;
        }
        else if (buttonType == "minus") {
            isMinusPressed = true;
        }
    }

    public void onPointerUpButton (string buttonType) {
        if (buttonType == "plus") {
            isPlusPressed = false;
            pIt = 0;
        }
        else if (buttonType == "minus") {
            isMinusPressed = false;
            mIt = 0;
        }
    }

    public void onTopUIButton () {
        top = GetComponent<Animator>();
        top.Play("openTopUI");
    }
}