using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grams : MonoBehaviour {

    public GameObject numerals;
    public GameObject label;

    private string[] multipliers = {"micro","milli","kilo","mega",
        "giga","tera","peta","exa","zetta","yotta","xenotta","bronto","geop",
        "xona","weka","vunda","uda","treda","sorta","rinta","quexa","pepta",
        "ocha","nena","minga","luma","hana","ana","strato"};

    private string[] units = {"gram","sequo","amphico","titano","jack","argenta",
        "para","raffle","gia","humo","mero","grea","quetza","blu","sarco","rikoxi"};

    private Text numeralsText;
    private Text labelText;

    private int[] fontSizesNumerals = new int[] { 200, 175, 150, 110, 90, 75 };
    private int[] fontSizesLabel = new int[] { 121, 112, 103, 94, 85, 76, 67, 58 };
    // Use this for initialization
    void Start () {
        numeralsText = numerals.GetComponent<Text>();
        labelText = label.GetComponent<Text>();

        ProcessTextSize("0", string.Format("{0}\n{1}", multipliers[0], units[0]));
    }

    void ProcessTextSize (string numberValue, string textString) {

        numeralsText.text = numberValue;
        labelText.text = textString;
    }

    public void AddGrams (string cubeName) {

    }
    // Update is called once per frame
    void Update () {

    }
}
