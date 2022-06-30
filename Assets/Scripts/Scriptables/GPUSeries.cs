using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GPU Series", menuName = "CustomScriptable/GPU Series")]
public class GPUSeries : Slot
{
    public GPUManager cardManager;

    public GPUModel cardModel;
    public string cardSeries;
    public int cardSeriesCounter;

    public GPUImage cardImages;

    public float cardPrice;
    
    public int cardBasePower;
    public int cardBaseSpeed;

    public int cardPowerIncrease;
    //public int cardSpeedIncrease;
    //public int cardBaseOverclock;

    //shop only 
    public int cardMarketUnlockOrder;
}

[System.Serializable]
public class GPUImage
{
    public Sprite ColossalImage;
    public Sprite EFCAImage;
    public Sprite MCIImage;
    public Sprite NfrididaImage;
    public Sprite SUSAImage;
}
