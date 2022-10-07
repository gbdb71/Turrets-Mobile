using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseAbillity : MonoBehaviour
{
    [System.Serializable]
    public class AbillityInfo
    {
        public string Title;
        public string Description;
    }

    [SerializeField] protected AbillityInfo _info;

    private Button _activateButton;
    protected AbillitySystem _system;

    protected virtual void Awake()
    {
        _activateButton = GetComponent<Button>();
        _activateButton.onClick.AddListener(Activate);
    }

    public void SetSystem(AbillitySystem system)
    {
        _system = system;
    }

    public abstract void Clear();

    public virtual AbillityInfo GetInfo() => _info;

    protected virtual void Activate()
    {
        if (_system != null)
            _system.OnAbillityActivated(this);
    }
}
