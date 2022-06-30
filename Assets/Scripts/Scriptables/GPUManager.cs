using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GPU Manager", menuName = "CustomScriptable/GPU Manager")]
public class GPUManager : ScriptableObject
{
    //public List<GPU> gpuList = new List<GPU>();
    //public List<GPUSeries> gpuBrandList = new List<GPUSeries>();
    public List<GPUModel> gpuModelsList = new List<GPUModel>();
    public List<GPUSeries> gpuSeriesList = new List<GPUSeries>();
    public List<GPUVersion> gpuVersionList = new List<GPUVersion>();
    //public List<int> versionList = new List<int>();
}
