using UnityEngine;

[System.Serializable]
public class EnemySpawnSequence
{
    [SerializeField] private EnemyFactory _factory = default;

    [SerializeField] private EnemyType _type = EnemyType.Medium;

    [SerializeField, Range(1, 100)] private int _amount = 1;

    [SerializeField, Range(0.1f, 30f)] private float _cooldown = 1f;

    public State Begin(Road road) => new State(this, road);
    public int Amount => _amount;
    public EnemyType Type => _type;


    [System.Serializable]
    public class State
    {
        public EnemySpawnSequence Sequence { get; private set; }

        private int _count;
        private float _cooldown;
        private Enemy[] spawnedEnemy;
        private Road _road;

        public int Count => _count;
        public float SequenceProgress => (float)_count / Sequence._amount;

        public State(EnemySpawnSequence sequence, Road road)
        {
            Sequence = sequence;
            _road = road;

            _count = 0;
            _cooldown = sequence._cooldown;
            spawnedEnemy = new Enemy[sequence._amount];
        }

        public float Progress(float deltaTime)
        {
            _cooldown += _count < Sequence._amount ? deltaTime : Sequence._cooldown;


            while (_cooldown >= Sequence._cooldown)
            {
                _cooldown -= Sequence._cooldown;

                if (_count >= Sequence._amount)
                {
                    for (int i = 0; i < spawnedEnemy.Length; i++)
                        if (spawnedEnemy[i] != null || !spawnedEnemy[i].IsDead)
                            return -1f;

                    return 0f;
                }

                Enemy enemy = Sequence._factory.Get(Sequence._type);
                enemy.SpawnOn(_road.Spline);
                spawnedEnemy[_count] = enemy;

                _count += 1;
            }

            return -1f;
        }
    }
}