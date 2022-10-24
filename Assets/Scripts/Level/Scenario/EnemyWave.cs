using System.Collections.Generic;
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

        public Dictionary<EnemyType, int> GetEnemiesCount(Road road)
        {
            Dictionary<EnemyType, int> count = new Dictionary<EnemyType, int>
            {
                {EnemyType.Small, 0},
                {EnemyType.Medium, 0},
                {EnemyType.Large, 0},
            };
            
            for (int i = 0; i < _wave._roads.Length; i++)
            {
                if (road == _wave._roads[i].Road)
                {
                    for (int j = 0; j < _wave._roads[i].EnemySequences.Length; j++)
                    {
                        EnemySpawnSequence sequence = _wave._roads[i].EnemySequences[j];

                        count[sequence.Type] += sequence.Amount;
                        count[sequence.Type] -= _sequences[j].Count;
                    }
                }
            }

            return count;
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

                if (deltaTime < 0)
                {
                    allFinished = false;
                }

                while (deltaTime >= 0f)
                {
                    if (++sequence.Index < sequence.EnemySequences.Length)
                    {
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
