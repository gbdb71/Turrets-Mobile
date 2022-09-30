using TMPro;
using UnityEngine;

public class UI_Helpers : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        Helper.OnHelperCreated += UpdateInterface;
        Helper.OnHelperDestroyed += UpdateInterface;    
    }

    private void Start()
    {
        UpdateInterface(null);
    }

    private void OnDestroy()
    {
        Helper.OnHelperCreated -= UpdateInterface;
        Helper.OnHelperDestroyed -= UpdateInterface;
    }

    private void UpdateInterface(Helper helper)
    {
        _text.text = Helper.Helpers.Count.ToString();
    }
}

