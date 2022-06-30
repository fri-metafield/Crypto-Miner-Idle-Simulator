using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayProfile : MonoBehaviour
{
    //sound
    public bool audioOn;
    public bool musicOn;

    //notif
    public bool notifOn;

    //UI
    [SerializeField] Text audioText;
    [SerializeField] Text musicText;
    [SerializeField] Text notifText;

    //-coupon
    [SerializeField] InputField couponInput;
    [SerializeField] GameObject InvalidText;

    public static GameplayProfile instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (audioOn) audioText.text = "On";
        else audioText.text = "Off";

        if (musicOn) musicText.text = "On";
        else musicText.text = "Off";

        if (notifOn) notifText.text = "On";
        else notifText.text = "Off";
    }

    public void ButtonClickLink(string link)
    {
        //open url
        Application.OpenURL(link);
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

    public void ButtonSetAudio()
    {
        audioOn = !audioOn;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    public void ButtonSetMusic()
    {
        musicOn = !musicOn;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }
    public void ButtonSetNotif()
    {
        notifOn = !notifOn;
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }


    public void InputCoupon()
    {
        if(couponInput.text == "nm10")
        {
            GameManager.instance.AddMoney(1000000);
        }
        else if(couponInput.text == "nm11")
        {
            GameManager.instance.AddCoin(1);
        }
        else if(couponInput.text == "nm12")
        {
            GameManager.instance.AddPremium(100);
        }
        else
        {
            InvalidText.SetActive(true);
            return;
        }
        InvalidText.SetActive(false);

        couponInput.text = null;
        SaveManager.instance.SaveOfflineProduction();
        GameManager.instance.PlaySound(GameManager.instance.sfxGeneral, false);
    }

}
