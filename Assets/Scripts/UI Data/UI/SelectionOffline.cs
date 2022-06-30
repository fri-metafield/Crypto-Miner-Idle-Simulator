using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionOffline : MonoBehaviour
{
    [SerializeField] float doublePrice;
    int multiplier = 1;

    //UI
    [SerializeField] GameObject multiplyBar;
    [SerializeField] Text multiplyNormal;
    [SerializeField] Text multipliedProfit;
    [SerializeField] Text multiplyPrice;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        multiplyPrice.text = doublePrice.ToString();

        multiplyNormal.text = GameUI.instance.tempMoney.ToString() + " BTC";
        multipliedProfit.text = (GameUI.instance.tempMoney * multiplier).ToString() + " BTC";
    }

    void TripleMoney(int am)
    {
        multiplier = am;

    }

    //UI
    public void ButtonOnClick()
    {
        GameUI.instance.CollectMoney(multiplier);
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void Multiply(int am)
    {
        TripleMoney(am);
    }
}
