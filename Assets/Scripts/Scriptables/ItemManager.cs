using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Manager", menuName = "CustomScriptable/Item Manager")]
public class ItemManager : ScriptableObject
{
    public List<Slot> allItems = new List<Slot>();
    public List<Currencies> allCurrencies = new List<Currencies>();

    [ContextMenu("Set Item")]
    public void SetItemID()
    {
        foreach (Slot item in allItems)
        {
            item.itemId = allItems.IndexOf(item);
        }
        foreach (Currencies item in allCurrencies)
        {
            item.currencyID = allCurrencies.IndexOf(item);
        }

    }
}
