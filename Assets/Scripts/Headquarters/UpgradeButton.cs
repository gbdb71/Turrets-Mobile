using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private UpgradeList currentUpgradeList;

    [SerializeField] private bool buttonIsActive;
    [SerializeField] private int upgradeCount;

    [Header("View Settings")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Initialization(UpgradeList upgradeList)
    {
        //button = GetComponentInChildren<Button>();
        currentUpgradeList = upgradeList;
        upgradeCount = 0;

        buttonIsActive = CheckCount();
        button.onClick.AddListener(() => ButtonPresed());
    }

    public void ButtonPresed()
    {
        if (buttonIsActive)
        {
            DebugPressed(currentUpgradeList.elementList[upgradeCount].Value, currentUpgradeList.elementList[upgradeCount].Cost);
            upgradeCount += 1; 
            CheckCount();
        }
    }

    public void UpdateButtonUI()
    {
        titleText.text = currentUpgradeList.name;
        costText.text = currentUpgradeList.elementList[upgradeCount].Cost.ToString();
    }

    public void DebugPressed(float value, float cost)
    {
        Debug.Log($"Upgrade - {currentUpgradeList.name} // Upgrade Level {upgradeCount} // Value {value} | Cost {cost}");
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

