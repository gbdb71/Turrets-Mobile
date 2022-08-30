using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private UpgradeList currentUpgradeList;

    [SerializeField] private bool buttonIsActive;
    private int upgradeCount;

    public void Initialization(UpgradeList upgradeList)
    {
        button = GetComponentInChildren<Button>();
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
            CheckCount();
        }
    }

    public void UpdateButtonUI()
    {

    }

    public void DebugPressed(float value, float cost)
    {
        Debug.Log($"Upgrade - {currentUpgradeList.name} // Upgrade Level {upgradeCount} // Value {value} | Cost {cost}");
    }

    public bool CheckCount()
    {
        if (upgradeCount + 1 < currentUpgradeList.elementList.Count) 
        {
            upgradeCount += 1;
            return true;
        }
        else {
            button.interactable = false;
            return false;
        } 
    }
}

