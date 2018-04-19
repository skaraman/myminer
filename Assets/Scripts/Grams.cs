using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grams : MonoBehaviour {

    public TheCube cube;
    public GameObject numerals;
    public GameObject label;
    public GameObject combo;

    private string[] multipliers = {"micro","milli","kilo","mega",
        "giga","tera","peta","exa","zetta","yotta","xenotta","bronto","geop",
        "xona","weka","vunda","uda","treda","sorta","rinta","quexa","pepta",
        "ocha","nena","minga","luma","hana","ana","strato"};

    private string[] units = {"phot","neutri","elect","qua","prot","ato",
        "molec","cel","humo","plane","sol","quetza","blu","sarco","rikoxi"};

    private int multiIndex = 0;
    private int unitIndex = 0;

    private Text numeralsText;
    private Text labelText;
    private Text comboText;

    private int[] fontSizesNumerals = new int[] { 200, 175, 150, 110, 90, 75 };
    private int[] fontSizesLabel = new int[] { 99, 95, 91, 87, 83, 79, 75, 71 };

    private Shadow numShadow;
    private Shadow labelShadow;
    private Outline numOutline;
    private Outline labelOutline;
    private Shadow comboShadow;
    private Outline comboOutline;

    private Dictionary<string, int> valuesKeeper = new Dictionary<string, int>();

    private int multiLength;
    private int unitLength;
    private int numLength;

    public bool started = false;

    // Use this for initialization
    void Start () {
        numeralsText = numerals.GetComponent<Text>();
        labelText = label.GetComponent<Text>();
        ProcessTextSize("0", GetValueName());
        numShadow = numerals.GetComponent<Shadow>();
        labelShadow = label.GetComponent<Shadow>();
        numOutline = numerals.GetComponent<Outline>();
        labelOutline = label.GetComponent<Outline>();
        comboShadow = combo.GetComponent<Shadow>();
        comboOutline = combo.GetComponent<Outline>();
        started = true;
    }

    string GetValueName () {
        multiLength = multipliers[multiIndex].Length;
        unitLength = units[unitIndex].Length;
        return string.Format("{0}\n{1}", multipliers[multiIndex], units[unitIndex]);
    }

    void ProcessTextSize (string numberValue, string textString) {
        numeralsText.text = numberValue;
        labelText.text = textString;
        numeralsText.fontSize = fontSizesNumerals[numLength];
        labelText.fontSize = fontSizesLabel[multiLength > unitLength ? multiLength : unitLength];
    }

    public void AddGrams (string cubeName) {
        int value = 0;
        int score = 1;
        if (valuesKeeper.ContainsKey(GetValueName())) {
            value = valuesKeeper[GetValueName()];
            value += score;
            valuesKeeper[GetValueName()] = value;
        }
        else {
            valuesKeeper.Add(GetValueName(), value + score);
        }
        cube._SaveGrams(new GramsKeeper(multiIndex, unitIndex, valuesKeeper));
        numLength = valuesKeeper[GetValueName()].ToString().Length;
        ProcessTextSize(valuesKeeper[GetValueName()].ToString(), GetValueName());
    }

    public void LoadGrams (GramsKeeper gramsKeeper) {
        multiIndex = gramsKeeper.multiI;
        unitIndex = gramsKeeper.unitI;
        valuesKeeper = gramsKeeper.values;
        numLength = valuesKeeper[GetValueName()].ToString().Length;
        ProcessTextSize(valuesKeeper[GetValueName()].ToString(), GetValueName());
    }

    public void SetColor (Color color) {
        numShadow.effectColor = color;
        labelShadow.effectColor = color;
        numOutline.effectColor = color;
        labelOutline.effectColor = color;
        comboShadow.effectColor = color;
        comboOutline.effectColor = color;
    }
    // Update is called once per frame
    //void Update () {

    //}
}


[System.Serializable]
public class GramsKeeper {
    public int multiI;
    public int unitI;
    public Dictionary<string, int> values;

    public GramsKeeper (int mI, int uI, Dictionary<string, int> v) {
        multiI = mI;
        unitI = uI;
        values = v;
    }
}