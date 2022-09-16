using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;

public class WaveBar : MonoBehaviour
{
    [Inject]
    private Game _game;
    private bool _enabled = false;

    [SerializeField] private Image fillImage;
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Awake()
    { 
        LevelScenario.OnWaveChanged += EnableProgressBar;
        DisableProgressBar();
    }

    public void Update()
    {
        if (!_enabled)
            return;

        if (_game.ActiveScenario.Wave.DelayProgress < _game.ActiveScenario.Wave.StartDelay)
        {
            float progress = _game.ActiveScenario.Wave.DelayProgress / _game.ActiveScenario.Wave.StartDelay;
            fillImage.fillAmount = 1 - progress;
        }
        else
            DisableProgressBar();
    }

    private void EnableProgressBar()
    {
        _enabled = true;
        content.gameObject.SetActive(true);
        titleText.text = "Wave " + (_game.ActiveScenario.WaveIndex + 1).ToString();
    }

    private void DisableProgressBar()
    {
        _enabled = false;
        content.gameObject.SetActive(false);
    }
}
