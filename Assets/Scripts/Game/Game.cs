using System;
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
    public LevelScenario.State ActiveScenario => _activeScenario;

    public bool GameStared { get; private set; } = false;
    public bool GameFinished { get; private set; } = false;
    public LevelData CurrentLevel { get; private set; }
    private List<Vector3> _pathPoints = new List<Vector3>();

    public static event Action OnGameFinished;

    private void Awake()
    {
        CurrentLevel = _startLevel;

        if (_map != null)
            _map.OnMapGenerated += OnMapGenerated;
    }

    private void Update()
    {
        if (GameStared && !GameFinished)
        {
            if (_activeScenario.Progress() == false)
            {
                GameFinished = true;
                OnGameFinished?.Invoke();
            }
        }
    }

    private void OnMapGenerated()
    {
        _activeScenario = CurrentLevel.LevelScenario.Begin();

        GameStared = true;
        _pathPoints.Clear();

        for (int i = 0; i < _map.PathGenerator.PathCells.Count; i++)
        {
            Vector2Int posXZ = _map.PathGenerator.PathCells[i];

            Vector3 worldPos = _map.MapGrid.GetWorldPosition(posXZ.x, posXZ.y);
            worldPos.y = .5f;

            _pathPoints.Add(worldPos);
        }
    }

    public Enemy SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(_pathPoints);

        _enemies.Add(enemy);
        return enemy;
    }
}
