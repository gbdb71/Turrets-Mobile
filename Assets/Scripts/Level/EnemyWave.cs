﻿using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Wave", menuName = "TowerDefense/Level/Wave")]
public class EnemyWave : ScriptableObject
{
    [SerializeField, ReorderableList] private EnemySpawnSequence[] _spawnSequences = {};
    [Inject] private Game _game;

    public State Begin() => new State(this);

    [System.Serializable]
    public struct State
    {

        private EnemyWave _wave;
        private EnemySpawnSequence.State _sequence;

        private int _index;

        public State(EnemyWave wave)
        {
            this._wave = wave;
            _index = 0;

            Debug.Assert(wave._spawnSequences.Length > 0, "Empty wave!");

            _sequence = wave._spawnSequences[0].Begin(_wave._game);
        }

        public float Progress(float deltaTime)
        {
            deltaTime = _sequence.Progress(deltaTime);
            while (deltaTime >= 0f)
            {
                if (++_index >= _wave._spawnSequences.Length)
                {
                    return deltaTime;
                }

                _sequence = _wave._spawnSequences[_index].Begin(_wave._game);
                deltaTime = _sequence.Progress(deltaTime);
            }
            return -1f;
        }
    }
}