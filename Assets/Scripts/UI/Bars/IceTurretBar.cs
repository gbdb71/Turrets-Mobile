using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IceTurretBar : BaseBar
{
    [Header("Ice Turret")]
    [SerializeField] private Image _fill;
    [SerializeField] private Transform _linesParent;

    [SerializeField] private RectTransform _linePrefab;
    [SerializeField] private float fillTime = 0.25f;

    public void SetStepsCount(int steps)
    {
        if (_linesParent == null || _linePrefab == null)
            return;

        int offsetStep = (int)((float)Math.Round(1f / steps, 2, MidpointRounding.ToEven) * 300);

        for (int i = 0; i < steps - 1; i++)
        {
            RectTransform line = Instantiate(_linePrefab, _linesParent.transform);
            line.anchoredPosition = new Vector3(offsetStep * (i + 1), 0, 0);
        }
    }

    public override void ChangeValue(int currentValue, int maxValue)
    {
        float fill = (float)Math.Round((float)currentValue / maxValue, 2, MidpointRounding.ToEven);
        _fill.DOFillAmount(fill, fillTime);
    }
}
