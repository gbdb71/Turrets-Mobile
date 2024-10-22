﻿using UnityEngine;


[System.Serializable]
public class RewardSettings
{
    [System.Serializable]
    public class AbillityInfo
    {
        [TypeConstraint(typeof(BaseAbillity<>), AllowAbstract = false, AllowObsolete = false, TypeSettings = TypeSettings.Class, TypeGrouping = TypeGrouping.None)]
        public SerializedType Abillity;
        [Range(0f, 1f)] public float AbillityChance = .5f;
        public GameObject AbillityPrefab;
    }

    [Label("Currency Rewawrd", fontStyle: FontStyle.Bold, skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private GameObject _currencyPrefab;
    [SerializeField, MinMaxSlider(1, 100)] private Vector2 _amount;

    [Label("Abillities", fontStyle: FontStyle.Bold, skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private AbillityInfo[] _abillities;

    public GameObject CurrencyPrefab => _currencyPrefab;
    public int GetAmount() => (int)Random.Range(_amount.x, _amount.y);

    public IAbillity GetAbillity()
    {
        AbillityInfo info = _abillities[Random.Range(0, _abillities.Length)];

        if (Random.Range(0f, 1f) > info.AbillityChance)
            return null;

        if (info.Abillity.Type != null)
        {
            GameObject abillityObject = Object.Instantiate(info.AbillityPrefab);
            IAbillity abillity = (IAbillity)abillityObject.AddComponent(info.Abillity.Type);


            return abillity;
        }

        return null;
    }
}
