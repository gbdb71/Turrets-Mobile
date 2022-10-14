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

    [SerializeField] private TextMeshProUGUI enemyCount;

    [Inject] private GameLogic _game;

    private void Awake()
    {
        RoadScenario.OnWaveChanged += Show;
        _readyButton.onClick.AddListener(SetReady);

        enemyCount.text = "";
    }

    private void OnDestroy()
    {
        RoadScenario.OnWaveChanged -= Show;
    }

    public void Update()
    {
        //if (!_game.IsReady)
        //    return;

        //if (!content.gameObject.activeSelf)
        //{
        //    float progress = _game.ActiveScenario.Wave.WaveProgress;
        //    if (waveProgressFill.fillAmount != progress)
        //        waveProgressFill.DOFillAmount(progress, fillTime);
        //    return;
        //}
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
        UpdateEnemyCount();

        waveProgressFill.fillAmount = 0;
    }

    public void UpdateEnemyCount()
    {
        enemyCount.text = "Enemy Count = ";
    }

    private void Hide()
    {
        waveProgressFill.fillAmount = 0;
        content.gameObject.SetActive(false);
    }
}
