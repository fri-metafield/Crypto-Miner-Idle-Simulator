using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayStaking : MonoBehaviour
{
    [SerializeField] float selectedStakeTimer;
    public float curStakeTimer;
    public bool isStaking;

    private float stakeAmount;
    public float moneyToAdd;

    public float stakingMoney;

    string timerOption;

    float stakePercent;
    float totalEarnedPercent;

    //UI
    //-lock
    [SerializeField] GameObject stakeLockUI;
    [SerializeField] Text stakeInfoText;
    [SerializeField] Text stakeTimer;
    [SerializeField] GameObject reduceButton;
    [SerializeField] Text amountInput;

    //reward
    [SerializeField] Button doneButton;
    [SerializeField] Text doneAmountText;

    //button
    [SerializeField] Button buttonStake;
    [SerializeField] Button buttonExchange;
    [SerializeField] Sprite buttonOn;
    [SerializeField] Sprite buttonOff;

    [SerializeField] GameObject notifBuy;
    [SerializeField] Text notifBuyText;

    [SerializeField] GameObject stakePanel;
    [SerializeField] GameObject exchangePanel;

    public static GameplayStaking instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        doneAmountText.text = "0 BTC";
        timerOption = "1d";
        stakePercent = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
        

        if (isStaking)
        {
            stakeLockUI.SetActive(true);
;

            stakeTimer.text = GameManager.instance.GenTimeSpanFromSeconds(curStakeTimer);

            curStakeTimer -= Time.deltaTime;
            if(curStakeTimer <= 0)
            {
                stakeInfoText.text = "Take your reward!";
                reduceButton.SetActive(false);
                StakeDone();
            }
            else
            {
                stakeInfoText.text = "Wait until stacking Complete";
                reduceButton.SetActive(GameManager.instance.stakeCounter > 0 ? true : false);
            }

            amountInput.text = stakingMoney.ToString() + "BTC";
            doneAmountText.text = stakingMoney.ToString() + "BTC";
        }
        else
        {
            stakeLockUI.SetActive(false);

            if (timerOption == "1h")
            {
                selectedStakeTimer = 60 * 60;
                totalEarnedPercent = 5;
            }
            else if (timerOption == "6h")
            {
                selectedStakeTimer = 60 * 60 * 6;
                totalEarnedPercent = 10;
            }
            else if (timerOption == "12h")
            {
                selectedStakeTimer = 60 * 60 * 12;
                totalEarnedPercent = 20;
            }
            else if (timerOption == "1d")
            {
                selectedStakeTimer = 60 * 60 * 24;
                totalEarnedPercent = 50;
            }

            stakeAmount = PlayerData.player_Coin * (stakePercent / 100);
            moneyToAdd = stakeAmount + (totalEarnedPercent * stakeAmount / 100);

            amountInput.text = stakeAmount.ToString() + "BTC";
            doneAmountText.text = moneyToAdd.ToString() + "BTC";
        }


        //notif
        if (GameplayExchange.instance.soldBTC < 1.1f)
        {
            notifBuyText.text = GameplayExchange.instance.soldBTC + "BTC / 5 BTC";
        }
        //selling
        if (exchangePanel.activeSelf)
        {
            buttonStake.image.sprite = buttonOff;
            buttonExchange.image.sprite = buttonOn;
        }
        else
        {
            buttonStake.image.sprite = buttonOn;
            buttonExchange.image.sprite = buttonOff;
        }
    }

    public void ChooseType(bool a) // exchange
    {
        if (a == false)
        {
            if (GameplayExchange.instance.soldBTC >= 5)
            {
                stakePanel.SetActive(true);
                exchangePanel.SetActive(false);
            }
            else
            {
                notifBuy.SetActive(true);
            }
        }
        else
        {
            stakePanel.SetActive(false);
            exchangePanel.SetActive(true);
        }

        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    private void StakeDone()
    {
        curStakeTimer = 0;
        doneButton.interactable = true;
        SaveManager.instance.SaveOfflineProduction();
    }

    public void StartStake(bool isLoaded)
    {
        if (PlayerData.player_Coin <= 0) return;

        if (!GameManager.instance.hasEnoughCoin(stakeAmount))
            return;
        
        curStakeTimer = selectedStakeTimer;
        stakingMoney = moneyToAdd;

        if(!isLoaded)
            GameManager.instance.AddCoin(-stakeAmount);

        isStaking = true;
    }

    private void Collected()
    {
        isStaking = false;
        
        //VIP
        GameManager.instance.AddCoin(GameManager.instance.isVIP? (moneyToAdd*2) : moneyToAdd);
        stakingMoney = 0;
        moneyToAdd = 0;

        timerOption = "1d";
        stakePercent = 100;

        doneButton.interactable = false;
        GameplayEarner.instance.EarnItem((GameManager.instance.isVIP ? (moneyToAdd * 2) : moneyToAdd) + " BTC", GameManager.instance.coinImage);
        SaveManager.instance.SaveOfflineProduction();
        //doneAmountText.text = "0 BTC";
    }

    //UI

    public void ButtonSelectTime(string timer)
    {
        timerOption = timer;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonSetPercent (float amount)
    {
        stakePercent = amount;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonStartStake()
    {
        StartStake(false);
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonCollect()
    {
        Collected();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    public void ButtonReduceStakeTime(float reduceTime)
    {
        if (GameManager.instance.stakeCounter <= 0)
        {
            if(!GameManager.instance.isVIP)
                return;
        }
        //watch ad
        curStakeTimer -= reduceTime;

        //VIP
        if(!GameManager.instance.isVIP)
            GameManager.instance.stakeCounter--;

        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    //debug
    [ContextMenu("Finish")]
    private void Finish()
    {
        StakeDone();
    }

}
