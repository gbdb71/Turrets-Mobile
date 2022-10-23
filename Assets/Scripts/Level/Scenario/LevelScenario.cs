using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelScenario 
{
    [SerializeField, ReorderableList] private EnemyWave[] _waves = { };

    public static event Action<int> OnWaveChanged;
    public int WavesCount => _waves.Length;
    public State Begin()
    {
        State state = new State(this);
        return state;
    }

    [Serializable]
    public class State
    {
        private EnemyWave.State _wave;
        private LevelScenario _scenario;
        private int _index;

        public int WaveIndex => _index;
        public EnemyWave.State Wave => _wave;

        public State(LevelScenario scenario)
        {
            _scenario = scenario;

            _index = 0;

            _wave = _scenario._waves[0].Begin();
            OnWaveChanged?.Invoke(0);
        }

        public bool Progress()
        {
            float deltaTime = _wave.Progress(Time.deltaTime);

            while (deltaTime >= 0f)
            {
                if (++_index >= _scenario._waves.Length)
                {
                    return false;
                }

                _wave = _scenario._waves[_index].Begin();
                deltaTime = -1;

                OnWaveChanged?.Invoke(_index);
            }

            return true;
        }
    }
}