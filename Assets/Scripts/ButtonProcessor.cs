using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonProcessor : MonoBehaviour {
    public bool isPlusPressed = false;
    public bool isMinusPressed = false;
    public CameraHandler camHandler;
    private int pIt = 0;
    private int mIt = 0;

    public Animator top;
    public Animator bottom;
    private Dictionary<string, ButtonWrapper> buttonAnimations = new Dictionary<string, ButtonWrapper>();

    void Start () {
        buttonAnimations.Add("top", new ButtonWrapper(new string[] { "openTopUI", "closeTopUI" }, top));
        buttonAnimations.Add("bottom", new ButtonWrapper(new string[] { "openBottomUI", "closeBottomUI" }, bottom));
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

    public void onUIButton (string name) {
        var button = buttonAnimations[name];
        var animator = button.Animator;
        animator.Play(button.Names[button.Tracker]);
        button.Tracker = button.Tracker == 0 ? 1 : 0;
    }
}

public class ButtonWrapper {

    public ButtonWrapper (string[] names, Animator animator) {
        Names = names;
        Animator = animator;
    }

    public string[] Names { get; set; }
    public Animator Animator { get; set; }
    public int Tracker = 0;
}