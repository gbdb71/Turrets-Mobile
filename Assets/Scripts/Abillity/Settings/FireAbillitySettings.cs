using UnityEngine;

[CreateAssetMenu(fileName = "FireAbillity", menuName = "TowerDefense/Abillities/Turret Fire")]
public class FireAbillitySettings : BaseAbillityConfig
{
    [SerializeField, MinMaxSlider(1, 20)] private Vector2 _firePercents = new Vector2(1, 10);

    public float GetFirePercents() => Random.Range(_firePercents.x, _firePercents.y);
}
