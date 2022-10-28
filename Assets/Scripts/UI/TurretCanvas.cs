using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class TurretCanvas : MonoBehaviour
{
    [SerializeField] private Image _rangeImage;
    [SerializeField] private Image _fillImage;
    [SerializeField] private List<Image> _stars;

    public Image Range => _rangeImage;
    public Image Fill => _fillImage;

    private int _starIndex = 0;

    private void Start()
    {
        for (int i = 0; i < _stars.Count; i++)
        {
            _stars[i].gameObject.SetActive(false);
        }
    }

    public void AddStar()
    {
        if(_starIndex >= _stars.Count)
            return;

        Image star = _stars[_starIndex];
        star.gameObject.SetActive(true);
        star.transform.DOScale(1f, .5f).From(0f).SetEase(Ease.OutBack);

        _starIndex++;
    }
}
