using UnityEngine;
using Zenject;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WaveBar : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private Button _readyButton;

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
        if (!_game.IsReady)
            return;
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
