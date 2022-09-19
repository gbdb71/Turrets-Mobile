using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeButton : CustomButton
{
    private UpgradeList currentUpgradeList;
    private int _upgrade;

    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;

    public void Initialization(UpgradeList upgradeList, int upgrade, string titleText, Headquarters headquarters)
    {
        currentUpgradeList = upgradeList;
        _upgrade = upgrade;
        Debug.Log(upgradeList);

        base.Initialization(titleText, headquarters);
    }

    protected override void UpdateButtonUI()
    {
        costText.text = CheckCount() ? currentUpgradeList.elementList[_upgrade + 1].Cost.ToString() : "-";
        levelText.text = $"Lvl {(_upgrade + 1)}";
    }

    public override void ButtonPresed()
    {
        if (_headquarters.TryWithdrawCurrency(CurrencyType.Upgrade, currentUpgradeList.elementList[_upgrade + 1].Cost))
        {
            _upgrade += 1;
            _headquarters.ValuePassing(currentUpgradeList.Type, currentUpgradeList.elementList[_upgrade].Value, _upgrade);

            base.ButtonPresed();
        }
    }

    public bool CheckCount()
    {
        if (_upgrade < currentUpgradeList.elementList.Count - 1)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            return true;
        }
        else
        {
            levelText.text = $"Max";
            costText.text = "";
            canvasGroup.alpha = 0.5f;
            canvasGroup.interactable = false;

            return false;
        }
    }
}
