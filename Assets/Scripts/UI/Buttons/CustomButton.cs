using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] protected Button button;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected TextMeshProUGUI _costText;

    [Header("View Settings")]
    [SerializeField] private TextMeshProUGUI _titleText;
    
    public void Initialization(string titleText, int cost, UnityAction callback)
    {
        _titleText.text = titleText;
        _costText.text = cost.ToString();

        UpdateButtonUI();

        button.onClick.AddListener(callback);
        button.onClick.AddListener(ButtonPresed);
    }

    public virtual void ButtonPresed()
    {
        UpdateButtonUI();
    }

    protected virtual void UpdateButtonUI() { }
}

