using UnityEngine;
using TMPro;
using Zenject;

public class DefaultTurretBar : BaseBar
{
    [Header("Default Turret")]
    [SerializeField] private TextMeshProUGUI titleText;
    [Inject] private Player _player;

    public override void ChangeValue(int currentValue, int maxValue)
    {
        titleText.text =  $"{currentValue}/{maxValue}";
    }

    private void Update()
    {
        if (_player != null)
            transform.LookAt(_player.transform);
    }
}
