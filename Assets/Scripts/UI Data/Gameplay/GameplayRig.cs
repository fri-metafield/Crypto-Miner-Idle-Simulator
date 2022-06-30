using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayRig : MonoBehaviour
{
    //rig
    public Rigs currentRig;
    public  int currentLevel = 1;
    GameplayRigManager rigManager;
    public float unlockPrice, upgradePrice, upgradePower;
    public int upgradeSlot;
    public float curPrice;
    int upgradeCounter = 1;

    //unlock
    public bool isAvailable;
    public bool isUnlocked;
    [SerializeField] GameObject activeObject, inactiveObject, lockedObject;

    //currency
    public Currencies currentCurrency;

    //power
    public float curPower;
    public float curMaxPower;
    bool isOverload;

    //overclock
    float curOC;
    [SerializeField] float maxOverclock;
    public bool isOverclocking;
    [SerializeField] float overClockEarn;
    public int overclockCycle;
    public int overclockMaxCycle;

    //earn
    float maxEarnTime;
    public float curEarnRate;
    public float curEarnPower;
    bool counting;

    //slots
    //public List<GameplayRigSlot> rigSlots = new List<GameplayRigSlot>();

    public List<RigSlotTemp> rigSlots2 = new List<RigSlotTemp>();

    public int usableSlots;
    public int usedSlots;

    [SerializeField] Transform spawnPos;
    [SerializeField] GameObject spawnCoinPrefab;

    //////////////////////////////UI

    [SerializeField] Image rigImage;
    [SerializeField] Text rigNameText;
    [SerializeField] Image currencyImage;
    [SerializeField] GameObject rigOverloadImage;
    [SerializeField] GameObject rigIdleImage;

    //-unlock
    [SerializeField] Text unlockPriceText;


    //-rate
    [SerializeField] Slider earnRateBar;
    [SerializeField] Text earnTimeText, earnRateText;

    //power
    [SerializeField] Text rigPowerText;

    //overclock
    [SerializeField] Text rigOverclockCount;
    [SerializeField] Slider rigOverclockBar;

    //slots
    [SerializeField] Text rigSlotText;

    //upgrade
    [SerializeField] Text rigUpgradePriceText, rigUpgradePowerText, rigUpgradeSlotText;
    public Button rigUpgradeButton, rigUnlockButton;
    [SerializeField] GameObject rigUpgradeMaxText;

    //BG
    [SerializeField] Image rigBGImage;
    [SerializeField] Sprite rigBGUnlock;
    [SerializeField] Sprite rigBGLock;

    private void Awake()
    {
        //ResetRig();
        //RecountSlots();
    }

    private void Start()
    {
        ResetRig();
    }

    public void InitializeInfo(GameplayRigManager rm, float price)
    {
        rigManager = rm;
        unlockPrice = price;
    }

    private void Update()
    {
        if(isAvailable)
        {
            activeObject.SetActive(isUnlocked);
            inactiveObject.SetActive(!isUnlocked);

            if(isUnlocked)
            {
                rigBGImage.sprite = rigBGUnlock;
            }
            else
            {
                rigBGImage.sprite = rigBGLock;
            }
        }
        else
        {
            activeObject.SetActive(false);
            inactiveObject.SetActive(false);

            rigBGImage.sprite = rigBGLock;
        }
        
        lockedObject.SetActive(!isAvailable);

        rigNameText.text = $"Rig {rigManager.allRigs.IndexOf(this) + 1} | Level {currentLevel}";

        if(isAvailable)
        {
            if (isUnlocked)
            {
                if (counting && !isOverload)
                    curEarnRate -= Time.deltaTime;

                if (curEarnRate <= 0 && !isOverload)
                {
                    curEarnRate = maxEarnTime;
                    MineCoin();
                }

                //boost
                if(GameplayBooster.instance.isBoosting)
                {
                    curOC = maxOverclock;
                    overclockCycle = 99;
                }

                //oveload power
                if (curPower > curMaxPower)
                {
                    isOverload = true;
                }
                else
                {
                    isOverload = false;
                }
                if (isOverload)
                {
                    rigPowerText.color = Color.red;
                    curEarnRate = 0;
                }
                else
                {
                    rigPowerText.color = Color.black;
                }

                rigOverloadImage.SetActive(isOverload);

                //UI
                if(curEarnPower > 0)
                    rigImage.sprite = currentRig.itemImage;
                else
                    rigImage.sprite = currentRig.rigIdle;

                rigIdleImage.SetActive((curEarnPower > 0? false : true));

                rigPowerText.text = $": {curPower} / {curMaxPower} w";
                rigSlotText.text = $": {usedSlots} / {usableSlots}";

                currencyImage.sprite = currentCurrency.currencyImage;

                //earn
                counting = CheckCounting();
                if (counting && !isOverload)
                {
                    earnTimeText.text = curEarnRate.ToString("N0") + " s";
                    earnRateText.text = curEarnPower.ToString();

                    earnRateBar.maxValue = maxEarnTime;
                    earnRateBar.value += Time.deltaTime;
                }

                //overclock
                rigOverclockBar.maxValue = maxOverclock;
                rigOverclockBar.value = curOC;

                rigOverclockCount.text = "x" + overclockCycle.ToString();

                //upgrade
                rigUpgradePriceText.text = "Upgrade $" + GameManager.instance.ScoreShow(upgradePrice);
                rigUpgradePowerText.text = upgradePower.ToString();
                rigUpgradeSlotText.text = upgradeSlot.ToString();

                rigUpgradeMaxText.SetActive(currentLevel >= currentRig.rigMaxLevel ? true : false);
                rigUpgradeButton.gameObject.SetActive(!rigUpgradeMaxText.activeSelf);

            }
            else
            {
                unlockPriceText.text = $"Unlock ${unlockPrice}";
                currentLevel = 0;
            }
        }
        else
        {

        }
        

    }

    


    bool CheckCounting()
    {
        foreach (RigSlotTemp rig in rigSlots2)
        {
            if (rig.gpuSeries) return true;
            
        }
        return false;
    }

    void AddOCCycle()
    {
        overclockCycle = overclockMaxCycle;
        curOC = maxOverclock;
        
    }

    public void ResetCycle()
    {
        overclockCycle = 0;
        curOC = 0;
    }

    void MineCoin()
    {
        if (isOverload) return;

        //UI
        earnRateBar.value = 0;

        if (overclockCycle > 1)
        {
            if(!GameplayBooster.instance.isBoosting)
                overclockCycle -= 1;
            //curOC = 0;
        }
        else if (overclockCycle == 1)
        {
            if (!GameplayBooster.instance.isBoosting)
            {
                overclockCycle -= 1;
                curOC = 0;
            }
        }
        else if(overclockCycle < 1)
        {
            curOC = 0;
        }

        if(GameUI.instance.selectedPage == 1 && GameUI.instance.selectedArea == rigManager.areaLevel)
        {
            var coin = Instantiate(spawnCoinPrefab, spawnPos.transform.position, spawnPos.transform.rotation);
            coin.transform.SetParent(gameObject.transform);
            coin.transform.localScale = Vector3.one;
            coin.GetComponent<CoinMove>().targetPos = GameUI.instance.endPosCoin;
            coin.GetComponent<CoinMove>().moveSpeed = 0.5f;
        }

        //VIP
        GameManager.instance.AddCoin(GameManager.instance.isVIP? (curEarnPower*2) : curEarnPower);
        RecountSlots();
    }

    public void ResetRig()
    {
        maxEarnTime = currentCurrency.currencyEarnTime;

        curEarnRate = maxEarnTime;
        earnRateBar.value = 0;

        overclockCycle = 0;
        isOverclocking = false;
        curOC = 0;

        RecountSlots();
    }

    void RecountSlots()
    {
        if (overclockCycle > 0)
        {
            isOverclocking = true;
        }
        else
        {
            isOverclocking = false;
        }

        var tempAmount = 0f;
        var tempWatt = 0f;
        var curSlot = 0;
        var tempCycle = 0;

        foreach(RigSlotTemp slot in rigSlots2)
        {
            if (rigSlots2.IndexOf(slot) + 1 <= usableSlots)
            {
                slot.isUsable = true;
            }
            else
            {
                slot.isUsable = false;
            }

            //slot.RefreshSlot();
            if (slot.gpuSeries)
            {
                if(isOverclocking)
                {
                    tempAmount += GameManager.instance.GetGPUOverclock(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion); //slot.currentGPU.cardBaseOverclockSpeed;
                }
                else
                {
                    tempAmount += GameManager.instance.GetGPUSpeed(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion);
                }
                tempWatt += GameManager.instance.GetGPUPower(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion);
                curSlot += 1;

                tempCycle += slot.gpuModel.maxOverclockCycle + slot.gpuBrand.brandOverclockCycleBonus;
            }
                
        }

        curEarnPower = currentCurrency.currencyEarnValue * tempAmount;
        curPower = tempWatt;
        usedSlots = curSlot;
        overclockMaxCycle = tempCycle;

        if(rigManager)
            rigManager.RefreshRigsList();

        //Debug.Log("Recount slots");
    }

    //global
    public void SetRigCurrency(Currencies c)
    {
        currentCurrency = c;
        rigManager.RefreshRigsList();
    }
    public void ButtonSelectCoin()
    {
        GameUI.instance.ActivateSelection(2);
        GameUI.instance.SelectRig(this);
    }

    public void ButtonSelectInventory()
    {
        GameUI.instance.ActivateSelection(3);
        GameUI.instance.SelectRig(this);
    }

    //Buttons
    public void ButtonOnClick()
    {
        if(isAvailable)
        {
            if (isUnlocked)
            {
                if (overclockCycle > 0 || GameplayBooster.instance.isBoosting) return;

                curOC += overClockEarn;

                if (curOC >= maxOverclock)
                    AddOCCycle();

                RecountSlots();
            }
            else
            {
                BuyRig(false);
                GameManager.instance.PlaySound(GameManager.instance.sfxUpgrade, false);
            }
        }
        
    }
    
    public void ButtonUpgradeRig()
    {
        if (GameManager.instance.hasEnoughMoney(upgradePrice))
        {
            UpgradeRig(false);
            GameManager.instance.PlaySound(GameManager.instance.sfxUpgrade, false);
        }
    }

    void BuyRig(bool isLoaded)
    {
        if(!isLoaded)
        {
            if (!GameManager.instance.hasEnoughMoney(unlockPrice)) 
                return;

            
        }

        if(!isLoaded)
        {
            if (GameManager.instance.hasEnoughMoney(unlockPrice))
                GameManager.instance.AddMoney(-unlockPrice);
        }
           

        isUnlocked = true;
        currentLevel = 1;


        if (unlockPrice != 0)
            curPrice = unlockPrice;
        else
            curPrice = 50;

        curMaxPower = currentRig.rigPower;
        usableSlots = 1;


        //upgrade
        upgradePrice = curPrice;

        var upAmount = 60 + (60 * currentLevel) + (380 * usableSlots);
        upgradePower = upAmount;
        upgradeSlot = 1;

        if (!isLoaded)
            rigManager.unlockedRigs++;

        rigManager.RefreshRigsList();

    }

    IEnumerator UpgradeRig(bool isUpgraded)
    {
        if(currentLevel >= currentRig.rigMaxLevel)
        {
            return null;
        }
        curPrice = upgradePrice;

        if (!isUpgraded)
            GameManager.instance.AddMoney(-curPrice);

        curMaxPower = upgradePower;
        usableSlots = upgradeSlot;

        currentLevel += 1;

        RecountSlots();
        SetUpgradeData();
        return null;
    }

    void SetUpgradeData()
    {
        //price
        var slotRackPrice = Mathf.Pow((50 * (rigManager.allRigs.IndexOf(this) + 1)), rigManager.areaLevel);
        upgradePrice = slotRackPrice * (currentLevel) * usableSlots;
        //slot rack price * level * jumlah GPU

        //power
        var upAmount = 60 + (60 * currentLevel) + (380 * usableSlots);
        upgradePower = upAmount;

        upgradeCounter++;

        if ((upgradeCounter % 5 == 0))
        {
            if (upgradeSlot < currentRig.rigMaxSlots)
                upgradeSlot = usableSlots + 1;
            else
                upgradeSlot = usableSlots;
        }
        else
        {
            upgradeSlot = usableSlots;
        }

        //Debug.Log(upgradeCounter);
        RecountSlots();

    }

    

   
}
[System.Serializable]
public class RigSlotTemp
{
    public bool isUsable;

    public Brand gpuBrand;
    public GPUModel gpuModel;
    public GPUSeries gpuSeries;
    public GPUVersion gpuVersion;

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