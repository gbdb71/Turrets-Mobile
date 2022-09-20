using System.Collections.Generic;
using UnityEngine;
using Zenject;

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

    public State Begin(Game game) => new State(this, game);


    [System.Serializable]
    public struct State
    {
        private EnemySpawnSequence _sequence;

        private int _count;
        private float _cooldown;
        private Game _game;
        private Enemy[] spawnedEnemy;

        public float SequenceProgress => (float)_count / _sequence._amount;

        public State(EnemySpawnSequence sequence, Game game)
        {
            this._sequence = sequence;
            this._game = game;

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

                spawnedEnemy[_count] = _game.SpawnEnemy(_sequence._factory, _sequence._type);

                _count += 1;
            }

            return -1f;
        }
    }
}
