using TMPro;
using UnityEngine;

public class AbillityDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;

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

            _title.text = abillityInfo.Title;
            _description.text = abillityInfo.Description;
        }
    }
}
