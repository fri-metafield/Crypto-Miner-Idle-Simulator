using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionCurrency : MonoBehaviour
{
    public Currencies thisCurrency;

    //UI
    [SerializeField] Text currencyName;
    [SerializeField] Text currencyName2;

    [SerializeField] Image currencyLogo;
    [SerializeField] Text currencyTime;
    [SerializeField] Text currencyRate;
    [SerializeField] Text currencyAmount;

    [SerializeField] GameObject LockedObject;
    [SerializeField] GameObject unlockObject;
    [SerializeField] GameObject unavailableObject;


    private void Update()
    {
        unlockObject.SetActive(!unavailableObject.activeSelf && !LockedObject.activeSelf);

        if (GameUI.instance.selectedSelection == 2)
        {
            currencyName.text = thisCurrency.currencyName.ToString();
            currencyLogo.sprite = thisCurrency.currencyImage;
            currencyTime.text = ": " + GameManager.instance.ScoreShow(thisCurrency.currencyEarnTime) + " s";
            currencyRate.text = ": " + thisCurrency.currencyEarnValue.ToString("F6") + "/Mhz";

            currencyAmount.text = SelectionCurrencyHolder.instance.SpeedText() + " / " + thisCurrency.currencyUnlockSpeed + " MHz";

            /*
            if (SelectionCurrencyHolder.instance.HasEnoughSpeed(thisCurrency))
            {
                LockedObject.SetActive(false);
                unavailableObject.SetActive(false);
            }
            else
            {
                //if amount > last
                
                if(SelectionCurrencyHolder.instance.SpeedText() >= SelectionCurrencyHolder.instance.currencyUI[SelectionCurrencyHolder.instance.currencyUI.IndexOf(this)-1].thisCurrency.currencyUnlockSpeed)
                {
                    
                    unavailableObject.SetActive(true);
                    LockedObject.SetActive(false);
                }
                else
                {
                    unavailableObject.SetActive(false);
                    LockedObject.SetActive(true);
                }
                
                
            }
            */
            if(SelectionCurrencyHolder.instance.unlockedCoin >= SelectionCurrencyHolder.instance.currencyUI.IndexOf(this)+1)
            {
                LockedObject.SetActive(false);
                unavailableObject.SetActive(false);
            }
            else if (SelectionCurrencyHolder.instance.unlockedCoin == SelectionCurrencyHolder.instance.currencyUI.IndexOf(this))
            {
                unavailableObject.SetActive(true);
                LockedObject.SetActive(false);
            }
            else
            {
                unavailableObject.SetActive(false);
                LockedObject.SetActive(true);
            }
        }
    }

    

    public void ButtonOnClick()
    {
        if(unlockObject.activeSelf)
            GameUI.instance.SetCoin(thisCurrency);
    }
}
