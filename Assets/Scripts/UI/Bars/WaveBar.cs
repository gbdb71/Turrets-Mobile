using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WaveBar : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button _readyButton;

    [SerializeField] private Image waveProgressFill;

    [SerializeField] private float fillTime = 0.25f;

    [Inject] private Game _game;

    private void Awake()
    {
        LevelScenario.OnWaveChanged += Show;
        _readyButton.onClick.AddListener(SetReady);
    }

    private void OnDestroy()
    {
        LevelScenario.OnWaveChanged -= Show;
    }

    public void Update()
    {
        if (!_game.GameStared)
            return;

        if (!content.gameObject.activeSelf)
        {
            float progress = _game.ActiveScenario.Wave.WaveProgress;
            if (waveProgressFill.fillAmount != progress)
                waveProgressFill.DOFillAmount(progress, fillTime);
            return;
        }
    }

    private void SetReady()
    {
        _game.SetReady(true);
        Hide();
    }

    private void Show(int index)
    {
        _game.SetReady(false);

        content.gameObject.SetActive(true);
        titleText.text = "Wave " + (_game.ActiveScenario.WaveIndex + 1).ToString();

        waveProgressFill.fillAmount = 0;
    }

    private void Hide()
    {
        waveProgressFill.fillAmount = 0;
        content.gameObject.SetActive(false);
    }
}
