using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WaveBar : MonoBehaviour
{
    [Inject]
    private Game _game;
    private bool _enabled = false;

    [SerializeField] private Image waveCooldownFill;
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private Image waveProgressFill;

    [SerializeField] private float fillTime = 0.25f;

    private void Awake()
    {
        LevelScenario.OnWaveChanged += EnableProgressBar;
        DisableProgressBar();
    }

    public void Update()
    {
        if (!_game.GameStared)
            return;

        if (!_enabled)
        {
            float progress = _game.ActiveScenario.Wave.WaveProgress;
            if (waveProgressFill.fillAmount != progress)
                waveProgressFill.DOFillAmount(progress, fillTime);
            return;
        }

        if (_game.ActiveScenario.Wave.DelayProgress < _game.ActiveScenario.Wave.StartDelay)
        {
            float progress = _game.ActiveScenario.Wave.DelayProgress / _game.ActiveScenario.Wave.StartDelay;
            waveCooldownFill.fillAmount = 1 - progress;
        }
        else
            DisableProgressBar();
    }

    private void EnableProgressBar()
    {
        _enabled = true;
        waveProgressFill.fillAmount = 0;
        content.gameObject.SetActive(true);
        titleText.text = "Wave " + (_game.ActiveScenario.WaveIndex + 1).ToString();
    }

    private void DisableProgressBar()
    {
        _enabled = false;
        waveProgressFill.fillAmount = 0;
        content.gameObject.SetActive(false);
    }
}
