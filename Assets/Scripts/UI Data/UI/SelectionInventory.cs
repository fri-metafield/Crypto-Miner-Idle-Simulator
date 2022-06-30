using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionInventory : MonoBehaviour
{
    //public Brand thisBrand;
    //public GPU thisCard;
    public Brand gpuBrand;
    public GPUModel gpuModel;
    public GPUSeries gpuSeries;
    public GPUVersion gpuVersion;

    public bool isUnlocked;
    public bool isUsable;

    [SerializeField] GameObject lockedObject;

    //public bool isUsed;

    //UI
    //[SerializeField] GameObject infoText;

    [SerializeField] GameObject equipButton;

    [SerializeField] Text itemName;
    [SerializeField] Image currencyLogo;

    [SerializeField] float buyPrice;

    private void Update()
    {
        
        lockedObject.SetActive(!isUnlocked);

        if (isUnlocked)
        {
            equipButton.SetActive(gpuSeries);

            if (gpuSeries)
            {
                itemName.text = GameManager.instance.GetGPUName(gpuModel, gpuSeries, gpuVersion);
                currencyLogo.sprite = GameManager.instance.GetGPUImage(gpuBrand, gpuSeries);
                currencyLogo.color = Color.white;

            }
            else
            {
                itemName.text = "";
                currencyLogo.color = new Color(0, 0, 0, 0);
            }
        }
        else
        {
            itemName.text = "";
            currencyLogo.color = new Color(0, 0, 0, 0);
        }
        
    }

    public void ButtonOnClick()
    {
        if (!isUnlocked) return;

        if (gpuSeries) //inventory has gpu
        {
            if (!GameplayRigSetting.instance.slotNames[GameplayRigSetting.instance.SelectedSlot].isUsable) return;

            if(GameUI.instance.selectedSlot.gpuSeries)// rig slot has gpu > swap
            {
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuBrand = GameUI.instance.selectedSlot.gpuBrand;
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuModel = GameUI.instance.selectedSlot.gpuModel;
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuSeries = GameUI.instance.selectedSlot.gpuSeries;
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuVersion = GameUI.instance.selectedSlot.gpuVersion;

                GameUI.instance.SetGPU(gpuBrand, gpuModel, gpuSeries, gpuVersion);

                gpuBrand = GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuBrand;
                gpuModel = GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuModel;
                gpuSeries = GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuSeries;
                gpuVersion = GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuVersion;

            }
            else //rig has no gpu > place
            {
                GameUI.instance.SetGPU(gpuBrand, gpuModel, gpuSeries, gpuVersion);

                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuBrand = null;
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuModel = null;
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuSeries = null;
                GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuVersion = null;

                ClearSlot();
            }

            
        }
        else // inv has none
        {
            

            GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuBrand = GameUI.instance.selectedSlot.gpuBrand;
            GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuModel = GameUI.instance.selectedSlot.gpuModel;
            GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuSeries = GameUI.instance.selectedSlot.gpuSeries;
            GameplayInventory.instance.inv.invContent[GameplayInventory.instance.inventorySlots.IndexOf(this)].gpuVersion = GameUI.instance.selectedSlot.gpuVersion;

            gpuBrand = GameUI.instance.selectedSlot.gpuBrand;
            gpuModel = GameUI.instance.selectedSlot.gpuModel;
            gpuSeries = GameUI.instance.selectedSlot.gpuSeries;
            gpuVersion = GameUI.instance.selectedSlot.gpuVersion;

            GameUI.instance.SetGPU(null, null, null, null);
        }

        GameplayMarket.instance.RefreshSlotList();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    void ClearSlot()
    {
        gpuBrand = null;
        gpuModel = null;
        gpuSeries = null;
        gpuVersion = null;
    }

    //public
    public void ButtonBuyInventory()
    {
        if (!GameManager.instance.hasEnoughPremium(buyPrice)) return;

        GameManager.instance.AddPremium(-buyPrice);
        GameplayInventory.instance.AddInventorySlot();
        SaveManager.instance.SaveOfflineProduction();
    }
}
