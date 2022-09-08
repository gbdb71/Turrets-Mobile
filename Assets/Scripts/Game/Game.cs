using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    //TODO : remove 
    [SerializeField] private LevelData _startLevel;
    [SerializeField] private MapGenerator _mapGenerator;

    private List<Enemy> _enemies = new List<Enemy>();
    private LevelScenario.State _activeScenario;


    public bool GameStared { get; private set; } = false;
    public LevelData CurrentLevel { get; private set; }

    private List<Vector3> _pathPoints;

    private void Awake()
    {
        CurrentLevel = _startLevel;

        if (_mapGenerator != null)
            _mapGenerator.OnMapGenerated += OnMapGenerated;

        _activeScenario = CurrentLevel.LevelScenario.Begin(this);
    }

    private void Update()
    {
        if (GameStared)
            _activeScenario.Progress();
    }

    private void OnMapGenerated()
    {
        GameStared = true;

        _pathPoints = _mapGenerator.PathGenerator.PathCells.FromVec2Int();
        _pathPoints = _pathPoints.Select((x) =>
        {
            x *= _mapGenerator.CellSize;
            x.y = .5f;
            return x;
        }).ToList();
    }

    public void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(_pathPoints);

        _enemies.Add(enemy);
    }
}
