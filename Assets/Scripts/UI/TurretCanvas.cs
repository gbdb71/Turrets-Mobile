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

    private Vector3 _rangeSize;
    private int _starIndex = 0;

    private void Start()
    {
        SetStarsEnabled(false);
    }

    public void AddStar()
    {
        if (_starIndex >= _stars.Count)
            return;

        Image star = _stars[_starIndex];
        star.gameObject.SetActive(true);
        star.transform.DOScale(1f, .5f).From(0f).SetEase(Ease.OutBack);

        _starIndex++;
    }

    public void InitRange(Vector3 size, Color color)
    {
        _rangeSize = size;
        Range.color = color;
    }

    public void SetRangeActive(bool enabled)
    {
        Range.gameObject.SetActive(enabled);
        Range.transform.DOScale(_rangeSize, .3f)
            .From(Vector3.zero).SetEase(Ease.OutBack);
    }

    public void SetStarsEnabled(bool enabled)
    {
        for (int i = 0; i < _stars.Count; i++)
        {
            if (_stars[i] != null)
            {
                if (enabled && i >= _starIndex)
                    continue;

                _stars[i].gameObject.SetActive(enabled);
            }
        }
    }
}