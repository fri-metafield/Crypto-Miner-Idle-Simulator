using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //UI
    //public Sprite unusedImage;

    public Font gameFont;
    public Font gameFont2;

    public int stakeCounter;
    public int marketRefreshCounter;
    public int exchangeRateCounter;

    public int stakeCounterMax;
    public int marketRefreshCounterMax;
    public int exchangeRateCounterMax;

    //VIP
    public bool isVIP;
    [SerializeField] float maxVipCounter;
    public float vipCounter;

    public bool isAdsDisabled;

    //image global
    public Sprite diamondImage;
    public Sprite coinImage;
    public Sprite moneyImage;
    public Sprite boosterImage;
    public Sprite disableImage;

    public Sprite marketRefreshImage;
    public Sprite doubleStakingImage;
    public Sprite doubleRigImage;

    //audio

    [SerializeField] AudioSource BGMusic;
    [SerializeField] float bgmVolumeMultiplier;

    public AudioClip sfxGeneral;
    public AudioClip sfxPopup;
    public AudioClip sfxUpgrade;


    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Last_Login") == false)
            RefreshCounter();

        GameplayInventory.instance.inv.allItems.SetItemID();
    }

    private void Update()
    {
        #region cheats
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddMoney(10000000);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddCoin(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddPremium(100);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveManager.instance.SaveOfflineProduction();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.instance.LoadOfflineProduction();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveManager.instance.FullReset();
        }

#endif
        #endregion

        if (PlayerData.player_Money < 0) PlayerData.player_Money = 0;
        if (PlayerData.player_Coin < 0) PlayerData.player_Coin = 0;
        if (PlayerData.player_Premium < 0) PlayerData.player_Premium = 0;

        //audio
        //AudioListener.volume = (GameplayProfile.instance.audioOn ? 1 : 0);
        //music 
        BGMusic.volume = (GameplayProfile.instance.musicOn ? 1 : 0) * bgmVolumeMultiplier;
        
        if(isVIP)
        {
            vipCounter -= Time.deltaTime;
        }
        if(vipCounter <= 0)
        {
            isVIP = false;
        }

    }
    public void GameQuit()
    {
        SaveManager.instance.SaveOfflineProduction();
        Application.Quit();
    }

    
    public void PlaySound(AudioClip clip, bool stopPrevious)
    {
        if (!GameplayProfile.instance.audioOn) return;

        AudioSource aS = GetComponent<AudioSource>();

        aS.clip = clip;
        aS.volume = 1;
        if (stopPrevious)
        {
            aS.Stop();
            aS.Play();
        }
        else
        {
            aS.PlayOneShot(clip);
        }
       
    }

    public string GenTimeSpanFromSeconds(float seconds)
    {
        // Create a TimeSpan object and TimeSpan string from 
        // a number of seconds.
        TimeSpan interval = TimeSpan.FromSeconds(seconds);
        string timeInterval = interval.ToString();

        // Pad the end of the TimeSpan string with spaces if it 
        // does not contain milliseconds.
        int pIndex = timeInterval.IndexOf(':');
        pIndex = timeInterval.IndexOf('.', pIndex);
        if (pIndex < 0) timeInterval += "        ";

        //string newText = string.Format("{0}", timeInterval);
        string newText = string.Format("{0} : {1} : {2} : {3}", interval.Days, interval.Hours, interval.Minutes, interval.Seconds);
        return newText;

        //Console.WriteLine("{0,21}{1,26}", seconds, timeInterval);
    }

    //counters daily
    [ContextMenu("Refresh Counters")]
    public void RefreshCounter()
    {
        stakeCounter = stakeCounterMax;
        marketRefreshCounter = marketRefreshCounterMax;
        exchangeRateCounter = exchangeRateCounterMax;
    }

    [ContextMenu("Add VIP")]
    public void AddVIP()
    {
        isVIP = true;
        vipCounter = maxVipCounter;
    }

    [ContextMenu("Disable Ad")]
    public void DisableAds()
    {
        isAdsDisabled = true;
    }

    //big data string formatting
    public string ScoreShow(float Score)
    {
        string result;
        string[] ScoreNames = new string[] { "", "k", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Score < 900)
                break;
            else Score = Mathf.Floor(Score / 100f) / 10f;

        if (Score == Mathf.Floor(Score))
            result = Score.ToString() + ScoreNames[i];
        else result = Score.ToString("F1") + ScoreNames[i];
        return result;
    }

    //amount checks
    public bool hasEnoughMoney(float comparePrice)
    {
        if (PlayerData.player_Money >= comparePrice)
        {
            return true;
        }
        Debug.Log("Not enough money");
        return false;
    }

    public bool hasEnoughCoin(float comparePrice)
    {
        if (PlayerData.player_Coin >= comparePrice)
        {
            return true;
        }
        Debug.Log("Not enough money");
        return false;
    }

    public bool hasEnoughPremium(float comparePrice)
    {
        if (PlayerData.player_Premium >= comparePrice)
        {
            return true;
        }
        Debug.Log("Not enough premium");
        return false;
    }
    
    //amount adds
    public void AddMoney(float amount)
    {
        PlayerData.player_Money += amount;
        Debug.Log("Added Money " + amount);

        foreach(GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            gmr.RefreshRigsList();
        }
        
    }
    public void AddCoin(float amount)
    {
        PlayerData.player_Coin += amount;
        Debug.Log("Added Coin " + amount);

        foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            gmr.RefreshRigsList();
        }

    }
    public void AddPremium(float amount)
    {
        PlayerData.player_Premium += amount;
        Debug.Log("Added Premium " + amount);

        foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            gmr.RefreshRigsList();
        }

    }
    

    //GPU

    //-base
    public float GetGPUBasePrice(GPUSeries gs)
    {
        var cs = gs.cardManager.gpuSeriesList.IndexOf(gs) + 1;

        var answer = gs.cardPrice;

        if (cs >= 1)
        {
            for (int i = 0; i < gs.cardSeriesCounter - 1; i++)
            {
                answer *= 2.5f;
            }
        }
        return Mathf.RoundToInt(answer);
    }
    public int GetGPUBasePower(GPUSeries gs)
    {
        var cs = gs.cardSeriesCounter; //gs.cardManager.gpuSeriesList.IndexOf(gs) + 1;
        return (gs.cardPowerIncrease * (cs - 1)) + gs.cardBasePower;
    }
    public int GetGPUBaseSpeed(GPUSeries gs)
    {
        //var cs = gpu.cardManager.gpuList.IndexOf(gpu) + 1;
        return gs.cardBaseSpeed;
        ;
    }

    //-main vars
    public string GetGPUName(GPUModel gm, GPUSeries gs, GPUVersion gv)
    {
        return gm.cardModelName + " " + gs.cardSeries + gv.cardVersion;
    }

    public Sprite GetGPUImage(Brand gb, GPUSeries gs)
    {
        if (gb.itemName == "Colossal")
        {
            return gs.cardImages.ColossalImage;
        }
        if (gb.itemName == "MCI")
        {
            return gs.cardImages.MCIImage;
        }
        if (gb.itemName == "Nfridia")
        {
            return gs.cardImages.NfrididaImage;
        }
        if (gb.itemName == "EFCA")
        {
            return gs.cardImages.EFCAImage;
        }
        if (gb.itemName == "SUSA")
        {
            return gs.cardImages.SUSAImage;
        }
        return null;
    }
    public float GetGPUPrice(GPUSeries gs, GPUVersion gv)
    {
        var cv = gv.cardVersionCounter;
        var answer = GetGPUBasePrice(gs) + (GetGPUBasePrice(gs) * ((float)5 / 100 * (cv - 1)));
        return Mathf.RoundToInt(answer);
    }


    public float GetGPUPower(Brand gb, GPUSeries gs, GPUVersion gv)
    {
        //Base Power+(40*GPU Versi ke-n)+(Bonus Brand)
        var cv = gv.cardVersionCounter;

        float answer = GetGPUBasePower(gs) + (40 * (cv - 1)) + gb.brandPowerBonus;
        return Mathf.RoundToInt(answer);
    }
    public float GetGPUSpeed(Brand gb, GPUSeries gs, GPUVersion gv)
    {
        var cv = gv.cardVersionCounter;

        //(Base Speed+(Bonus Brand)+(Bonus Special Model)) + (8%/20%(khusus model Colossal)  dari Base Speed* versi ke-n)*(Booster)
        //float a = (GetGPUBaseSpeed(gs) + ((float)(gb.brandSpeedBonus / 100)) * ((float)(8 / 100))); //0.08f;
        //float b = (cv - 1);
        //float answer = GetGPUBaseSpeed(gs) + ((a) * (b));
        float a = GetGPUBaseSpeed(gs);
        float b = (((float)(8  * GetGPUBaseSpeed(gs) / 100)) * (cv - 1));
        float answer = (a + b);
        float answerBonus = answer + ((float)(gb.brandSpeedBonus / 100) * answer);
        return Mathf.RoundToInt(answerBonus);

    }
    public float GetGPUOverclock(Brand gb, GPUSeries gs, GPUVersion gv)
    {
        //(Idle Mining Speed*160%)+(Bonus Brand)+(Bonus Special Model)
        float answer = (float)((GetGPUSpeed(gb, gs, gv) * 160) / 100);
        float answerBonus = answer + (float)((gb.brandSpeedOverclockBonus * answer) / 100 );
        return Mathf.RoundToInt(answerBonus);
    }

    

}
