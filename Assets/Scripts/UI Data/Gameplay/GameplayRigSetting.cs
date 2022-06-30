using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayRigSetting : MonoBehaviour
{
    public GameplayRig thisRig;

    public int SelectedSlot;

    //UI
    [SerializeField] Image slotImage;
    [SerializeField] Text slotNameText;
    [SerializeField] GameObject lockedText;
    [SerializeField] Image brandImage;

    //-texts
    [SerializeField] Text powerText;
    [SerializeField] Text speedText;
    [SerializeField] Text ocText;
    [SerializeField] Text cycleText;

    //-total
    [SerializeField] Text powerTotalText;
    [SerializeField] Text speedTotalText;
    [SerializeField] Text ocTotalText;
    [SerializeField] Text cycleTotalText;

    //-bonus
    [SerializeField] Text brandSpeedBonus;
    [SerializeField] Text brandOCBonus;
    [SerializeField] Text brandCycleBonus;
    [SerializeField] Text brandPowerBonus;
    [SerializeField] Text brandPriceBonus;

    //total
    [SerializeField] Text rigTotalEarn;
    [SerializeField] Text rigTotalOCEarn;

    //-rig select sign
    [SerializeField] Sprite slotCircleOn;
    [SerializeField] Sprite slotCircleOff;

    [SerializeField] List<Image> slotCircle = new List<Image>();

    public List<SelectionRigSlot> slotNames = new List<SelectionRigSlot>();

    public static GameplayRigSetting instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (SelectionRigSlot name in slotNames)
        {
            name.slotText.text = "SLOT " + slotNames.IndexOf(name);
        }
        SetRig(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(thisRig)
        {
            if(slotNames[SelectedSlot].isUsable)
            {
                if (slotNames[SelectedSlot].gpuSeries)
                {
                    slotImage.sprite = GameManager.instance.GetGPUImage(slotNames[SelectedSlot].gpuBrand, slotNames[SelectedSlot].gpuSeries);
                    slotImage.color = Color.white;

                    brandImage.sprite = slotNames[SelectedSlot].gpuBrand.itemImage;
                    brandImage.color = Color.white;

                    slotNameText.text = slotNames[SelectedSlot].gpuBrand.itemName + " " + GameManager.instance.GetGPUName(slotNames[SelectedSlot].gpuModel, slotNames[SelectedSlot].gpuSeries, slotNames[SelectedSlot].gpuVersion);
                }
                else
                {
                    slotImage.color = new Color(0, 0, 0, 0);
                    slotNameText.text = "None";

                    brandImage.color = new Color(0, 0, 0, 0);
                }

                lockedText.SetActive(false);

            }
            else
            {
                slotImage.color = new Color(0, 0, 0, 0);
                slotNameText.text = "Locked";

                brandImage.color = new Color(0, 0, 0, 0);


                lockedText.SetActive(true);
            }

            GetData();



        }
        foreach (Image ci in slotCircle)
        {
            if (SelectedSlot == slotCircle.IndexOf(ci))
            {
                ci.sprite = slotCircleOn;
            }
            else
            {
                ci.sprite = slotCircleOff;
            }
        }

    }

    void GetData()
    {
        foreach (SelectionRigSlot slot in slotNames)
        {
            slot.SetName();
        }

        if (!slotNames[SelectedSlot].gpuSeries)
        {
            powerText.text = ": -";
            speedText.text = ": -";
            ocText.text = ": -";
            cycleText.text = ": -";

            brandSpeedBonus.text = ": -";
            brandOCBonus.text = ": -";
            brandCycleBonus.text = ": -";
            brandPowerBonus.text = ": -";
            brandPriceBonus.text = ": -";

            rigTotalEarn.text = ": -";
            rigTotalOCEarn.text = ": -";

            powerTotalText.text = ": -";
            speedTotalText.text = ": -";
            ocTotalText.text = ": -";
            cycleTotalText.text = ": -";

            return;
        }

        powerText.text = ": " + GameManager.instance.GetGPUPower(slotNames[SelectedSlot].gpuBrand, slotNames[SelectedSlot].gpuSeries, slotNames[SelectedSlot].gpuVersion) + " W";
        speedText.text = ": " + GameManager.instance.GetGPUSpeed(slotNames[SelectedSlot].gpuBrand, slotNames[SelectedSlot].gpuSeries, slotNames[SelectedSlot].gpuVersion) + " MHz";
        ocText.text = ": " + GameManager.instance.GetGPUOverclock(slotNames[SelectedSlot].gpuBrand, slotNames[SelectedSlot].gpuSeries, slotNames[SelectedSlot].gpuVersion) + " MHz";
        cycleText.text = ": " + slotNames[SelectedSlot].gpuModel.maxOverclockCycle + " Cycles";

        brandSpeedBonus.text = ": " + GameManager.instance.ScoreShow(slotNames[SelectedSlot].gpuBrand.brandSpeedBonus) + "%";
        brandOCBonus.text = ": " + GameManager.instance.ScoreShow(slotNames[SelectedSlot].gpuBrand.brandSpeedOverclockBonus) + "%";
        brandCycleBonus.text = ": " + GameManager.instance.ScoreShow(slotNames[SelectedSlot].gpuBrand.brandOverclockCycleBonus) + " Cycles";
        brandPowerBonus.text = ": " + GameManager.instance.ScoreShow(slotNames[SelectedSlot].gpuBrand.brandPowerBonus) + " W";
        brandPriceBonus.text = ": " + GameManager.instance.ScoreShow(slotNames[SelectedSlot].gpuBrand.brandCostBonus) + "%";

        /////////////////////// totals

        

        var tempEarn = 0f;
        foreach (RigSlotTemp slot in thisRig.rigSlots2)
        {
            if (slot.gpuSeries)
            {
                tempEarn += GameManager.instance.GetGPUSpeed(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion);
            }

        }
        rigTotalEarn.text = ": " + (thisRig.currentCurrency.currencyEarnValue * tempEarn);

        var tempOC = 0f;
        foreach(RigSlotTemp slot in thisRig.rigSlots2)
        {
            if(slot.gpuSeries)
            {
                tempOC += GameManager.instance.GetGPUOverclock(slot.gpuBrand, slot.gpuSeries, slot.gpuVersion);
            }
            
        }
        rigTotalOCEarn.text = ": " + (thisRig.currentCurrency.currencyEarnValue * tempOC);

        powerTotalText.text = ": " + thisRig.curPower + " W";
        speedTotalText.text = ": " + tempEarn + " MHz";
        ocTotalText.text = ": " + tempOC + " MHz";
        cycleTotalText.text = ": " + thisRig.overclockMaxCycle + " Cycles";
    }

    public void SetRig(GameplayRig rig)
    {
        thisRig = rig;
        RefreshSlot();

        if (!thisRig) return;
        foreach(RigSlotTemp slot in thisRig.rigSlots2)
        {
            slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuBrand = slot.gpuBrand;
            slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuModel = slot.gpuModel;
            slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuSeries = slot.gpuSeries;
            slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuVersion = slot.gpuVersion;
        }
    }

    public void SetRigData()
    {
        foreach(RigSlotTemp slot in thisRig.rigSlots2)
        {
            slot.gpuBrand = slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuBrand;
            slot.gpuModel = slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuModel;
            slot.gpuSeries = slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuSeries;
            slot.gpuVersion = slotNames[thisRig.rigSlots2.IndexOf(slot)].gpuVersion;
        }
        RefreshSlot();
    }

    public void RefreshSlot()
    {
        foreach (SelectionRigSlot name in slotNames)
        {
            if (!thisRig) return;

            if (slotNames.IndexOf(name) + 1 <= thisRig.usableSlots)
            {
                name.isUsable = true;
            }
            else
            {
                name.isUsable = false;
            }
        }
        GetData();
    }

    //UI
    public void ButtonNextSlot(int amount)
    {
        SelectedSlot += amount;
        if (SelectedSlot > 7)
            SelectedSlot = 0;
        if (SelectedSlot < 0)
            SelectedSlot = 7;
    }
}
