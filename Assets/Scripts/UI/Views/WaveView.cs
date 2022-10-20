using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class WaveView : BaseView
{
    [SerializeField] private SerializedDictionary<EnemyType, TextMeshProUGUI> _countText;

    private CanvasGroup _canvasGroup;
    private Road _road;
    [Inject] private GameLogic _game;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _road = GetComponentInParent<Road>();
    }

    protected override void UpdateLogic()
    {
        if(_game.IsReady)
        {
            _canvasGroup.alpha = 0;
            return;
        }

        _canvasGroup.alpha = 1;

        if (_road != null)
        {
            var enemiesCount = _game.ScenarioState.Wave.GetEnemiesCount(_road);

            foreach (var enemy in enemiesCount)
            {
                if(_countText.ContainsKey(enemy.Key))
                {
                    _countText[enemy.Key].text = enemy.Value.ToString();
                }
            }
        }
    }
}
