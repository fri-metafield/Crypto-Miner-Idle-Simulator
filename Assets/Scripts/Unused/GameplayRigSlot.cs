using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayRigSlot : MonoBehaviour
{
    public GameplayRig thisRig;
    public bool isUsable;

    //public GPU currentGPU;
    public Brand gpuBrand;
    public GPUModel gpuModel;
    public GPUSeries gpuSeries;
    public GPUVersion gpuVersion;

    //Objects
    [SerializeField] Text nameText;

    [SerializeField] Image barImage;

    [SerializeField] Sprite grayBar;
    [SerializeField] Sprite greenBar;
    [SerializeField] Sprite redBar;


    private void OnEnable()
    {
        /*
        RefreshSlot();

        if (thisRig)
            transform.name = "Slot " + thisRig.rigSlots.IndexOf(this);
        else
            transform.name = "Unassigned Slot";
        */
    }

    private void Update()
    {
        /*
        activeObject.SetActive(isUsable);
        inactiveObject.SetActive(!isUsable);

        if (isUsable)
        {
            if (gpuSeries)
            {
                slotImage.sprite = GameManager.instance.GetGPUImage(gpuBrand, gpuSeries);
                slotImage.color = Color.white ;

                powerText.text = ": " + GameManager.instance.GetGPUSpeed(gpuBrand, gpuSeries, gpuVersion).ToString() + " MHz";
                wattageText.text = ": " + GameManager.instance.GetGPUPower(gpuBrand, gpuSeries, gpuVersion).ToString() + " W";

                infoText.SetActive(true);
            }
            else
            {
                slotImage.color = new Color(0, 0, 0, 0);

                powerText.text = ": - MHz";
                wattageText.text = ": - W";

                infoText.SetActive(false);
            }

            backgroundImage.sprite = unlockSprite;
        }
        else
        {
            backgroundImage.sprite = lockSprite;
        }
        */

        if(isUsable)
        {
            if(gpuSeries)
            {
                nameText.text = GameManager.instance.GetGPUName(gpuModel, gpuSeries, gpuVersion);
                barImage.sprite = greenBar;
            }
            else
            {
                barImage.sprite = redBar;
            }
        }
        else
        {
            barImage.sprite = grayBar;
        }

    }

    public void RefreshSlot()
    {
        /*
        if (thisRig.rigSlots.Contains(this))
        {
            if (thisRig.rigSlots.IndexOf(this) + 1 <= thisRig.usableSlots)
            {
                isUsable = true;
            }
            else
            {
                isUsable = false;
            }
        }
        else
        {
            isUsable = false;
        }
        */
    }

    //global
    public void SetSlotGPU(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv)
    {
        //currentGPU = g;
        gpuBrand = gb;
        gpuModel = gm;
        gpuSeries = gs;
        gpuVersion = gv;
    }
    public void SetSlotLoad(int brandID, int modelID, int seriesID, int versionID)
    {
        if (seriesID < 0)
        {
            gpuBrand = null;
            gpuModel = null;
            gpuSeries = null;
            gpuVersion = null;
            return;
        }
        gpuBrand = GameplayInventory.instance.inv.allItems.allItems[brandID] as Brand;
        gpuModel = GameplayInventory.instance.inv.allItems.allItems[modelID] as GPUModel;
        gpuSeries = GameplayInventory.instance.inv.allItems.allItems[seriesID] as GPUSeries;
        gpuVersion = GameplayInventory.instance.inv.allItems.allItems[versionID] as GPUVersion;

    }
    
}
