using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class Game : MonoBehaviour
{
    [Inject] private LevelManager _levelManager;
    [Inject] private Map _map;
    [Inject] private Data _data;
    private Headquarters _headquarters;

    private List<Vector3> _pathPoints = new List<Vector3>();
    private List<Enemy> _enemies = new List<Enemy>();
    private LevelScenario.State _activeScenario;

    public bool GameStared { get; private set; } = false;
    public bool GameFinished { get; private set; } = false;
    public bool IsReady { get; private set; } = false;    

    public LevelScenario.State ActiveScenario => _activeScenario;
    public Headquarters Headquarters => _headquarters;
    public Data Data => _data;

    public static event Action OnGameFinished;

    private void Awake()
    {
        Map.OnMapGenerated += OnMapGenerated;
    }
    private void OnDestroy()
    {
        Map.OnMapGenerated -= OnMapGenerated;
    }
    private void Update()
    {
        if (GameStared && !GameFinished)
        {
            if (!_headquarters.IsDead && _activeScenario.Progress() == false)
            {
                GameFinished = true;
                _levelManager.NextLevel();

                OnGameFinished?.Invoke();            

                _data.User.SetCurrencyValue(CurrencyType.Construction, 0);
            }
        }
    }

    public void SetHeadquarters(Headquarters headquarters)
    {
        _headquarters = headquarters;
    }
    public void SetReady(bool ready)
    {
        IsReady = ready;
    }

    private void OnMapGenerated()
    {
        _activeScenario = _levelManager.CurrentLevel.LevelScenario.Begin();

        GameStared = true;
        _pathPoints.Clear();

        for (int i = 0; i < _map.PathGenerator.PathCells.Count; i++)
        {
            Vector2Int posXZ = _map.PathGenerator.PathCells[i];

            Vector3 worldPos = _map.MapGrid.GetWorldPosition(posXZ.x, posXZ.y);
            worldPos.y = .5f;

            _pathPoints.Add(worldPos);
        }

        _data.User.SetCurrencyValue(CurrencyType.Construction, _levelManager.CurrentLevel.ConstructionCurrency);
    }
    public Enemy SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(_pathPoints);

        _enemies.Add(enemy);
        return enemy;
    }
}
