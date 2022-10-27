using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class WaveView : BaseView
{
    [SerializeField] private SerializedDictionary<EnemyType, TextMeshProUGUI> _countText;

    private bool _isShowed = false;
    private Road _road;
    [Inject] private GameLogic _game;


    private void Awake()
    {
        _road = GetComponentInParent<Road>();
        transform.localScale = Vector3.zero;
    }

    protected override void UpdateLogic()
    {
        if (_road != null)
        {
            var enemiesCount = _game.ScenarioState.Wave.GetEnemiesCount(_road);

            if (!enemiesCount.Any(x => x.Value > 0))
            {
                if (_isShowed && transform.localScale != Vector3.zero)
                {
                    transform.DOScale(0f, .7f).From(1f).SetEase(Ease.InBack);
                }

                _isShowed = false;

                return;
            }

            if (!_isShowed && transform.localScale != Vector3.one)
            {
                transform.DOScale(1f, .7f).From(0f).SetEase(Ease.OutBack);
            }

            _isShowed = true;

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