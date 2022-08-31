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

    [SerializeField] private bool buttonIsActive;
    [SerializeField] private int upgradeCount;

    [Header("View Settings")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Initialization(UpgradeList upgradeList, Headquarters headquarters)
    {
        currentUpgradeList = upgradeList;
        _headquarters = headquarters;

        upgradeCount = 0;

        buttonIsActive = CheckCount();
        button.onClick.AddListener(() => ButtonPresed());

        OutputValue();
    }

    public void ButtonPresed()
    {
        if (buttonIsActive)
        {
            //DebugPressed(currentUpgradeList.elementList[upgradeCount].Value, currentUpgradeList.elementList[upgradeCount].Cost);
            currentUpgradeList.elementList[upgradeCount].Debug();
            OutputValue();
            upgradeCount += 1;
            CheckCount();
        }
    }

    private void UpdateButtonUI()
    {
        titleText.text = currentUpgradeList.name;
        costText.text = currentUpgradeList.elementList[upgradeCount].Cost.ToString();
    }

    private void OutputValue()
    {
        _headquarters.ValuePassing(currentUpgradeList.currentType, currentUpgradeList.elementList[upgradeCount].Value);
    }


    public bool CheckCount()
    {
        if (upgradeCount < currentUpgradeList.elementList.Count)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            levelText.text = "Lvl " + (upgradeCount + 1).ToString();

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

public static class Ectencion
{
    public static void Debug(this Upgrade upgrade)
    {
        //Debug.Log($"Upgrade - {currentUpgradeList.name} // Upgrade Level {upgrade.upgradeCount} // Value {upgrade.value} | Cost {cost}")
        UnityEngine.Debug.Log($"Value {upgrade.Value} // Cost {upgrade.Cost}");
    }
}

