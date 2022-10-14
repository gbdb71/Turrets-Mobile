using UnityEngine;

[System.Serializable]
public class EnemySpawnSequence
{
    [SerializeField]
    private EnemyFactory _factory = default;

    [SerializeField]
    private EnemyType _type = EnemyType.Medium;

    [SerializeField, Range(1, 100)]
    private int _amount = 1;

    [SerializeField, Range(0.1f, 30f)]
    private float _cooldown = 1f;

    public State Begin(Road road) => new State(this, road);
    public int Amount => _amount;


    [System.Serializable]
    public class State
    {
        private EnemySpawnSequence _sequence;

        private int _count;
        private float _cooldown;
        private Enemy[] spawnedEnemy;
        private Road _road;

        public float SequenceProgress => (float)_count / _sequence._amount;

        public State(EnemySpawnSequence sequence, Road road)
        {
            _sequence = sequence;
            _road = road;

            _count = 0;
            _cooldown = sequence._cooldown;
            spawnedEnemy = new Enemy[sequence._amount];
        }

        public float Progress(float deltaTime)
        {
            _cooldown += deltaTime;

            while (_cooldown >= _sequence._cooldown)
            {
                _cooldown -= _sequence._cooldown;

                if (_count >= _sequence._amount)
                {
                    for (int i = 0; i < spawnedEnemy.Length; i++)
                        if (spawnedEnemy[i] != null || !spawnedEnemy[i].IsDead)
                            return -1f;

                    return _cooldown;
                }

                Enemy enemy = _sequence._factory.Get(_sequence._type);
                enemy.SpawnOn(_road.Spline);
                spawnedEnemy[_count] = enemy;

                _count += 1;
            }

            return -1f;
        }
    }
}
