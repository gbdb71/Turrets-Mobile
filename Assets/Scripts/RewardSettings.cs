using UnityEngine;

[System.Serializable]
public class RewardSettings
{
    [SerializeField] private GameObject _rewardPrefab;
    [SerializeField, MinMaxSlider(1, 100)] private Vector2 _amount;

    public GameObject RewardPrefab => _rewardPrefab;
    public int GetAmount() => (int)Random.Range(_amount.x, _amount.y);
}
