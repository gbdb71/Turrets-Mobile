using System;
using DG.Tweening;
using System.Linq;
using ToolBox.Pools;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public abstract class BaseAbillity<ConfigType> : MonoBehaviour, IAbillity where ConfigType : BaseAbillityConfig
{
    [Inject] protected Player _player;

    private bool _isActivated = false;
    protected ConfigType _config;
    protected PopupText _popupPrefab;
    public Collider Collider { get; private set; }

    public event Action OnActivated;

    private void Awake()
    {
        _config = Resources.LoadAll<ConfigType>("GameData/Abillities").FirstOrDefault();
        _popupPrefab = Resources.Load<PopupText>("GameData/Effects/PopupText");

        Collider = GetComponent<Collider>();

        if(Collider == null)
        {
            Collider = gameObject.AddComponent<Collider>();
        }

        Collider.isTrigger = true;
    }

    public virtual void Clear()
    {
        transform.DOScale(0f, .3f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
    }
    public virtual void Activate()
    {
        _isActivated = true;
        
        PopupText popup = _popupPrefab.gameObject.Reuse<PopupText>(PopupPosition, Quaternion.identity);
        popup.SetColor(_config.TextColor);
        popup.SetText(_config.ActivateText, PopupText.DurationType.Long);
        
        OnActivated?.Invoke();
    }
    public virtual bool CanActivate() => true && !_isActivated;
    public virtual bool HasDelay() => true;

    public Transform GetTransform() => transform;
    public virtual Vector3 PopupPosition => _player.transform.position + new Vector3(0, 2.2f, 0);
}

public abstract class BaseAbillityConfig : ScriptableObject
{
    public string ActivateText;
    public Color TextColor = Color.white;
}
