using UnityEngine;

[CreateAssetMenu(fileName = "Scenario", menuName = "TowerDefense/Level/Scenario")]
public class LevelScenario : ScriptableObject
{
    [SerializeField, ReorderableList] private EnemyWave[] waves = { };
    [SerializeField, Range(0, 10)] private int cycles = 1;
    public State Begin() => new State(this);

    [System.Serializable]
    public struct State
    {

        private EnemyWave.State _wave;
        private LevelScenario _scenario;

        private int _index, _cycle;

        public State(LevelScenario scenario)
        {
            this._scenario = scenario;

            _cycle = 0;
            _index = 0;

            Debug.Assert(scenario.waves.Length > 0, "Empty scenario!");

            _wave = scenario.waves[0].Begin();
        }

        public bool Progress()
        {
            float deltaTime = _wave.Progress(Time.deltaTime);
            while (deltaTime >= 0f)
            {
                if (++_index >= _scenario.waves.Length)
                {
                    if (++_cycle >= _scenario.cycles && _scenario.cycles > 0)
                    {
                        return false;
                    }

                    _index = 0;
                }
                _wave = _scenario.waves[_index].Begin();
                deltaTime = _wave.Progress(deltaTime);
            }
            return true;
        }
    }
}