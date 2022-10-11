using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummableAbillity : BaseAbillity
{
    [SerializeField, Range(1, 100)] protected float _startValue = 1;
    [SerializeField] protected Type _type;

    protected int _currentLevel = 1;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Activate()
    {
        base.Activate();

        float percents = _currentLevel * _startValue;

        if (_values.ContainsKey(_type))

            _values[_type] += percents;
        else
            _values.Add(_type, percents);
    }

    public override AbillityInfo GetInfo()
    {
        AbillityInfo info = base.GetInfo();

        float percents = _currentLevel * _startValue;


        try
        {
            return new AbillityInfo
            {
                Title = info.Title,
                Icon = info.Icon,
                Description = string.Format(info.Description, percents),
            };
        }
        catch (Exception e)
        {
            Debug.Log(info.Title);
            Debug.Log(info.Description);
            Debug.LogError(e);
        }

        return info;
    }

    public override void Clear()
    {
        _values.Clear();
    }

    public enum Type
    {
        TurretDamage,
        TurretFire,
        TurretReload,
        PlayerMovementSpeed,
        HeadquartersHealth,
        Loot
    }

    private static Dictionary<Type, float> _values = new Dictionary<Type, float>();
    public static float GetValue(Type type)
    {
        if (_values == null)
            return 0f;

        if (_values.TryGetValue(type, out var value))
            return value;

        return 0f;
    }
}

