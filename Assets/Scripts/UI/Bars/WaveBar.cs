using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;

public class WaveBar : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Button _readyButton;
    [SerializeField] private TextMeshProUGUI _titleText;

    [Inject] private GameLogic _game;

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
        if(!_game.IsReady && _game.ScenarioState != null)
        {
            _titleText.text = $"Wave {_game.ScenarioState.WaveIndex + 1}/{_game.ScenarioState.WavesCount}";
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
    }

    private void Hide()
    {
        content.gameObject.SetActive(false);
    }
}
