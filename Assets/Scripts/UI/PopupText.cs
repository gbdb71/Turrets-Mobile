using ToolBox.Pools;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class PopupText : MonoBehaviour
{
    public enum DurationType
    {
        Default = 1,
        Long = 2
    }

    [SerializeField, Range(.2f, 2f)] private float _hideDuration = .7f;
    [SerializeField, Range(.2f, 5f)] private float _yMovement;

    private TextMeshPro _text;
    private RectTransform _transform;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _text = GetComponent<TextMeshPro>();
    }

    public void SetColor(Color color)
    {
        _text.color = color;
    }

    public void SetText(string value, DurationType durationType = DurationType.Default)
    {
        _text.text = value;

        float duration = _hideDuration * (int)durationType;

        _transform.DOLocalMoveY(transform.localPosition.y + _yMovement, duration * .6f).OnComplete(() =>
        {
            _transform.SetParent(null);
            gameObject.Release();
        });

        _text.DOFade(0f, duration * .6f).From(1f).SetEase(Ease.OutFlash).SetDelay(0.2f);
    }
}
