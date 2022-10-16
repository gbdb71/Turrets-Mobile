using System;
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
        public EnemyWave.State Wave => _wave;
        public int WaveIndex => _index;
        public int WaveCount => _scenario._waves.Length;
        public int WaveEnemyCount;

        public State(LevelScenario scenario)
        {
            _scenario = scenario;

            _index = 0;

            _wave = _scenario._waves[0].Begin();
            WaveEnemyCount = _scenario._waves[_index].GetEnemyCount();
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
                WaveEnemyCount = _scenario._waves[_index].GetEnemyCount();
                deltaTime = -1;

                OnWaveChanged?.Invoke(_index);
            }

            return true;
        }
    }
}