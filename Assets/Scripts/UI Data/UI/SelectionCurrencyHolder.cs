using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCurrencyHolder : MonoBehaviour
{
    public List<SelectionCurrency> currencyUI = new List<SelectionCurrency>();

    public int unlockedCoin = 1;
    [SerializeField] int maxUnlockedCoin = 5;
    public int savedCoin;

    public static SelectionCurrencyHolder instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (unlockedCoin < 0) unlockedCoin = 1;
        if (unlockedCoin > maxUnlockedCoin) unlockedCoin = maxUnlockedCoin;

        if (GameUI.instance.selectedSelection == 2)
        {
            CheckUnlock();

        }

    }

    void CheckUnlock()
    {

        var unlock = 0;
        foreach(SelectionCurrency cur in currencyUI)
        {
            if(SpeedText() > cur.thisCurrency.currencyUnlockSpeed)
            {
                unlock++;
            }
        }

        if(savedCoin < unlock)
        {
            savedCoin = unlock;
            unlockedCoin = savedCoin;
        }
        else
        {
            if (unlockedCoin < savedCoin)
            {
                unlockedCoin = savedCoin;
            }
            else
            {
                savedCoin = unlockedCoin;
            }
        }

        
    }

    public bool HasEnoughSpeed(Currencies thisCurrency)
    {
        float speed = 0;
        foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            foreach (GameplayRig rig in gmr.allRigs)
            {

                foreach (RigSlotTemp slot in rig.rigSlots2)
                {
                    if (slot.gpuSeries)
                    {
                        speed += GameManager.instance.GetGPUSpeed(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion);
                    }

                }
            }
        }

        Debug.Log("speed: " + speed);

        if (speed >= thisCurrency.currencyUnlockSpeed)
            return true;
        else
            return false;


    }

    public float SpeedText()
    {
        float speed = 0;
        foreach (GameplayRigManager gmr in GameUI.instance.rigManagers)
        {
            foreach (GameplayRig rig in gmr.allRigs)
            {

                foreach (RigSlotTemp slot in rig.rigSlots2)
                {
                    if (slot.gpuSeries)
                    {
                        speed += GameManager.instance.GetGPUSpeed(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion);
                    }

                }
            }
        }

        return speed;
    }
}
