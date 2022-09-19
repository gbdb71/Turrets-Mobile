using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomButton : MonoBehaviour
{
    protected Headquarters _headquarters;

    [Header("Main Settings")]
    [SerializeField] protected Button button;
    [SerializeField] protected CanvasGroup canvasGroup;

    [Header("View Settings")]
    [SerializeField] private TextMeshProUGUI _titleText;
    
    public void Initialization(string titleText, Headquarters headquarters)
    {
        _headquarters = headquarters;
        _titleText.text = titleText;

        UpdateButtonUI();
        button.onClick.AddListener(ButtonPresed);
    }

    public virtual void ButtonPresed()
    {
        UpdateButtonUI();
    }

    protected virtual void UpdateButtonUI() { }
}

