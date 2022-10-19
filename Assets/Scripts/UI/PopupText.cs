using ToolBox.Pools;
using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class PopupText : MonoBehaviour
{
    [SerializeField, Range(.2f, 2f)] private float _hideDuration = .5f;
    [SerializeField, Range(.2f, 5f)] private float _yMovement;
    [SerializeField, Range(.2f, 2f)] private float _moveDuration = 1f;

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

    public void SetText(string value)
    {
        _text.text = value; 

        _transform.DOLocalMoveY(transform.localPosition.y + _yMovement, _moveDuration).OnComplete(() =>
        {
            _transform.SetParent(null);
            gameObject.Release();
        });

        _text.DOFade(0f, _hideDuration);
    }
}
