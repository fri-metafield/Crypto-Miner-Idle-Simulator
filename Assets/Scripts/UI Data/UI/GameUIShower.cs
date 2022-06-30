using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIShower : MonoBehaviour
{
    [SerializeField] ShowUIType showType;
    [SerializeField] GameplayRig rigObject;
    [SerializeField] GPUSeries gpuObject;
    [SerializeField] SelectionInventory inventoryObject;

    string showText;

    private void Update()
    {
        if(showType == ShowUIType.RigPower)
        {
            showText = rigObject.curEarnPower.ToString();
        }
        else if (showType == ShowUIType.RigWatt)
        {
            showText = $": {rigObject.curPower} / {rigObject.curMaxPower} w";
        }
        else if (showType == ShowUIType.RigSlot)
        {
            showText = $": {rigObject.usedSlots} / {rigObject.usableSlots}";
        }
        ///////////
        else if (showType == ShowUIType.InventoryName)
        {
            showText = inventoryObject.gpuSeries.cardModel + " " + inventoryObject.gpuSeries.cardSeries + inventoryObject.gpuVersion.cardVersion;
        }
        else if (showType == ShowUIType.InventoryWatt)
        {
            showText = inventoryObject.gpuSeries.cardBasePower.ToString();
        }
        else if (showType == ShowUIType.InventoryPower)
        {
            showText = inventoryObject.gpuSeries.cardBaseSpeed.ToString();
        }
        else if (showType == ShowUIType.InventoryOC)
        {
            showText = GameManager.instance.GetGPUOverclock(inventoryObject.gpuBrand, inventoryObject.gpuSeries, inventoryObject.gpuVersion).ToString(); //inventoryObject.thisCard.cardBaseOverclockSpeed.ToString();
        }
        ////////////
        else if (showType == ShowUIType.GpuPower)
        {
            showText = gpuObject.cardBaseSpeed.ToString();
        }

        GetComponent<Text>().text = showText;
    }
}

public enum ShowUIType
{
    RigPower,
    RigWatt,
    RigSlot,

    InventoryName,
    InventoryWatt,
    InventoryPower,
    InventoryOC,

    GpuPower,
}
