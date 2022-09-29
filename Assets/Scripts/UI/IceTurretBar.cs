using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceTurretBar : BaseBar
{
    [Header("Ice Turret")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GridLayoutGroup canvasGroup;

    [SerializeField] private GameObject linePrefab;

    public override void Initialization(int currentValue, int maxValue)
    {
        Debug.Log("Dod");
        int newFill = (maxValue / 100) * currentValue;
        fillImage.fillAmount = 1 - newFill;

        for (int i = 0; i < maxValue; i++)
        {
            GameObject line = Instantiate(linePrefab, canvasGroup.transform);
        }
        Debug.Log("Initialization | Max " + maxValue);
    }

    public override void ChangeValue(int currentValue, int maxValue)
    {
        int newFill = (maxValue / 100) * currentValue;
        Debug.Log(newFill);

        fillImage.fillAmount = 1 - newFill;
    }
}
