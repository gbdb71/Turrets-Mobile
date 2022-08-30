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
        baseGroup.alpha = 0;
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

    public void OpenViewGroup()
    {
        baseGroup.alpha = 1;
    }

    public void ClosedViewGroup()
    {
        baseGroup.alpha = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerInventory player))
        {
            Debug.Log(player);
            if (!player.HasTurret)
                OpenViewGroup();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerInventory player))
        {
            //if (!player.HasTurret)
            ClosedViewGroup();
        }
    }
}
