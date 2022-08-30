using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Headquarters : MonoBehaviour
{
    [Header("DATA Settings")]
    public UpgradesInfo upgradesInfo;
    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    [Header("Siew Settings")]
    public CanvasGroup baseGroup;

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
                Debug.Log($"No More Upgrade Info {i}");
                upgradeButtons[i].gameObject.SetActive(false);
                break;
            }
        }
    }


}
