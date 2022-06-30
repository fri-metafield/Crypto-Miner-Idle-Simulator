using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Currency", menuName = "CustomScriptable/Currency")]
public class Currencies : ScriptableObject
{
    public string currencyName;
    public string currencyToken;
    public Sprite currencyImage;
    public int currencyID;

    public float currencyEarnValue;

    public float currencyEarnTime;

    public float currencyUnlockSpeed;
}
