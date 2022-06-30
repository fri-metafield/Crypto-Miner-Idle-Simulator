using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayInventory : MonoBehaviour
{
    public Inventory inv;
    public List<SelectionInventory> inventorySlots = new List<SelectionInventory>();

    public GameObject invFullPopup;

    int usableSlot = 4;

    public static GameplayInventory instance;
    private void Awake()
    {
        instance = this;
        

        while (inv.invContent.Count < inv.maxSlot)
        {
            InventoryContent newAddition = new InventoryContent();
            inv.invContent.Add(newAddition);
        }
       
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(SelectionInventory slot in inventorySlots)
        {
            slot.gameObject.name = "Inventory Slot " + (inventorySlots.IndexOf(slot) + 1);
        }
        RefreshList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshList()
    {
        usableSlot = inv.usableSlots;

        //inventory slots
        foreach (SelectionInventory slot in inventorySlots)
        {
            if(inventorySlots.IndexOf(slot) <= (usableSlot-1))
            {
                slot.isUnlocked = true;
                slot.isUsable = true;
            }
            else
            {
                slot.isUnlocked = false;
            }

            slot.gpuBrand = inv.invContent[inventorySlots.IndexOf(slot)].gpuBrand;
            slot.gpuModel = inv.invContent[inventorySlots.IndexOf(slot)].gpuModel;
            slot.gpuSeries = inv.invContent[inventorySlots.IndexOf(slot)].gpuSeries;
            slot.gpuVersion = inv.invContent[inventorySlots.IndexOf(slot)].gpuVersion;
            
        }

        GameplayMarket.instance.RefreshSlotList();

        Debug.Log("Refreshed Inventory");
    }



    public bool InventoryHasEnough()
    {
        if (inv.invContent.Count <= 0) return true;
        var counter = 0;
        foreach(InventoryContent invCon in inv.invContent)
        {
            if(invCon.gpuSeries)
            {
                counter++;
            }
        }
        if (counter < inv.usableSlots)
        {
            return true;
        }

        invFullPopup.SetActive(true);

        Debug.Log("Not enough space");
        return false;
    }

    public void InventoryAddItem(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv)
    {
        bool added = false;
        if (InventoryHasEnough())
        {
            foreach (InventoryContent invCon in inv.invContent)
            {
                if (added) return;

                if (!invCon.gpuSeries) //found emppty
                {
                    AddHere(gb, gm, gs, gv, inv.invContent.IndexOf(invCon)); //add to empty
                    added = true;

                    RefreshList();
                    return;
                }
            }


        }
        else return;

        RefreshList();
    }

    void AddHere(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv, int index)
    {
        InventoryContent newAddition = inv.invContent[index];

        newAddition.gpuBrand = gb;
        newAddition.gpuModel = gm;
        newAddition.gpuSeries = gs;
        newAddition.gpuVersion = gv;

        Debug.Log("Added " + newAddition.gpuBrand + " " + newAddition.gpuModel);
    }

    public void InventoryRemoveItem(int index)
    {
        foreach(InventoryContent slot in inv.invContent)
        {
            if(inv.invContent.IndexOf(slot) == index)
            {
                slot.gpuBrand = null;
                slot.gpuModel = null;
                slot.gpuSeries = null;
                slot.gpuVersion = null;
            }
        }


        //inv.invContent.Remove(inv.invContent[index]);
    }

    public void AddInventorySlot()
    {
        inv.usableSlots += 1;

        RefreshList();
    }
}
