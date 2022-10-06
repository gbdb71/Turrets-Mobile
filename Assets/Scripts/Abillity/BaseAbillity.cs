using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class BaseAbillity : MonoBehaviour
{
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

    protected virtual void Activate()
    {
        if (_system != null)
            _system.OnAbillityActivated(this);
    }
}
