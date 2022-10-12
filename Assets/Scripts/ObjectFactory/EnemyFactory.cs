using UnityEngine;

[System.Serializable]
class EnemyConfig
{
    [AssetPreview, NotNull]
    public Enemy Prefab = default;

    [MinMaxSlider(0.5f, 2f)]
    public Vector2 Scale;

    [MinMaxSlider(0.2f, 5f)]
    public Vector2 Speed;

    [MinMaxSlider(-2f, 2f)]
    public Vector2 PathOffset;

    [MinMaxSlider(10f, 4000f)]
    public Vector2 Health;

    [Range(1, 300)]
    public float Damage;

    public RewardSettings RewardSettings;
}

[CreateAssetMenu(fileName = "EnemyFactory", menuName = "TowerDefense/Enemy/Factory")]
public partial class EnemyFactory : GameObjectFactory<Enemy, EnemyType>
{
    [Label("Enemies Types", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [BeginGroup]
    [SerializeField] private EnemyConfig _small = default;
    [SerializeField] private EnemyConfig _large = default;
    [SerializeField, EndGroup] private EnemyConfig _medium = default;


    public override Enemy Get(EnemyType type = EnemyType.Medium)
    {
        EnemyConfig config = GetConfig(type);

        float scale = Random.Range(config.Scale.x, config.Scale.y);
        float speed = Random.Range(config.Speed.x, config.Speed.y);
        float pathOffset = Random.Range(config.PathOffset.x, config.PathOffset.y);
        float health = Random.Range(config.Health.x, config.Health.y);

        Enemy instance = CreateGameObjectInstance(config.Prefab);
        instance.OriginFactory = this;
        instance.Initialize(scale,speed,pathOffset,
            health, config.Damage, config.RewardSettings);

        return instance;
    }

    public override void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");

        Destroy(enemy.gameObject);
    }


    private EnemyConfig GetConfig(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Small: return _small;
            case EnemyType.Large: return _large;
            case EnemyType.Medium: return _medium;
        }

        return null;
    }
}
