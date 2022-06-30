using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GPU Model", menuName = "CustomScriptable/GPU Model")]
public class GPUModel : Slot
{
    public string cardModelName;
    public int maxOverclockCycle;
}
