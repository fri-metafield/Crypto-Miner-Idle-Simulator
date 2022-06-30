using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayMarket : MonoBehaviour
{
    public int availableMarketSlot;
    int currentAvailableSeriesOrder;

    //GPUS
    [SerializeField] List<GPUManager> availableGPUManagers = new List<GPUManager>();
    [SerializeField] List<Brand> availableGPUBrands = new List<Brand>();
    [SerializeField] List<GPUModel> availableGPUModels = new List<GPUModel>();
    [SerializeField] List<GPUSeries> availableGPUSeries = new List<GPUSeries>();
    [SerializeField] List<GPUVersion> availableGPUVersions = new List<GPUVersion>();

    public List<SelectionMarket> buyList = new List<SelectionMarket>();
    public List<SelectionMarket> sellList = new List<SelectionMarket>();

    //Timer
    public float maxRefereshTime;
    public float refreshTimer;

    [SerializeField] Button refreshButton;
    public bool canRefresh;

    //UI
    [SerializeField] Text refreshTimeText;

    //-ads
    [SerializeField] Button adButton;
    [SerializeField] Button bundleSmallButton;
    [SerializeField] Button bundleVIPButton;

    public bool boughtSmallPack;
    public bool boughtVIPPack;

    //-IAP TITAN V
    [SerializeField] Brand titanBrand;
    [SerializeField] GPUModel titanModel;
    [SerializeField] GPUSeries titanSeries;
    [SerializeField] GPUVersion titanVersion;

    public static GameplayMarket instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Last_Login") == false)
        {
            canRefresh = true;
            availableMarketSlot = 2;
            RefreshBuyStock();
        }
            
        foreach (SelectionMarket sm in buyList)
        {
            if(buyList.IndexOf(sm) <= availableMarketSlot)
            {
                sm.isAvailable = true;
                sm.isUnlocked = true;
            }
        }
        foreach (SelectionMarket slot in sellList)
        {
            slot.isSell = true;

            slot.isUnlocked = true;
            slot.isAvailable = true;
        }
        RefreshSlotList();

        if (GameManager.instance.isAdsDisabled) DisableAds();
    }

    // Update is called once per frame
    void Update()
    {
        int minutes = Mathf.FloorToInt(refreshTimer / 60F);
        int seconds = Mathf.FloorToInt(refreshTimer - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        refreshTimeText.text = "Refresh: " + niceTime.ToString();

        refreshTimer -= Time.deltaTime;
        if(refreshTimer <= 0)
        {
            RefreshBuyStock();

            if (!canRefresh) canRefresh = true;
        }

        refreshButton.gameObject.SetActive(canRefresh);

        if(boughtVIPPack)
        {
            bundleVIPButton.interactable = false;
            bundleVIPButton.transform.Find("Text").GetComponent<Text>().text = "Purchased";
        }
        if (boughtSmallPack)
        {
            bundleSmallButton.interactable = false;
            bundleSmallButton.transform.Find("Text").GetComponent<Text>().text = "Purchased";
        }
        //sell
    }

    [ContextMenu("ReduceRefresh")]
    void ReduceRefreshTime()
    {
        refreshTimer = 3;
    }

    [ContextMenu("Refresh Market")]
    void RefreshBuyStock()
    {
        refreshTimer = maxRefereshTime;

        float rigs = 0;
        foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            foreach (GameplayRig rig in gmr.allRigs)
            {
                if (rig.isUnlocked)
                {
                    rigs++;
                }
            }
        }
        if (rigs >= 5 && rigs < 13)
        {
            currentAvailableSeriesOrder = 2;
        }
        else if (rigs >= 13)
        {
            currentAvailableSeriesOrder = 3;
        }
        else
        {
            currentAvailableSeriesOrder = 1;
        }


        foreach (SelectionMarket slot in buyList)
        {
            var indexMarket = Random.Range(0, availableGPUBrands.Count);
            var indexModel = Random.Range(0, availableGPUModels.Count);
            var indexSeries = GetSeries();// Random.Range(0, availableGPUSeries.Count);
            var indexVersion = Random.Range(0, availableGPUVersions.Count);

            slot.SetItemStock(availableGPUBrands[indexMarket], availableGPUModels[indexModel], availableGPUSeries[indexSeries], availableGPUVersions[indexVersion]);
        }
    }

    private int GetSeries()
    {
        var indexSeries = Random.Range(0, availableGPUSeries.Count);

        while(availableGPUSeries[indexSeries].cardMarketUnlockOrder > currentAvailableSeriesOrder)
        {
            indexSeries = Random.Range(0, availableGPUSeries.Count);
        }

        return indexSeries;
    }

    void RefreshSellStock()
    {
        foreach(SelectionMarket slot in sellList)
        {
            slot.gpuBrand = GameplayInventory.instance.inv.invContent[sellList.IndexOf(slot)].gpuBrand;
            slot.gpuModel = GameplayInventory.instance.inv.invContent[sellList.IndexOf(slot)].gpuModel;
            slot.gpuSeries = GameplayInventory.instance.inv.invContent[sellList.IndexOf(slot)].gpuSeries;
            slot.gpuVersion = GameplayInventory.instance.inv.invContent[sellList.IndexOf(slot)].gpuVersion;
        }
    }

    public void BuyNewSlot()
    {
        availableMarketSlot++;
        RefreshSlotList();
    }

    public void RefreshSlotList()
    {
        foreach (SelectionMarket sm in buyList)
        {
            if (buyList.IndexOf(sm) == availableMarketSlot+1)
            {
                sm.isAvailable = true;
            }
        }

        RefreshSellStock();
    }

    public void BuyItem(GPUSeries gs, GPUVersion gv)
    {
        var price = GameManager.instance.GetGPUPrice(gs, gv);
        GameManager.instance.AddMoney(-price);

        GameplayInventory.instance.RefreshList();
        RefreshSlotList();
        //SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void SellItem(GPUSeries gs, GPUVersion gv)
    {
        var price = GameManager.instance.GetGPUPrice(gs, gv);
        var percent = price * (float)50 / 100;
        GameManager.instance.AddMoney(percent);

        GameplayInventory.instance.RefreshList();
        RefreshSlotList();
        //SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    void AddGPURandom()
    {
        int randomBrand = Random.Range(0, availableGPUBrands.Count);
        int randomModel = Random.Range(0, availableGPUModels.Count);
        int randomSeries = Random.Range(0, availableGPUSeries.Count);
        int randomVersion = Random.Range(0, availableGPUVersions.Count);

        GameplayInventory.instance.InventoryAddItem(availableGPUBrands[randomBrand], availableGPUModels[randomModel], availableGPUSeries[randomSeries], availableGPUVersions[randomVersion]);
        GameplayEarner.instance.EarnItem(GameManager.instance.GetGPUName(availableGPUModels[randomModel], availableGPUSeries[randomSeries], availableGPUVersions[randomVersion]), GameManager.instance.GetGPUImage(availableGPUBrands[randomBrand], availableGPUSeries[randomSeries]));
    }
    void AddGPU(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv)
    {
        GameplayInventory.instance.InventoryAddItem(gb, gm, gs, gv);
        GameplayEarner.instance.EarnItem(GameManager.instance.GetGPUName(gm,gs,gv), GameManager.instance.GetGPUImage(gb,gs));
    }
    void DisableAds()
    {
        adButton.interactable = false;
        adButton.transform.Find("Text").GetComponent<Text>().text = "Purchased";

        GameManager.instance.DisableAds();
        SaveManager.instance.SaveOfflineProduction();
    }

    //Button
    public void ButtonClickRefreshMarket()
    {
        if (GameManager.instance.marketRefreshCounter <= 0)
        {
            if (!GameManager.instance.isVIP)
                return;
        }
        //watch ad
        RefreshBuyStock();
        canRefresh = false;

        if(!GameManager.instance.isVIP)
            GameManager.instance.marketRefreshCounter--;

        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonClickDiamond(float amount)
    {
        GameManager.instance.AddPremium(amount);

        GameplayEarner.instance.EarnItem(amount + " Diamonds", GameManager.instance.diamondImage);

        SaveManager.instance.SaveOfflineProduction();
        Debug.Log("Bought Premium Currency");
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonClickBooster(float timer)
    {
        GameplayBooster.instance.ShopBooster(timer);

        GameplayEarner.instance.EarnItem("Auto overclock on", GameManager.instance.boosterImage);

        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonClickMystery(int spaceNeeded)
    {
        var spaceFree = 0;
        var added = spaceNeeded;

        foreach (InventoryContent inv in GameplayInventory.instance.inv.invContent)
        {
            if (!inv.gpuSeries)
            {
                if (GameplayInventory.instance.inventorySlots[GameplayInventory.instance.inv.invContent.IndexOf(inv)].isUnlocked)
                {
                    if (!GameplayInventory.instance.inventorySlots[GameplayInventory.instance.inv.invContent.IndexOf(inv)].gpuSeries)
                    {
                        spaceFree++;
                    }
                }
            }
        }

        if (spaceFree >= spaceNeeded)
        {
            while(added > 0)
            {
                AddGPURandom();
                added--;
            }
        }
        else
        {
            GameplayInventory.instance.invFullPopup.SetActive(true);
            return;
        }

        Debug.Log("Bought Mystery");
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    

    public void ButtonClickCard()
    {
        AddGPU(titanBrand, titanModel, titanSeries, titanVersion);
        SaveManager.instance.SaveOfflineProduction();
        Debug.Log("Bought Card");
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    public void ButtonClickNoAds()
    {
        DisableAds();
        SaveManager.instance.SaveOfflineProduction();
        Debug.Log("Bought No Ads");
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonClickBundle(int bundlePack)
    {
        if (bundlePack == 0) //small
        {
            var spaceFree = 0;
            foreach (InventoryContent inv in GameplayInventory.instance.inv.invContent)
            {
                if (!inv.gpuSeries )
                {
                    if(GameplayInventory.instance.inventorySlots[GameplayInventory.instance.inv.invContent.IndexOf(inv)].isUnlocked)
                    {
                        if(!GameplayInventory.instance.inventorySlots[GameplayInventory.instance.inv.invContent.IndexOf(inv)].gpuSeries)
                        {
                            spaceFree++;
                        }
                    }
                }
            }

            if (spaceFree >= 3)
            {

                DisableAds();
                GameplayBooster.instance.ShopBooster(86400 * 3);
                
                

                GameplayEarner.instance.EarnItem("Disable Ads", GameManager.instance.disableImage);
                GameplayEarner.instance.EarnItem("3 Days auto Overclock", GameManager.instance.boosterImage);

                ButtonClickMystery(3);

                GameplayEarner.instance.EarnItem("20 Diamonds", GameManager.instance.diamondImage);

                GameManager.instance.AddPremium(20);
                boughtSmallPack = true;
            }
            else
            {
                GameplayInventory.instance.invFullPopup.SetActive(true);
                return;
            }

            
        }
        else if (bundlePack == 1) //VIP
        {

            var spaceFree = 0;
            foreach (InventoryContent inv in GameplayInventory.instance.inv.invContent)
            {
                if (!inv.gpuSeries)
                {
                    if (GameplayInventory.instance.inventorySlots[GameplayInventory.instance.inv.invContent.IndexOf(inv)].isUnlocked)
                    {
                        if (!GameplayInventory.instance.inventorySlots[GameplayInventory.instance.inv.invContent.IndexOf(inv)].gpuSeries)
                        {
                            spaceFree++;
                        }
                    }
                }
            }

            if (spaceFree >= 5)
            {
                DisableAds();
                GameplayBooster.instance.ShopBooster(86400 * 7);
                GameManager.instance.AddVIP();

                GameplayEarner.instance.EarnItem("Disable Ads", GameManager.instance.disableImage);
                GameplayEarner.instance.EarnItem("7 Days auto Overclock", GameManager.instance.boosterImage);

                GameplayEarner.instance.EarnItem("7 Days unlimited market refresh", GameManager.instance.marketRefreshImage);
                GameplayEarner.instance.EarnItem("7 Days double staking amount", GameManager.instance.doubleStakingImage);
                GameplayEarner.instance.EarnItem("7 Days double rig income", GameManager.instance.doubleRigImage);

                ButtonClickMystery(5);

                GameplayEarner.instance.EarnItem("60 Diamonds", GameManager.instance.diamondImage);

                GameManager.instance.AddPremium(60);
                boughtVIPPack = true;
            }
            else
            {
                GameplayInventory.instance.invFullPopup.SetActive(true);
                return;
            }
            

            
        }
        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
}
