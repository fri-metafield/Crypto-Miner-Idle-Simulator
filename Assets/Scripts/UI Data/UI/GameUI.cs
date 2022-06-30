using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [SerializeField] Text currentMoney;
    [SerializeField] string moneySymbol;

    [SerializeField] Text currentCoin;
    [SerializeField] string coinSymbol;

    [SerializeField] Text currentPremium;
    [SerializeField] string premiumSymbol;


    //rigs
    //[SerializeField] int totalRigsUnlocked;
    public List<GameplayRigManager> rigManagers = new List<GameplayRigManager>();
    public GameplayRig selectedRig;
    public SelectionRigSlot selectedSlot;
    public int allUnlockedRigs;

    //slots

    //UI
    //-start
    [SerializeField] Image startBG;
    [SerializeField] Image startTitle;
    [SerializeField] Text startLoadingText;

    [SerializeField] float titelFadeValue;
    [SerializeField] float loadingTime;
    float loadTime;

    //coin
    public Transform endPosMoney;
    public Transform endPosCoin;
    public Transform endPosPremium;


    //mover
    public int selectedPage = 1;
    public int selectedArea = 1;
    public int selectedSelection = 1;
    public int selectedMarket = 1;

    //area
    [SerializeField] string area2Name;
    [SerializeField] float area2Price;

    [SerializeField] string area3Name;
    [SerializeField] float area3Price;

    [SerializeField] GameObject area2Warning;
    [SerializeField] GameObject area3Warning;

    [SerializeField] GameObject area2Buy;
    [SerializeField] GameObject area3Buy;

    public bool area2Unlocked;
    public bool area3Unlocked;

    [SerializeField] Text area2PlaceText;
    [SerializeField] Text area2PriceText;

    [SerializeField] Text area3PlaceText;
    [SerializeField] Text area3PriceText;

    //offline
    [SerializeField] GameObject offlineMenu;
    [SerializeField] Text timeSpentText;
    [SerializeField] Text coinsEarnedText;
    public float tempMoney;

    [SerializeField] float premiumTriplePrice;

    //ads
    [SerializeField] RectTransform adBar;
    [SerializeField] Vector2 adBarNormalPos;
    [SerializeField] Vector2 adBarDisabledPos;

    //-quit confirm
    [SerializeField] GameObject quitObject;

    //instance
    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedPage = 1;
        selectedArea = 1;

        loadTime = loadingTime;
        animateLoading();
    }

    // Update is called once per frame
    void Update()
    {

        //in game
        //currentMoney.text = moneySymbol + " " + PlayerData.player_Money.ToString();
        //currentCoin.text = coinSymbol + " " + PlayerData.player_Coin.ToString();
        //currentPremium.text = premiumSymbol + " " + PlayerData.player_Premium.ToString();

        currentMoney.text = moneySymbol + " " + GameManager.instance.ScoreShow(PlayerData.player_Money);
        currentCoin.text = coinSymbol + " " + PlayerData.player_Coin.ToString();
        currentPremium.text = premiumSymbol + " " + GameManager.instance.ScoreShow(PlayerData.player_Premium);

        Color curColorA = startTitle.color;
        Color curColorB = startBG.color;
        Color curColorC = startLoadingText.color;

        startTitle.gameObject.SetActive(true);
        startBG.gameObject.SetActive(true);
        

        if (loadTime <= 0)
        {
            if (curColorA.a > 0f)
            {
                curColorA.a -= Time.deltaTime * titelFadeValue;
                startTitle.color = curColorA;
                curColorB.a -= Time.deltaTime * titelFadeValue;
                startBG.color = curColorB;
                curColorC.a -= Time.deltaTime * titelFadeValue;
                startLoadingText.color = curColorC;
            }
            else
            {
                startTitle.gameObject.SetActive(false);
                startBG.gameObject.SetActive(false);
            }

        }
        else
        {
            loadTime -= Time.deltaTime;
        }
        
        area2PlaceText.text = area2Name;
        area2PriceText.text = "$" + GameManager.instance.ScoreShow(area2Price);

        area3PlaceText.text = area3Name;
        area3PriceText.text = "$" + GameManager.instance.ScoreShow(area3Price);

        if(selectedRig)
        {
            selectedSlot = GameplayRigSetting.instance.slotNames[GameplayRigSetting.instance.SelectedSlot];
        }

        if(GameManager.instance.isAdsDisabled)
        {
            var pos = adBar.rect.position;
            adBar.transform.localPosition = adBarDisabledPos;
        }
        else
        {
            adBar.transform.localPosition = adBarNormalPos;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            quitObject.SetActive(true);
        }
    }

    void animateLoading()
    {
        startLoadingText.text = "Loading";

        // TODO: add optional delay when to start
        StartCoroutine("PlayText");
    }
    IEnumerator PlayText()
    {
        foreach (char c in "...")
        {
            startLoadingText.text += c;
            yield return new WaitForSeconds(1f);
        }
        animateLoading();
    }



    //Selection
    public void SelectRig(GameplayRig rig)
    {
        selectedRig = rig;
        GameplayRigSetting.instance.SetRig(rig);
        //ActivateSelection(3);
        //SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    //Buttons

    public void ActivateSelection(int select)
    {
        selectedSelection = select;
        //SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonDoneInventory()
    {
        selectedSelection = 1;
        selectedRig = null;

        GameplayRigSetting.instance.SelectedSlot = 0;
        selectedSlot.SetSlotGPU(null, null, null, null);
        selectedSlot = null;

        GameplayRigSetting.instance.SetRig(null);
        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void SetCoin(Currencies c)
    {
        selectedRig.SendMessage("SetRigCurrency", c, SendMessageOptions.DontRequireReceiver);
        selectedRig.ResetRig();
        selectedRig = null;

        ActivateSelection(1);
        SaveManager.instance.SaveOfflineProduction();

        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void SetGPU(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv) /////////////////////////////////////////////////////////////////////////////////////////////////
    {
        //selectedSlot.SendMessage("SetSlotGPU", g, SendMessageOptions.DontRequireReceiver);
        selectedSlot.SetSlotGPU(gb, gm, gs, gv);
        GameplayRigSetting.instance.SetRigData();

        GameplayRigSetting.instance.thisRig.ResetRig();
        SaveManager.instance.SaveOfflineProduction();
        //ActivateSelection(1);
    }

    //Page
    public void ButtonSetPage(int page)
    {
        selectedPage = page;

        ButtonSetArea(1);
        ActivateSelection(1);
        ButtonSetMarket(1);

        GameUIRefreshRigs();

        //GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    //Area
    public void ButtonSetArea(int area)
    {
        GameUIRefreshRigs();

        if (area == 2)
        {
            if (allUnlockedRigs >= 5)
            {
                if(area2Unlocked)
                {
                    selectedArea = area;
                }
                else
                {
                    area2Buy.SetActive(true);
                }
            }
            else
            {
                area2Warning.SetActive(true);
            }
        }
        else if (area == 3)
        {
            if (allUnlockedRigs >= 13)
            {
                if (area3Unlocked)
                {
                    selectedArea = area;
                }
                else
                {
                    area3Buy.SetActive(true);
                }
            }
            else
            {
                area3Warning.SetActive(true);
            }
        }
        else
        {
            selectedArea = area;
        }
        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonBuyArea(int area)
    {
        if(area == 2)
        {
            if (!GameManager.instance.hasEnoughMoney(area2Price))
                return;
            GameManager.instance.AddMoney(-area2Price);
            area2Unlocked = true;
            area2Buy.SetActive(false);
        }
        else
        {
            if (!GameManager.instance.hasEnoughMoney(area3Price))
                return;
            GameManager.instance.AddMoney(-area3Price);
            area3Unlocked = true;
            area3Buy.SetActive(false);
        }
        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    //
    public void GameUIRefreshRigs()
    {
        allUnlockedRigs = 0;
        foreach (GameplayRigManager gmr in rigManagers)
        {
            foreach (GameplayRig rig in gmr.allRigs)
            {
                if (rig.isUnlocked)
                {
                    allUnlockedRigs++;
                }
            }
        }
    }

    public void ButtonSetMarket(int value)
    {
        selectedMarket = value;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void LoadOffline(int days, int hours, int minutes, int seconds, int totalSeconds)
    {
        offlineMenu.SetActive(true);
        timeSpentText.text = string.Format("{0} Days {1} Hours {2} Minutes {3} Seconds", days, hours, minutes, seconds);

        float earned = PlayerPrefs.GetFloat("Last_Rigs");
        float toAdd = earned * (float)totalSeconds;

        coinsEarnedText.text = toAdd.ToString() + " BTC";
        tempMoney = toAdd;
    }

    public void CollectMoney(int multiplier)
    {
        GameManager.instance.AddCoin(tempMoney * (multiplier == 0? 1 : multiplier));
        tempMoney = 0;
    }
}