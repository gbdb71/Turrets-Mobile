using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Upgrades", menuName = "TowerDefense/Upgrade Info")]
public class UpgradesInfo : ScriptableObject
{
    [LabelByChild("Type")]
    public List<UpgradeList> Upgrades = new List<UpgradeList>();
}

public enum UpgradeType
{
    Speed,
    SpeedWithTurret,
    AmmoCount
}

[Serializable]
public class UpgradeList
{
    public UpgradeType Type;
    public List<Upgrade> Elements;
}

[Serializable]
public class Upgrade
{
    public float Value;
    public int Cost;
}
