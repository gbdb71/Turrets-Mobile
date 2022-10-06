using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummableAbillity : BaseAbillity
{
    [SerializeField, Range(1, 100)] protected float _startValue = 1;
    [SerializeField] protected Type _type;

    protected int _currentLevel = 1;
    private TextMeshProUGUI _title;

    protected override void Awake()
    {
        base.Awake();

        _title = GetComponentInChildren<TextMeshProUGUI>();
        _title.text = _type.ToString();
    }

    protected override void Activate()
    {
        base.Activate();

        float value = _currentLevel * _startValue;

        if (_values.ContainsKey(_type))

            _values[_type] += value;
        else
            _values.Add(_type, value);
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

        if(_values.TryGetValue(type, out var value))
            return value;

        return 0f;
    }
}

