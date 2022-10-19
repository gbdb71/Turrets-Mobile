using System.Linq;
using ToolBox.Pools;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public abstract class BaseAbillity<ConfigType> : MonoBehaviour, IAbillity where ConfigType : BaseAbillityConfig
{
    [Inject] protected Player _player;

    protected ConfigType _config;
    protected PopupText _popupPrefab;
    public Collider Collider { get; private set; }

    private void Awake()
    {
        _config = Resources.LoadAll<ConfigType>("GameData/Abillities").FirstOrDefault();
        _popupPrefab = Resources.Load<PopupText>("GameData/Effects/PopupText");

        Collider = GetComponent<Collider>();
    }

    public virtual void Clear() => Destroy(gameObject);
    public virtual void Activate()
    {
        PopupText popup = _popupPrefab.gameObject.Reuse<PopupText>(_player.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        popup.SetColor(_config.TextColor);
        popup.SetText(_config.ActivateText);
    }
    public virtual bool CanActivate() => true;

    public Transform GetTransform() => transform;
}

public abstract class BaseAbillityConfig : ScriptableObject
{
    public string ActivateText;
    public Color TextColor = Color.white;
}
