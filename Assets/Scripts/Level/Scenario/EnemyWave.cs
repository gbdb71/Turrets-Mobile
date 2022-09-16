using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Wave", menuName = "TowerDefense/Level/Wave")]
public class EnemyWave : ScriptableObject
{
    [SerializeField, ReorderableList] private EnemySpawnSequence[] _spawnSequences = { };
    [Inject]
    private Game _game;

    [SerializeField] private float startDelay;

    public State Begin() => new State(this);

    [System.Serializable]
    public struct State
    {
        private EnemyWave _wave;
        private EnemySpawnSequence.State _sequence;

        private int _index;
        private float delayTimer;
        public float DelayProgress => delayTimer;
        public float StartDelay => _wave.startDelay;
        public float WaveProgress
        {
            get 
            {
                float prevProgress = (float)_index / _wave._spawnSequences.Length;
                float progress = prevProgress + (_sequence.SequenceProgress / _wave._spawnSequences.Length);
                return progress;
            }
        }

        public State(EnemyWave wave)
        {
            this._wave = wave;
            _index = 0;
            delayTimer = 0;

            Debug.Assert(wave._spawnSequences.Length > 0, "Empty wave!");

            _sequence = wave._spawnSequences[0].Begin(_wave._game);
        }

        public float Progress(float deltaTime)
        {
            if (delayTimer < _wave.startDelay)
            {
                delayTimer += deltaTime;
            }

            if (delayTimer >= _wave.startDelay)
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
                    Debug.Log("Sequence" + _sequence);
                }
            }
            return -1f;
        }
    }
}
