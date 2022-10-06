using TMPro;
using UnityEngine;

public class UI_Helpers : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        HelperDrone.OnHelperCreated += UpdateInterface;
        HelperDrone.OnHelperDestroyed += UpdateInterface;    
    }

    private void Start()
    {
        UpdateInterface(null);
    }

    private void OnDestroy()
    {
        HelperDrone.OnHelperCreated -= UpdateInterface;
        HelperDrone.OnHelperDestroyed -= UpdateInterface;
    }

    private void UpdateInterface(HelperDrone helper)
    {
        _text.text = HelperDrone.Helpers.Count.ToString();
    }
}

