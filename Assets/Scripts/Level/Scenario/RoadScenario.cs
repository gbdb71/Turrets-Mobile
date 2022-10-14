using System;
using UnityEngine;

[Serializable]
public class RoadScenario 
{
    [SerializeField, ReorderableList] private EnemyWave[] _waves = { };

    public static event Action<int> OnWaveChanged;
    public int WavesCount => _waves.Length;
    public State Begin(Road road)
    {
        State state = new State(this, road);
        return state;
    }

    [Serializable]
    public class State
    {
        private EnemyWave.State _wave;
        private RoadScenario _scenario;
        private int _index;
        private Road _road;

        public EnemyWave.State Wave => _wave;
        public int WaveIndex => _index;
        public int WaveCount => _scenario._waves.Length;
        public int WaveEnemyCount;

        public State(RoadScenario scenario, Road road)
        {
            _scenario = scenario;
            _road = road;

            _index = 0;

            _wave = _scenario._waves[0].Begin(_road);
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

                _wave = _scenario._waves[_index].Begin(_road);
                WaveEnemyCount = _scenario._waves[_index].GetEnemyCount();
                deltaTime = -1;

                OnWaveChanged?.Invoke(_index);
            }

            return true;
        }
    }
}