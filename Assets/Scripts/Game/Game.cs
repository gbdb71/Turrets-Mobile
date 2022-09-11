using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Game : MonoBehaviour
{
    //TODO : remove 
    [SerializeField] private LevelData _startLevel;

    [Inject] private Map _map;
    private List<Enemy> _enemies = new List<Enemy>();
    private LevelScenario.State _activeScenario;


    public bool GameStared { get; private set; } = false;
    public LevelData CurrentLevel { get; private set; }
    private List<Vector3> _pathPoints = new List<Vector3>();


    private void Awake()
    {
        CurrentLevel = _startLevel;

        if (_map != null)
            _map.OnMapGenerated += OnMapGenerated;

        _activeScenario = CurrentLevel.LevelScenario.Begin();
    }

    private void Update()
    {
        if (GameStared)
            _activeScenario.Progress();
    }

    private void OnMapGenerated()
    {
        GameStared = true;

        _pathPoints.Clear();

        for (int i = 0; i < _map.PathGenerator.PathCells.Count; i++)
        {
            Vector2Int pos = _map.PathGenerator.PathCells[i];

            _pathPoints.Add(_map.MapGrid.GetWorldPosition(pos.x, pos.y));
        }
    }

    public void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(_pathPoints);

        _enemies.Add(enemy);
    }
}
