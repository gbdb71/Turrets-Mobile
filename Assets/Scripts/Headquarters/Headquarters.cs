using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Headquarters : MonoBehaviour
{
    public UpgradesInfo upgradesInfo;

    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    public void Awake()
    {
        InitializationButtons();
    }

    private void InitializationButtons()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < upgradesInfo.upgrades.Count && upgradesInfo.upgrades[i] != null)
                upgradeButtons[i].Initialization(upgradesInfo.upgrades[i]);
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
                Debug.Log($"No More Upgrade Info {i}");
                break;
            }
        }
    }
}
