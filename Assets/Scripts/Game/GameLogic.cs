using System;
using UnityEngine;
using Zenject;

public class GameLogic : MonoBehaviour
{
    [Inject] private Data _data;
    [Inject] private LevelManager _levelManager;
    private Headquarters _headquarters;

    public bool GameFinished { get; private set; } = false;
    public bool IsWin { get; private set; } = false;
    public bool IsReady { get; private set; } = false;

    public Headquarters Headquarters => _headquarters;
    public Data Data => _data;

    public static event Action OnGameFinished;

    private void Update()
    {
        if (_headquarters == null)
            return;

        if (!GameFinished && IsReady)
        {
            if (Headquarters.IsDead)
            {
                GameFinished = true;
                OnGameFinished?.Invoke();
                return;
            }

            for (int i = 0; i < Road.Instances.Count; i++)
            {
                Road road = Road.Instances[i];

                bool allFinished = true;

                if (road.ScenarioState.Progress())
                {
                    allFinished = false;
                }

                if (allFinished)
                {
                    IsWin = true;

                    _levelManager.NextLevel();

                    GameFinished = true;
                    OnGameFinished?.Invoke();
                }
            }
        }
    }

    public void SetHeadquarters(Headquarters headquarters)
    {
        _headquarters = headquarters;
        _data.User.TryAddCurrency(CurrencyType.Construction, headquarters.StartCurrency);
    }

    public void SetReady(bool ready)
    {
        IsReady = ready;
    }
}
