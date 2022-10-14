using UnityEngine;

[System.Serializable]
public class EnemyWave 
{
    [SerializeField, ReorderableList] private EnemySpawnSequence[] _spawnSequences = { };

    public State Begin(Road road) => new State(this, road);

    public int GetEnemyCount()
    {
        int spawnCounts = 0;

        for (int i = 0; i < _spawnSequences.Length; i++)
        {
            int waveEnemyCount = _spawnSequences[i].Amount;
            spawnCounts += waveEnemyCount;
        }

        return spawnCounts;
    }

    [System.Serializable]
    public class State
    {
        private EnemyWave _wave;
        private EnemySpawnSequence.State _sequence;
        private Road _road;

        private int _index;
        private float delayTimer;
        public float DelayProgress => delayTimer;
        public float WaveProgress
        {
            get
            {
                float prevProgress = (float)_index / _wave._spawnSequences.Length;
                float progress = prevProgress + (_sequence.SequenceProgress / _wave._spawnSequences.Length);
                return progress;
            }
        }

        public State(EnemyWave wave, Road road)
        {
            _wave = wave;
            _road = road;

            _index = 0;
            delayTimer = 0;

            Debug.Assert(wave._spawnSequences.Length > 0, "Empty wave!");

            _sequence = wave._spawnSequences[0].Begin(_road);
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

                _sequence = _wave._spawnSequences[_index].Begin(_road);
                deltaTime = _sequence.Progress(deltaTime);
            }

            return -1f;
        }
    }
}
