using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Headquarters _headquarters;

    [Header("Main Settings")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;

    public UpgradeList currentUpgradeList;

    [SerializeField] private int _upgrade;

    [Header("View Settings")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Initialization(UpgradeList upgradeList, int upgrade, Headquarters headquarters)
    {
        currentUpgradeList = upgradeList;
        _headquarters = headquarters;

        _upgrade = upgrade;

        CheckCount();
        button.onClick.AddListener(ButtonPresed);
    }

    public void ButtonPresed()
    {
        _headquarters.ValuePassing(currentUpgradeList.Type, currentUpgradeList.elementList[_upgrade].Value);

        _upgrade++;

        CheckCount();
    }

    private void UpdateButtonUI()
    {
        titleText.text = currentUpgradeList.name;
        costText.text = currentUpgradeList.elementList[_upgrade].Cost.ToString();
        levelText.text = $"Lvl {(_upgrade + 1)}";
    }


    public bool CheckCount()
    {
        if (_upgrade < currentUpgradeList.elementList.Count)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            UpdateButtonUI();
            return true;
        }
        else
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.interactable = false;

            return false;
        }
    }
}

