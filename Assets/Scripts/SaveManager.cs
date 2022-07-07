using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //inventory
    [SerializeField] Inventory inventory;

    //offlin manager
    [SerializeField] DateTime currentTime;
    [SerializeField] DateTime oldTime;

    [SerializeField] int maxDays;

    //save data
    [SerializeField] string saveRackName;
    [SerializeField] string saveRackUnlock;
    [SerializeField] string saveRackLevel;
    [SerializeField] string saveRackSlot;
    [SerializeField] string saveRigManagerName;
    [SerializeField] string saveInventoryName;
    [SerializeField] string saveMarketName;


    //starter
    public Brand starterBrand;
    public GPUModel starterModel;
    public GPUSeries starterSeries;
    public GPUVersion starterVersion;

    //debug
    [SerializeField] bool keepResetting;

    public static SaveManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (keepResetting) FullReset();
        LoadOfflineProduction();

        if (!PlayerPrefs.HasKey("Last_Login")) FullReset();
    }

    public void LoadOfflineProduction()
    {
        GameplayInventory.instance.inv.allItems.SetItemID();
        currentTime = DateTime.Now;

        if (PlayerPrefs.HasKey("Last_Login"))
        {
            //Player Dta

            PlayerData.player_Money = PlayerPrefs.GetFloat("PlayerMoney", 0);
            PlayerData.player_Coin = PlayerPrefs.GetFloat("PlayerCoin", 0);
            PlayerData.player_Premium = PlayerPrefs.GetFloat("PlayerPremium", 0);

            //Offline Gain

            oldTime = DateTime.Parse(PlayerPrefs.GetString("Last_Login"));

            TimeSpan ts = currentTime - oldTime;

            //max days
            if (ts.TotalDays > maxDays)
            {
                GameUI.instance.LoadOffline(maxDays, 0,0,0, (int)ts.TotalSeconds);
            }
            else
            {
                GameUI.instance.LoadOffline(ts.Days, ts.Hours, ts.Minutes, ts.Seconds, (int)ts.TotalSeconds);
            }
            //no mins
            if(ts.TotalSeconds < 0)
            {
                GameUI.instance.LoadOffline(0, 0, 0, 0, (int)ts.TotalSeconds);
            }

            //counter
            if(currentTime.Date != oldTime.Date) // NEW DAY
            {
                GameManager.instance.RefreshCounter();
            }

            //rigs
            foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
            {
                //unlocks
                gmr.unlockedRigs = PlayerPrefs.GetInt("Area" + gmr.areaLevel + saveRigManagerName + GameUI.instance.rigManagers.IndexOf(gmr), 0);

                foreach (GameplayRig rig in gmr.allRigs)
                {
                    var name = "";
                    name += saveRackName + gmr.allRigs.IndexOf(rig)+"Area"+GameUI.instance.rigManagers.IndexOf(gmr);

                    //unlock
                    bool unlock = PlayerPrefs.GetInt(name + saveRackUnlock) == 1? true : false;
                    if(unlock)
                    {
                        rig.SendMessage("BuyRig", true, SendMessageOptions.DontRequireReceiver);

                        //currency
                        rig.currentCurrency = GameplayInventory.instance.inv.allItems.allCurrencies[PlayerPrefs.GetInt(name + "Coin")];

                        //level
                        while (rig.currentLevel < PlayerPrefs.GetInt(name + saveRackLevel, 0))
                        {
                            rig.SendMessage("UpgradeRig", true, SendMessageOptions.DontRequireReceiver);
                        }

                        


                        //slots
                        foreach (RigSlotTemp slot in rig.rigSlots2)
                        {
                            slot.SetSlotLoad(
                                PlayerPrefs.GetInt(name + saveRackSlot + "Brand" + rig.rigSlots2.IndexOf(slot), -1),
                                PlayerPrefs.GetInt(name + saveRackSlot + "Model" + rig.rigSlots2.IndexOf(slot), -1),
                                PlayerPrefs.GetInt(name + saveRackSlot + "Series" + rig.rigSlots2.IndexOf(slot), -1),
                                PlayerPrefs.GetInt(name + saveRackSlot + "Version" + rig.rigSlots2.IndexOf(slot), -1)
                                );
                        }
                        
                        rig.ResetRig();

                        //rate
                        rig.curEarnRate = PlayerPrefs.GetFloat(name + "Rate", 0);
                    }
                }
            }

            //areas
            GameUI.instance.area2Unlocked = PlayerPrefs.GetInt("Area2Unlocked", 0) == 1 ? true : false; 
            GameUI.instance.area3Unlocked = PlayerPrefs.GetInt("Area3Unlocked", 0) == 1 ? true : false;

            //inventory
            GameplayInventory.instance.inv.usableSlots = PlayerPrefs.GetInt("InventoryUsable", GameplayInventory.instance.inv.startingSlot);
            GameplayInventory.instance.RefreshList();

            var inv = GameplayInventory.instance.inv.invContent;
            foreach (InventoryContent content in inv)
            {
                GameplayInventory.instance.inv.SetSlot(GameplayInventory.instance.inv.invContent.IndexOf(content),
                    PlayerPrefs.GetInt(saveInventoryName + "Brand" + inv.IndexOf(content),-1),
                    PlayerPrefs.GetInt(saveInventoryName + "Model" + inv.IndexOf(content), -1),
                    PlayerPrefs.GetInt(saveInventoryName + "Series" + inv.IndexOf(content), -1),
                    PlayerPrefs.GetInt(saveInventoryName + "Version" + inv.IndexOf(content), -1));
            }

            GameplayInventory.instance.RefreshList();

            //market
            float marketTime = PlayerPrefs.GetFloat("MarketRefresh") - (float)ts.TotalSeconds;
            GameplayMarket.instance.refreshTimer = marketTime;

            GameplayMarket.instance.canRefresh = (PlayerPrefs.GetInt("MarketCanRefresh", 1) == 1 ? true : false);

            GameplayMarket.instance.boughtSmallPack = PlayerPrefs.GetInt("SmallBought", 0) == 1 ? true : false;
            GameplayMarket.instance.boughtVIPPack = PlayerPrefs.GetInt("VIPBought", 0) == 1 ? true : false;
       

            //GameplayMarket.instance.availableMarketSlot = PlayerPrefs.GetInt("MarketAvailable");
            while (GameplayMarket.instance.availableMarketSlot < PlayerPrefs.GetInt("MarketAvailable", 2))
            {
                GameplayMarket.instance.BuyNewSlot();
            }

            foreach (SelectionMarket slot in GameplayMarket.instance.buyList)
            {
                slot.hasStock = PlayerPrefs.GetInt(saveMarketName + "Bool" + GameplayMarket.instance.buyList.IndexOf(slot), 0) == 1 ? true : false;

                slot.SetSlot(
                    PlayerPrefs.GetInt(saveMarketName + "Brand" + GameplayMarket.instance.buyList.IndexOf(slot), -1),
                    PlayerPrefs.GetInt(saveMarketName + "Model" + GameplayMarket.instance.buyList.IndexOf(slot), -1),
                    PlayerPrefs.GetInt(saveMarketName + "Series" + GameplayMarket.instance.buyList.IndexOf(slot), -1),
                    PlayerPrefs.GetInt(saveMarketName + "Version" + GameplayMarket.instance.buyList.IndexOf(slot), -1));

                //Debug.Log("Loaded " + saveMarketName + "Brand" + GameplayMarket.instance.buyList.IndexOf(slot));
            }

            //exchange

            GameplayExchange.instance.ChangeRate(-1, false);
            GameplayExchange.instance.ChangeRate(PlayerPrefs.GetFloat("ExchangeRate", GameplayExchange.instance.moneyRate), false);
            GameplayExchange.instance.soldBTC = PlayerPrefs.GetFloat("ExchangeSoldBTC", 0);

            float exchangeTime = PlayerPrefs.GetFloat("ExchangeRefresh") - (float)ts.TotalSeconds;
            GameplayExchange.instance.curExchangeTime = exchangeTime;


            GameplayExchange.instance.canRefresh = PlayerPrefs.GetInt("ExchangeButton", 1) == 1? true:false;

            //staking


            if (PlayerPrefs.GetInt("Staking") == 1)
            {
                GameplayStaking.instance.StartStake(true);

                GameplayStaking.instance.stakingMoney = PlayerPrefs.GetFloat("StakeMoney");
            }

            float stakeTimer = PlayerPrefs.GetFloat("StakingTimer") - (float)ts.TotalSeconds;
            GameplayStaking.instance.curStakeTimer = stakeTimer;

            GameplayStaking.instance.moneyToAdd = PlayerPrefs.GetFloat("StakeMoneyAdd");


            Debug.Log("Loaded from " + oldTime);

            //boost
            GameplayBooster.instance.isBoostShop = PlayerPrefs.GetInt("BoostBool",0) == 1 ? true : false ;
            GameplayBooster.instance.boostTimer = (PlayerPrefs.GetFloat("BoostTimer", 0) - (float)ts.TotalSeconds);

            //counter
            GameManager.instance.stakeCounter = PlayerPrefs.GetInt("CounterStake", GameManager.instance.stakeCounterMax);
            GameManager.instance.marketRefreshCounter = PlayerPrefs.GetInt("CounterMarketRefresh", GameManager.instance.marketRefreshCounterMax);
            GameManager.instance.exchangeRateCounter = PlayerPrefs.GetInt("CounterExchange", GameManager.instance.exchangeRateCounterMax);

            //game manager
            GameManager.instance.isVIP  = PlayerPrefs.GetInt("VIP", 0) == 1? true : false;
            GameManager.instance.vipCounter = PlayerPrefs.GetFloat("VIPTime", 0);
            GameManager.instance.isAdsDisabled = PlayerPrefs.GetInt("Ads", 0) == 1 ? true : false;
        }
        else
        {
            GameplayTutorial.instance.StartTutorial();
        }
    }


    public void SaveOfflineProduction()
    {
        GameplayInventory.instance.inv.allItems.SetItemID();

        //Offline 
        PlayerPrefs.SetString("Last_Login", DateTime.Now.ToString());

        float rigPower = 0;
        foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            rigPower += gmr.totalRigsEarnTime;
        }
        PlayerPrefs.SetFloat("Last_Rigs", rigPower);

        //Player Data
        PlayerPrefs.SetFloat("PlayerMoney",PlayerData.player_Money);
        PlayerPrefs.SetFloat("PlayerCoin", PlayerData.player_Coin);
        PlayerPrefs.SetFloat("PlayerPremium", PlayerData.player_Premium);

        //rigs
        foreach(GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            //unlocks
            PlayerPrefs.SetInt("Area" + gmr.areaLevel + saveRigManagerName + GameUI.instance.rigManagers.IndexOf(gmr), gmr.unlockedRigs);

            foreach(GameplayRig rig in gmr.allRigs)
            {
                //unlocks
                var name = "";
                name += saveRackName + gmr.allRigs.IndexOf(rig) + "Area" + GameUI.instance.rigManagers.IndexOf(gmr);
                PlayerPrefs.SetInt(name + saveRackUnlock, (rig.isUnlocked ? 1 : 0));

                //currency
                PlayerPrefs.SetInt(name + "Coin", rig.currentCurrency.currencyID);

                //level
                PlayerPrefs.SetInt(name + saveRackLevel, rig.currentLevel);

                //rate
                PlayerPrefs.SetFloat(name + "Rate", rig.curEarnRate);

                
                //slots
                foreach (RigSlotTemp slot in rig.rigSlots2)
                {
                    if (slot.gpuSeries)
                    {
                        PlayerPrefs.SetInt(name + saveRackSlot + "Brand" + rig.rigSlots2.IndexOf(slot), slot.gpuBrand.itemId);
                        PlayerPrefs.SetInt(name + saveRackSlot + "Model" + rig.rigSlots2.IndexOf(slot), slot.gpuModel.itemId);
                        PlayerPrefs.SetInt(name + saveRackSlot + "Series" + rig.rigSlots2.IndexOf(slot), slot.gpuSeries.itemId);
                        PlayerPrefs.SetInt(name + saveRackSlot + "Version" + rig.rigSlots2.IndexOf(slot), slot.gpuVersion.itemId);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(name + saveRackSlot + "Brand" + rig.rigSlots2.IndexOf(slot), -1);
                        PlayerPrefs.SetInt(name + saveRackSlot + "Model" + rig.rigSlots2.IndexOf(slot), -1);
                        PlayerPrefs.SetInt(name + saveRackSlot + "Series" + rig.rigSlots2.IndexOf(slot), -1);
                        PlayerPrefs.SetInt(name + saveRackSlot + "Version" + rig.rigSlots2.IndexOf(slot), -1);
                    }
                }
                
                
            }
        }

        //areas
        PlayerPrefs.SetInt("Area2Unlocked", GameUI.instance.area2Unlocked ? 1 : 0);
        PlayerPrefs.SetInt("Area3Unlocked", GameUI.instance.area3Unlocked ? 1 : 0);

        var inv = GameplayInventory.instance.inv.invContent;

        //inventory

        PlayerPrefs.SetInt("InventoryUsable", GameplayInventory.instance.inv.usableSlots);

        foreach (InventoryContent content in inv)
        {
            if(content.gpuSeries)
            {
                PlayerPrefs.SetInt(saveInventoryName + "Brand" + inv.IndexOf(content), content.gpuBrand.itemId);
                PlayerPrefs.SetInt(saveInventoryName + "Model" + inv.IndexOf(content), content.gpuModel.itemId);
                PlayerPrefs.SetInt(saveInventoryName + "Series" + inv.IndexOf(content), content.gpuSeries.itemId);
                PlayerPrefs.SetInt(saveInventoryName + "Version" + inv.IndexOf(content), content.gpuVersion.itemId);
            }
            else
            {
                PlayerPrefs.SetInt(saveInventoryName + "Brand" + inv.IndexOf(content), -1);
                PlayerPrefs.SetInt(saveInventoryName + "Model" + inv.IndexOf(content), -1);
                PlayerPrefs.SetInt(saveInventoryName + "Series" + inv.IndexOf(content), -1);
                PlayerPrefs.SetInt(saveInventoryName + "Version" + inv.IndexOf(content), -1);
            }
            
        }

        //market
        PlayerPrefs.SetFloat("MarketRefresh", GameplayMarket.instance.refreshTimer);
        PlayerPrefs.SetInt("MarketCanRefresh", GameplayMarket.instance.canRefresh ? 1 : 0);

        PlayerPrefs.SetInt("MarketAvailable", GameplayMarket.instance.availableMarketSlot);

        PlayerPrefs.SetInt("SmallBought", GameplayMarket.instance.boughtSmallPack ? 1 : 0);
        PlayerPrefs.SetInt("VIPBought", GameplayMarket.instance.boughtVIPPack ? 1 : 0);

        foreach (SelectionMarket slot in GameplayMarket.instance.buyList)
        {
            if (slot.gpuSeries)
            {
                PlayerPrefs.SetInt(saveMarketName + "Bool" + GameplayMarket.instance.buyList.IndexOf(slot), (slot.hasStock ? 1 : 0));

                PlayerPrefs.SetInt(saveMarketName + "Brand" + GameplayMarket.instance.buyList.IndexOf(slot), slot.gpuBrand.itemId);
                PlayerPrefs.SetInt(saveMarketName + "Model" + GameplayMarket.instance.buyList.IndexOf(slot), slot.gpuModel.itemId);
                PlayerPrefs.SetInt(saveMarketName + "Series" + GameplayMarket.instance.buyList.IndexOf(slot), slot.gpuSeries.itemId);
                PlayerPrefs.SetInt(saveMarketName + "Version" + GameplayMarket.instance.buyList.IndexOf(slot), slot.gpuVersion.itemId);

                //Debug.Log("Saved " + saveMarketName + "Brand" + GameplayMarket.instance.buyList.IndexOf(slot));
            }
        }

        //exchange
        PlayerPrefs.SetFloat("ExchangeRefresh", GameplayExchange.instance.curExchangeTime);

        PlayerPrefs.SetFloat("ExchangeRate", GameplayExchange.instance.moneyRate);

        PlayerPrefs.SetInt("ExchangeButton", GameplayExchange.instance.canRefresh? 1 : 0);

        PlayerPrefs.SetFloat("ExchangeSoldBTC", GameplayExchange.instance.soldBTC);

        //staking
        PlayerPrefs.SetFloat("StakingTimer", GameplayStaking.instance.curStakeTimer);
        PlayerPrefs.SetInt("Staking", GameplayStaking.instance.isStaking? 1 : 0);
        PlayerPrefs.SetFloat("StakeMoney", GameplayStaking.instance.stakingMoney);
        PlayerPrefs.SetFloat("StakeMoneyAdd", GameplayStaking.instance.moneyToAdd);

        //boost
        PlayerPrefs.SetInt("BoostBool", GameplayBooster.instance.isBoostShop? 1 : 0);
        PlayerPrefs.SetFloat("BoostTimer", GameplayBooster.instance.boostTimer);

        //counter
        PlayerPrefs.SetInt("CounterStake", GameManager.instance.stakeCounter);
        PlayerPrefs.SetInt("CounterMarketRefresh", GameManager.instance.marketRefreshCounter);
        PlayerPrefs.SetInt("CounterExchange", GameManager.instance.exchangeRateCounter);

        //game manager
        PlayerPrefs.SetInt("VIP", GameManager.instance.isVIP ? 1 : 0);
        PlayerPrefs.SetFloat("VIPTime", GameManager.instance.vipCounter);
        PlayerPrefs.SetInt("Ads", GameManager.instance.isAdsDisabled ? 1 : 0);


        Debug.Log("Saved at " + currentTime);

    }

    private void OnApplicationQuit()
    {
        if (!keepResetting)
            SaveOfflineProduction();
        else
            FullReset();
    }

    [ContextMenu("Reset Data")]
    public void FullReset()
    {
        //player data
        PlayerPrefs.DeleteAll();
        PlayerData.player_Money = 0;
        PlayerData.player_Coin = 0;
        PlayerData.player_Premium = 0;

        //rigs
        if(GameUI.instance)
        {
            foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
            {
                foreach (GameplayRig rig in gmr.allRigs)
                {
                    rig.isUnlocked = false;

                    //currency
                    rig.currentCurrency = GameplayInventory.instance.inv.allItems.allCurrencies[0];
                }
            }
        }

        //inventory
        inventory.ClearInventory();
        inventory.ResetInventorySlot();

        inventory.invContent[0].gpuBrand = starterBrand;
        inventory.invContent[0].gpuModel = starterModel;
        inventory.invContent[0].gpuSeries = starterSeries;
        inventory.invContent[0].gpuVersion = starterVersion;

        //market
        if (GameplayMarket.instance)
        {
            GameplayMarket.instance.refreshTimer = GameplayMarket.instance.maxRefereshTime;

            GameplayMarket.instance.availableMarketSlot = 2;

            GameplayMarket.instance.boughtSmallPack = false;
            GameplayMarket.instance.boughtVIPPack = false;
        }
            

        //exchange
        if (GameplayExchange.instance)
        {
            GameplayExchange.instance.curExchangeTime = GameplayExchange.instance.exchangeTime;
            GameplayExchange.instance.soldBTC = 0;
        }

        //staking

        //boost
        if (GameplayBooster.instance)
            GameplayBooster.instance.boostTimer = 0;

        //game manager
        if(GameManager.instance)
        {
            GameManager.instance.isVIP = false;
            GameManager.instance.isAdsDisabled = false;
        }

    }


}
