using System;
using System.Linq;
using ToolBox.Pools;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseAbillity<ConfigType> : MonoBehaviour, IAbillity where ConfigType : BaseAbillityConfig
{
    protected ConfigType _config;
    protected PopupText _popupPrefab;
    public Collider Collider { get; private set; }

    private void Awake()
    {
        _config = Resources.LoadAll<ConfigType>("GameData/Abillities").FirstOrDefault();
        _popupPrefab = Resources.Load<PopupText>("GameData/Effects/PopupText");

        if (_config == null)
        {
            _config = (ConfigType)Activator.CreateInstance(typeof(ConfigType), null);
        }

        Collider = GetComponent<Collider>();
    }

    public virtual void Clear() => Destroy(gameObject);
    public virtual void Activate()
    {
        PopupText popup = _popupPrefab.gameObject.Reuse<PopupText>(transform.position, Quaternion.identity);
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
