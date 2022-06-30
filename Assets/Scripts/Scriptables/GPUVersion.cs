using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GPU Version", menuName = "CustomScriptable/GPU Version")]
public class GPUVersion : Slot
{
    public GPUModel cardModel;
    public string cardVersion;
    public int cardVersionCounter;
}
