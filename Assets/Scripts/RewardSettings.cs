using UnityEngine;

[System.Serializable]
public class RewardSettings
{
    [SerializeField, Range(.1f, 1f)] private float _rewardChance = .7f;
    [SerializeField, MinMaxSlider(1f, 300f)] private Vector2 _rewardAmount;
    [SerializeField] private CurrencyType _rewardType;

    public float RewardChance => _rewardChance;
    public Vector2 RewardAmount => _rewardAmount;   
    public CurrencyType RewardType => _rewardType;
}
