using System.Linq;
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
        if (_road != null)
        {
            var enemiesCount = _game.ScenarioState.Wave.GetEnemiesCount(_road);

            if (!enemiesCount.Any(x => x.Value > 0))
            {
                _canvasGroup.alpha = 0;
                return;
            }

            _canvasGroup.alpha = 1;

            foreach (var enemy in enemiesCount)
            {
                if (_countText.ContainsKey(enemy.Key))
                {
                    TextMeshProUGUI text = _countText[enemy.Key];

                    if (enemy.Value == 0)
                    {
                        text.transform.parent.gameObject.SetActive(false);
                        continue;
                    }

                    text.transform.parent.gameObject.SetActive(true);
                    _countText[enemy.Key].text = enemy.Value.ToString();
                }
            }
        }
    }
}
