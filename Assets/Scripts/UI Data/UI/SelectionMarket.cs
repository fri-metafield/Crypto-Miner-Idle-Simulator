using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMarket : MonoBehaviour
{
    [SerializeField] float slotUnlockPrice;
    public bool isUnlocked;
    public bool isAvailable;

    public bool isSell;
    public bool hasStock;

    //[SerializeField] GPU thisGPU;
    public Brand gpuBrand;
    public GPUModel gpuModel;
    public GPUSeries gpuSeries;
    public GPUVersion gpuVersion;

    //UI
    //-screen
    [SerializeField] GameObject blockedScreen;
    [SerializeField] GameObject unlockScreen;
    [SerializeField] GameObject mainScreen;
    [SerializeField] Text slotpriceText;

    //-buttons
    [SerializeField] GameObject buttonBuy;
    [SerializeField] GameObject buttonSoldOut;

    //-text
    [SerializeField] Image thisImage;
    [SerializeField] Text thisName;
    [SerializeField] Text thisPower;
    [SerializeField] Text thisSpeed;
    [SerializeField] Text thisOverclock;

    [SerializeField] Image brandImage;

    float basePrice;
    float baseSpeed;
    float basePower;

    [SerializeField] Text thisPrice;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        blockedScreen.SetActive(!isAvailable);

        if(isAvailable)
        {
            buttonBuy.SetActive(hasStock);
            mainScreen.SetActive(isUnlocked);
            unlockScreen.SetActive(!isUnlocked);
        }
        else
        {
            mainScreen.SetActive(false);
            unlockScreen.SetActive(false);
        }

        if (isUnlocked)
        {
            if(!isSell)
                buttonSoldOut.SetActive(!buttonBuy.activeSelf);

            if (gpuSeries)
            {
                thisImage.sprite = GameManager.instance.GetGPUImage(gpuBrand, gpuSeries);
                thisImage.color = Color.white;

                brandImage.sprite = gpuBrand.itemImage;
                brandImage.color = Color.white;

                thisName.text = $"{gpuBrand.itemName} {GameManager.instance.GetGPUName(gpuModel, gpuSeries, gpuVersion)}";
                thisPower.text = ": " + GameManager.instance.GetGPUPower(gpuBrand, gpuSeries, gpuVersion).ToString();
                thisSpeed.text = ": " + GameManager.instance.GetGPUSpeed(gpuBrand, gpuSeries, gpuVersion).ToString();
                thisOverclock.text = ": " + GameManager.instance.GetGPUOverclock(gpuBrand, gpuSeries, gpuVersion).ToString();

                //test
                basePrice = GameManager.instance.GetGPUBasePrice(gpuSeries);
                baseSpeed = GameManager.instance.GetGPUBaseSpeed(gpuSeries);
                basePower = GameManager.instance.GetGPUBasePower(gpuSeries);

                if(isSell)
                    thisPrice.text = "" + GameManager.instance.ScoreShow((GameManager.instance.GetGPUPrice(gpuSeries, gpuVersion)/2));
                else
                    thisPrice.text = "" + GameManager.instance.ScoreShow(GameManager.instance.GetGPUPrice(gpuSeries, gpuVersion));
            }
            else
            {
                //thisImage.sprite = GameManager.instance.unusedImage;
                thisImage.color = new Color(0, 0, 0, 0);

                brandImage.color = new Color(0, 0, 0, 0);

                thisName.text = "-";
                thisPower.text = "-";
                thisSpeed.text = "-";
                thisOverclock.text = "-";

                thisPrice.text = "-";

                if (isSell)
                    buttonSoldOut.SetActive(false);
            }
        }
        else
        {
            slotpriceText.text = slotUnlockPrice.ToString();
        }

        if(isSell)
        {
            if(gpuSeries)
            {
                hasStock = true;
            }
            else
            {
                hasStock = false;
            }
        }
        
    }

    //UI
    public void ButtonOnClick()
    {
        if (isSell) MarketSell(); else MarketBuy();
    }
    public void ButtonBuyThisSlot()
    {
        if(GameManager.instance.hasEnoughPremium(slotUnlockPrice))
        {
            isUnlocked = true;
            GameplayMarket.instance.BuyNewSlot();
            GameManager.instance.AddPremium(-slotUnlockPrice);
            SaveManager.instance.SaveOfflineProduction();
        }
        
    }

    private void MarketBuy()
    {
        if(GameManager.instance.hasEnoughMoney(GameManager.instance.GetGPUPrice(gpuSeries, gpuVersion)))
        {
            if (GameplayInventory.instance.InventoryHasEnough())
            {
                GameplayInventory.instance.InventoryAddItem(gpuBrand, gpuModel, gpuSeries, gpuVersion);
                hasStock = false;
                GameplayMarket.instance.BuyItem(gpuSeries, gpuVersion);

                GameplayEarner.instance.EarnItem(GameManager.instance.GetGPUName(gpuModel,gpuSeries,gpuVersion), GameManager.instance.GetGPUImage(gpuBrand, gpuSeries));

                SaveManager.instance.SaveOfflineProduction();
            }
            
        }
        
        
    }

    private void MarketSell()
    {
        var item = GameplayInventory.instance.inv.invContent[GameplayMarket.instance.sellList.IndexOf(this)];
        int index = GameplayInventory.instance.inv.invContent.IndexOf(item);
        GameplayInventory.instance.InventoryRemoveItem(index);

        GameplayMarket.instance.SellItem(gpuSeries, gpuVersion);

        ClearSlot();
        SaveManager.instance.SaveOfflineProduction();
    }

    void ClearSlot()
    {
        gpuBrand = null;
        gpuModel = null;
        gpuSeries = null;
        gpuVersion = null;
    }

    //global
    public void SetItemStock(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv)
    {
        gpuBrand = gb;
        gpuModel = gm;
        gpuSeries = gs;
        gpuVersion = gv;


        //var gpuVer = Random.Range(0, gs.cardManager.versionList.Count);
        ///gpu.version = gpu.cardManager.versionList[gpuVer];
        ///
        hasStock = true;
    }

    public void SetSlot(int brandID, int modelID, int seriesID, int versionID)
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
