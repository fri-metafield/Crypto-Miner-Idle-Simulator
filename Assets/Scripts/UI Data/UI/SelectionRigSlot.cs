using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionRigSlot : MonoBehaviour
{
    public bool isUsable;

    public Text slotName;

    public Image slotBar;
    [SerializeField] Sprite slotGreen;
    [SerializeField] Sprite slotGray;
    [SerializeField] Sprite slotRed;

    public Text slotText;

    public Brand gpuBrand;
    public GPUModel gpuModel;
    public GPUSeries gpuSeries;
    public GPUVersion gpuVersion;

    private void Start()
    {
    }

    public void SetSlotGPU(Brand gb, GPUModel gm, GPUSeries gs, GPUVersion gv)
    {
        //currentGPU = g;
        gpuBrand = gb;
        gpuModel = gm;
        gpuSeries = gs;
        gpuVersion = gv;

        //SetName();
        GameplayRigSetting.instance.RefreshSlot();
    }

    public void SetName()
    {
        var gms = GameplayRigSetting.instance;

        if (isUsable)
        {
            if (gms.thisRig.rigSlots2[gms.slotNames.IndexOf(this)].gpuSeries)
            {
                if(gms.slotNames[gms.slotNames.IndexOf(this)].gpuSeries)
                {
                    slotName.text = GameManager.instance.GetGPUName(gpuModel, gpuSeries, gpuVersion);
                    slotBar.sprite = slotGreen;
                }
            }
            else
            {
                slotName.text = "NONE";
                slotBar.sprite = slotRed;
            }
        }
        else
        {
            slotName.text = "LOCKED";
            slotBar.sprite = slotGray;
        }

    }
}
