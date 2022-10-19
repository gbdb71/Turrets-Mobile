using UnityEngine;

[CreateAssetMenu(fileName = "DamageAbillity", menuName = "TowerDefense/Abillities/Turret Damage")]
public class DamageAbillitySettings : BaseAbillityConfig
{
    [SerializeField, MinMaxSlider(1, 20)] private Vector2 _damagePercents = new Vector2(1, 10);

    public float GetDamagePercents() => Random.Range(_damagePercents.x, _damagePercents.y);
}
