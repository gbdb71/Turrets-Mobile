using UnityEngine;
using TMPro;

public class DefaultTurretBar : BaseBar
{
    [Header("Default Turret")]
    [SerializeField] private TextMeshProUGUI titleText;

    public override void ChangeValue(int currentValue, int maxValue)
    {
        titleText.text =  $"{currentValue}/{maxValue}";
    }
}
