using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayBooster : MonoBehaviour
{
    public bool isBoosting;
    public float boostTimer;
    [SerializeField] float boostAdd;
    [SerializeField] float maxBoostTimer;

    public bool isBoostShop;

    //UI
    [SerializeField] Text boostTimerText;
    [SerializeField] Text boostTimerOutsideText;

    [SerializeField] Slider boostTimerBar;
    [SerializeField] Button boosterButton;

    public static GameplayBooster instance;

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

        if(boostTimer > 0)
        {
            if(isBoostShop)
            {
                boosterButton.interactable = false;
            }

            boostTimer -= Time.deltaTime;
            isBoosting = true;
        }
        else
        {
            if (boostTimer <= 0)
            {
                boostTimer = 0;
                if(isBoosting)
                    ResetBoost();
            }
        }

        

        if (boostTimer > maxBoostTimer)
        {
            if(!isBoostShop)
            {
                boostTimer = maxBoostTimer;
            }
        }

        //UI

        boostTimerText.text = GameManager.instance.GenTimeSpanFromSeconds(boostTimer);
        boostTimerOutsideText.text = GameManager.instance.GenTimeSpanFromSeconds(boostTimer);


        boostTimerBar.maxValue = maxBoostTimer;
        boostTimerBar.value = boostTimer;
    }

    [ContextMenu("Add Boost")]
    void AddBoost()
    {
        boostTimer += boostAdd;
    }

    [ContextMenu("Remove Boost")]
    void RemoveBoost()
    {
        boostTimer -= boostAdd;
    }

    void ResetBoost()
    {
        isBoosting = false;

        if (isBoostShop)
        {
            isBoostShop = false;
            boosterButton.interactable = true;
        }

        foreach (GameplayRigManager grm in GameUI.instance.rigManagers)
        {
            foreach(GameplayRig gr in grm.allRigs)
            {
                gr.ResetCycle();
            }
        }
    }

    //Button
    public void ButtonClickBoost()
    {
        AddBoost();
    }

    //shop
    public void ShopBooster(float timer)
    {
        isBoostShop = true;
        boostTimer = timer;
    }
}
