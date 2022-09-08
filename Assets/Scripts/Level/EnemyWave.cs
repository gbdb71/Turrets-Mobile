using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "TowerDefense/Level/Wave")]
public class EnemyWave : ScriptableObject
{
    [SerializeField, ReorderableList] private EnemySpawnSequence[] _spawnSequences = {};

    public State Begin(Game game) => new State(this, game);

    [System.Serializable]
    public struct State
    {

        private EnemyWave _wave;
        private EnemySpawnSequence.State _sequence;

        private int _index;
        private Game _game;


        public State(EnemyWave wave, Game game)
        {
            this._wave = wave;
            this._game = game;

            _index = 0;

            Debug.Assert(wave._spawnSequences.Length > 0, "Empty wave!");

            _sequence = wave._spawnSequences[0].Begin(_game);
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

                _sequence = _wave._spawnSequences[_index].Begin(_game);
                deltaTime = _sequence.Progress(deltaTime);
            }
            return -1f;
        }
    }
}
