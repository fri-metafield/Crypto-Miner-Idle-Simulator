using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayRigManager : MonoBehaviour
{
    public int areaLevel = 1;

    public List<GameplayRig> allRigs = new List<GameplayRig>();
    public int unlockedRigs = 0;

    public float totalRigsEarnTime;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameplayRig rigs in allRigs)
        {
            var setPrice = 0f;
            if (areaLevel == 1)
            {
                if (allRigs.IndexOf(rigs) == 0)
                    setPrice = 0;
                else
                    setPrice = Mathf.Pow((50 * (allRigs.IndexOf(rigs) + 1)), areaLevel);
            }
            else
            {
                setPrice = Mathf.Pow((50 * (allRigs.IndexOf(rigs) + 1)), areaLevel);
            }

            rigs.InitializeInfo(this, setPrice);
        }

        
    }

    private void Start()
    {
        RefreshRigsList();
    }

    public void RefreshRigsList()
    {
        totalRigsEarnTime = 0;

        foreach(GameplayRig rigs in allRigs)
        {
            float rigToEarn = rigs.curEarnPower / rigs.currentCurrency.currencyEarnTime;
            totalRigsEarnTime += rigToEarn;

            if (GameManager.instance.hasEnoughMoney(rigs.upgradePrice) && GameManager.instance.hasEnoughMoney(rigs.unlockPrice))
            {
                rigs.rigUpgradeButton.interactable = true;
                rigs.rigUnlockButton.interactable = true;
            }
            else
            {
                rigs.rigUpgradeButton.interactable = false;
                rigs.rigUnlockButton.interactable = false;
            }

            if(allRigs.IndexOf(rigs) <= unlockedRigs)
            {
                rigs.isAvailable = true;
            }
            else
            {
                rigs.isAvailable = false;
            }

        }

        GameUI.instance.GameUIRefreshRigs();

        //SaveManager.instance.SaveOfflineProduction();
    }
}
