using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class UpgradeButton : CustomButton
{
    [SerializeField] private TextMeshProUGUI levelText;

    private Data _data;
    private UpgradeType _upgradeType;

    public void Initialization(Data data, UpgradeType upgradeType, string titleText, Action<UpgradeType> callback = null)
    {
        _data = data;
        _upgradeType = upgradeType;

        base.Initialization(titleText, -1, () => callback?.Invoke(_upgradeType));
    }

    protected override void UpdateButtonUI()
    {
        UpgradeList upgradeList = _data.UpgradesInfo.Upgrades.Where(x => x.Type == _upgradeType).First();
        int upgradeIndex = _data.User.UpgradesProgress[_upgradeType];

        _costText.text = CheckCount() ? upgradeList.Elements[upgradeIndex + 1].Cost.ToString() : "-";
        levelText.text = $"Lvl {(upgradeIndex + 1)}";
    }

    public bool CheckCount()
    {
        UpgradeList upgradeList = _data.UpgradesInfo.Upgrades.Where(x => x.Type == _upgradeType).First();
        int upgradeIndex = _data.User.UpgradesProgress[_upgradeType];

        if (upgradeIndex < upgradeList.Elements.Count - 1)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            return true;
        }
        else
        {
            levelText.text = $"Max";
            _costText.text = "";
            canvasGroup.alpha = 0.5f;
            canvasGroup.interactable = false;

            return false;
        }
    }
}
