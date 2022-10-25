using System;
using UnityEngine;
using Zenject;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private LevelScenario _scenario;

    [Inject] private Data _data;
    [Inject] private LevelManager _levelManager;
    private Headquarters _headquarters;

    public LevelScenario.State ScenarioState { get; private set; }
    public bool GameFinished { get; private set; } = false;
    public bool IsWin { get; private set; } = false;
    public bool IsReady { get; private set; } = false;

    public Headquarters Headquarters => _headquarters;
    public Data Data => _data;

    public static event Action OnGameFinished;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }


    private void Start()
    {
        ScenarioState = _scenario.Begin();
    }

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

            if (!ScenarioState.Progress())
            {
                IsWin = true;

                _levelManager.NextLevel();

                GameFinished = true;
                OnGameFinished?.Invoke();
            }
        }
    }

    public void SetHeadquarters(Headquarters headquarters)
    {
        _headquarters = headquarters;
        _data.User.SetCurrencyValue(CurrencyType.Construction, headquarters.StartCurrency);
    }

    public void SetReady(bool ready)
    {
        IsReady = ready;
    }
}
