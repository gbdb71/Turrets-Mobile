using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class DelayProgressBar : MonoBehaviour
{
    [Inject]
    private Game _game;
    private bool _enabled = false;

    [SerializeField] private Image fillImage;

    private void Awake()
    { 
        LevelScenario.OnWaveChanged += EnableProgressBar;
        DisableProgressBar();
    }

    public void Update()
    {
        if (_game.GameStared)
        {
            Debug.Log("Wave Lenght" + _game.ActiveScenario.Wave.WaveProgress);
        }

        if (!_enabled)
            return;

        if (_game.ActiveScenario.Wave.DelayProgress < _game.ActiveScenario.Wave.StartDelay)
        {
            float progress = _game.ActiveScenario.Wave.DelayProgress / _game.ActiveScenario.Wave.StartDelay;
            Debug.Log(progress);
            fillImage.fillAmount = 1 - progress;
        }
        else
            DisableProgressBar();
    }

    private void EnableProgressBar()
    {
        _enabled = true;
        fillImage.gameObject.SetActive(true);
    }

    private void DisableProgressBar()
    {
        _enabled = false;
        fillImage.gameObject.SetActive(false);
    }
}
