using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Slot", menuName = "CustomScriptable/Slot")]
public class Slot : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int itemId;
    //public float itemPrice;
}