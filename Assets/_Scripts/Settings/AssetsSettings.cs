using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AssetsSettings", menuName = "Settings/Assets Settings", order = 999)]
public class AssetsSettings : ScriptableObject
{
    public List<AssetReferenceT<ModuleSettings>> _settingsList;
}
