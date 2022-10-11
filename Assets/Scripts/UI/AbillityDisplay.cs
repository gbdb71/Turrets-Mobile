using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbillityDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _icon;

    private BaseAbillity _abillity;

    private void Awake()
    {
        _abillity = GetComponent<BaseAbillity>();
    }

    private void OnEnable()
    {
        if (_abillity != null)
        {
            BaseAbillity.AbillityInfo abillityInfo = _abillity.GetInfo();

            _icon.sprite = abillityInfo.Icon;
            _title.text = abillityInfo.Title;
            _description.text = abillityInfo.Description;
        }
    }
}
