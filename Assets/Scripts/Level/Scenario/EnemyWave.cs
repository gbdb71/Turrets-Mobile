using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    [System.Serializable]
    public class RoadSequence
    {
        public Road Road;
        [LabelByChild("_type")] public EnemySpawnSequence[] EnemySequences;

        [HideInInspector] public int Index;
    }

    [SerializeField, ReorderableList] private RoadSequence[] _roads;

    public State Begin() => new State(this);

    public int GetEnemyCount()
    {
        int spawnCounts = 0;

        for (int i = 0; i < _roads.Length; i++)
        {
            RoadSequence sequence = _roads[i];

            if (sequence.Road != null && sequence.EnemySequences.Length > 0)
            {
                for (int x = 0; x < sequence.EnemySequences.Length; x++)
                {
                    spawnCounts += sequence.EnemySequences[x].Amount;
                }
            }
        }

        return spawnCounts;
    }

    [System.Serializable]
    public class State
    {
        private EnemySpawnSequence.State[] _sequences;
        private EnemyWave _wave;
        public float WaveProgress { get; private set; }

        public State(EnemyWave wave)
        {
            _wave = wave;
            _sequences = new EnemySpawnSequence.State[_wave._roads.Length];

            for (int i = 0; i < _wave._roads.Length; i++)
            {
                _sequences[i] = _wave._roads[i].EnemySequences[0].Begin(_wave._roads[i].Road);
            }
        }

        public float Progress(float deltaTime)
        {
            bool allFinished = true;
            float roadsProgress = 0f;

            float scenarioDelta = deltaTime;

            for (int i = 0; i < _wave._roads.Length; i++)
            {
                RoadSequence sequence = _wave._roads[i];

                deltaTime = _sequences[i].Progress(scenarioDelta);

                float prevProgress = (float)sequence.Index / sequence.EnemySequences.Length;
                roadsProgress += prevProgress + (_sequences[i].SequenceProgress / sequence.EnemySequences.Length);

                while (deltaTime >= 0f)
                {
                    if (++sequence.Index < sequence.EnemySequences.Length)
                    {
                        allFinished = false;
                        _sequences[i] = sequence.EnemySequences[sequence.Index].Begin(sequence.Road);
                        deltaTime = _sequences[i].Progress(deltaTime);
                    }
                    else break;
                }
            }

            WaveProgress = roadsProgress / _wave._roads.Length;

            return allFinished ? deltaTime : -1f;
        }
    }
}
