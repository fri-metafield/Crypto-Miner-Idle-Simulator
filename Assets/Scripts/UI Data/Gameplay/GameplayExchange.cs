using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayExchange : MonoBehaviour
{   
    //exchange rate
    [SerializeField] private float maxMoneyRate = 30000;
    [SerializeField] private float minMoneyRate = 10000;
    [SerializeField] public float moneyRate;

    public float exchangeTime;
    public float curExchangeTime;

    [SerializeField] Button refreshButton;
    public bool canRefresh;

    //exchange
    bool isSelling;
    float percentExchangeRateCoin;
    float percentExchangeRateMoney;
    float percentage;

    [SerializeField] float exchangeTax;

    //UI
    [SerializeField] Text moneyRateText;

    [SerializeField] Button exchangeButton;
    [SerializeField] Sprite exchangeYellow;
    [SerializeField] Sprite exchangeGray;

    [SerializeField] Sprite buttonBlue;
    [SerializeField] Sprite buttonDark;

    [SerializeField] Button buttonBuy;
    [SerializeField] Button buttonSell;

    //-exchange
    [SerializeField] Text exchangeAmountText;
    [SerializeField] Text exchangeTotalEarnedText;

    //-time
    [SerializeField] Text refreshTimeText;

    //data
    [SerializeField] Text maxRateText;
    [SerializeField] Text midRateText;
    [SerializeField] Text minRateText;

    //-graph
    [SerializeField] List<float> rateHistory = new List<float>();
    [SerializeField] int rateHistoryMaxCount = 5;
    [SerializeField] GraphLineRenderer graphLines;

    //notif
    [SerializeField] GameObject notifBuy;
    [SerializeField] Text notifBuyText;
    public float soldBTC = 0;

    public static GameplayExchange instance;
    private void Awake()
    {
        instance = this;
        //ChangeRate();
    }

    // Start is called before the first frame update
    void Start()
    {
        percentage = 100;
        isSelling = true;
        

        if (PlayerPrefs.HasKey("Last_Login") == false)
        {
            canRefresh = true;
            ChangeRate(-1, false);
            ChangeRate(-1, false);
            Debug.Log("Exchange from 0");
        }

    }

    // Update is called once per frame
    void Update()
    {
        curExchangeTime -= Time.deltaTime;
        if (curExchangeTime <= 0)
        {
            if (!canRefresh) canRefresh = true;
            ChangeRate(-1, false);
        }

        percentExchangeRateMoney = (PlayerData.player_Money * percentage / 100);
        percentExchangeRateCoin = (PlayerData.player_Coin * percentage / 100);

        refreshButton.gameObject.SetActive(canRefresh);

        moneyRateText.text = "$" + moneyRate.ToString("N0");

        int minutes = Mathf.FloorToInt(curExchangeTime / 60F);
        int seconds = Mathf.FloorToInt(curExchangeTime - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        refreshTimeText.text = "Refresh Exchange: " + niceTime + "... or";

        maxRateText.text = "$" + maxMoneyRate.ToString("N0");
        midRateText.text = "$" + ((minMoneyRate + maxMoneyRate) / 2).ToString("N0");
        minRateText.text = "$" + minMoneyRate.ToString("N0");

        if (exchangeButton.interactable)
        {
            exchangeButton.image.sprite = exchangeYellow;
        }
        else
        {
            exchangeButton.image.sprite = exchangeGray;
        }

        if (isSelling)
        {
            if (percentExchangeRateCoin <= 0)
            {
                exchangeButton.interactable= false;
            }
            else
            {
                exchangeButton.interactable = true;
            }

            exchangeAmountText.text = percentExchangeRateCoin.ToString() + " BTC";

            float totalSold = percentExchangeRateCoin * moneyRate;
            exchangeTotalEarnedText.text = "$ " + GameManager.instance.ScoreShow(totalSold - ((float)(5 / 100)));

        }
        else
        {
            if (percentExchangeRateMoney <= 0)
            {
                exchangeButton.interactable = false;
            }
            else
            {
                exchangeButton.interactable = true;
            }

            exchangeAmountText.text = "$ " + GameManager.instance.ScoreShow(percentExchangeRateMoney);

            float totalSold = percentExchangeRateMoney / moneyRate;
            exchangeTotalEarnedText.text = (totalSold - ((float)(5 / 100))).ToString() + " BTC";
        }

        if(moneyRate <= minMoneyRate)
        {
            ChangeRate(minMoneyRate, false);
        }
        
        //notif
        if(soldBTC < 1.1f)
        {
            notifBuyText.text = soldBTC + "BTC / 1 BTC";
        }

        //selling
        if(isSelling)
        {
            buttonBuy.image.sprite = buttonDark;
            buttonSell.image.sprite = buttonBlue;
        }
        else
        {
            buttonBuy.image.sprite = buttonBlue;
            buttonSell.image.sprite = buttonDark;
        }
    }

    [ContextMenu("Exchange Now")]
    void ExchangeNow()
    {
        ChangeRate(-1, false);
    }

    public void ChangeRate(float setAmount, bool isCooldown)
    {
        if (setAmount != -1)
            moneyRate = setAmount;
        else
        {
            if(isCooldown)
            {
                moneyRate = Random.Range((rateHistory[rateHistory.Count-1]), maxMoneyRate);
            }
            else
            {
                moneyRate = Random.Range(minMoneyRate, maxMoneyRate);
            }
            
        }
            

        if (rateHistory.Count < rateHistoryMaxCount)
        {
            rateHistory.Add(moneyRate);
            //graphLines.points.Add(new Vector2(0, 0));
        }
        else
        {
            rateHistory.RemoveAt(0);
            rateHistory.Add(moneyRate);
        }
        SetLines();
        curExchangeTime = exchangeTime;

        if (isCooldown) canRefresh = false;
    }

    private void SetLines()
    {
        graphLines.points.Clear();
        foreach (float point in rateHistory)
        {
            var offset = -2;
            var offsetY = graphLines.gridSize.y;

            graphLines.points.Add(new Vector2(
                graphLines.grid.gridSize.x / rateHistory.Count * (rateHistory.IndexOf(point) + 1) + offset,
                (point) / (graphLines.grid.gridSize.y * minMoneyRate) * offsetY * 2
                ));
            graphLines.SetVerticesDirty();
        }
    }

    //buttons
    public void ButtonConvertCoins()
    {
        ConvertCoins();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    public void ButtonPercent(float amount)
    {
        percentage = amount;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    public void ButtonSwitchMode(bool a)
    {
        if(a == false)
        {
            if (soldBTC >= 1)
            {
                isSelling = a;
            }
            else
            {
                notifBuy.SetActive(true);
            }
        }
        else
        {
            isSelling = a;
        }
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);

    }
    public void ButtonInstantRate()
    {
        //watch ad
        if (GameManager.instance.exchangeRateCounter <= 0) return;
        ChangeRate(-1, true);
        GameManager.instance.exchangeRateCounter--;

        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    //

    private void ConvertCoins()
    {
        //float buyRate = tempExchangeRate * moneyRate;

        if (!GameManager.instance.hasEnoughMoney(percentExchangeRateMoney)) return;
        if (!GameManager.instance.hasEnoughCoin(percentExchangeRateCoin)) return;

        if (isSelling)
        {
            GameManager.instance.AddMoney(percentExchangeRateCoin * moneyRate);
            GameManager.instance.AddCoin((-percentExchangeRateCoin) - ((float)(5/100)));

            GameplayEarner.instance.EarnItem(GameManager.instance.ScoreShow(percentExchangeRateCoin * moneyRate) + " USDT", GameManager.instance.moneyImage);

            soldBTC += (percentExchangeRateCoin) - ((float)(5 / 100));
        }
        else
        {
            
            GameManager.instance.AddCoin(percentExchangeRateMoney / moneyRate);
            GameManager.instance.AddMoney((-percentExchangeRateMoney) - ((float)(5 / 100)));

            GameplayEarner.instance.EarnItem(GameManager.instance.ScoreShow(percentExchangeRateMoney / moneyRate) + " BTC", GameManager.instance.coinImage);
        }
        
    }


}
