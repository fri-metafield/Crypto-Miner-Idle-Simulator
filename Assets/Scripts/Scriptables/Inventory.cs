using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "CustomScriptable/Inventory")]
public class Inventory : ScriptableObject
{
    public ItemManager allItems;
    public List<InventoryContent> invContent = new List<InventoryContent>();
    public int usableSlots;
    public int maxSlot;

    public int startingSlot;

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        foreach (InventoryContent inv in invContent)
        {
            inv.gpuBrand = null;
            inv.gpuModel = null;
            inv.gpuSeries = null;
            inv.gpuVersion = null;
        }
    }

    [ContextMenu("Reset Slots")]
    public void ResetInventorySlot()
    {
        usableSlots = startingSlot;
    }

    public void SetSlot(int slot, int brandID, int modelID, int seriesID, int versionID)
    {
        if(seriesID < 0)
        {
            invContent[slot].gpuBrand = null;
            invContent[slot].gpuModel = null;
            invContent[slot].gpuSeries = null;
            invContent[slot].gpuVersion = null;
            return;
        }
        invContent[slot].gpuBrand = allItems.allItems[brandID] as Brand;
        invContent[slot].gpuModel = allItems.allItems[modelID] as GPUModel;
        invContent[slot].gpuSeries = allItems.allItems[seriesID] as GPUSeries;
        invContent[slot].gpuVersion = allItems.allItems[versionID] as GPUVersion;

    }
}

[System.Serializable]
public class InventoryContent
{
    //public GPU gpuType;
    //public Brand brandName;
    public Brand gpuBrand;
    public GPUModel gpuModel;
    public GPUSeries gpuSeries;
    public GPUVersion gpuVersion;
}