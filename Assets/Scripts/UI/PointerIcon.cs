using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PointerIcon : MonoBehaviour {

    [SerializeField] private  Image _image;
    private bool _isShown = true;
    private Transform _transform;

    private void Awake() {
        _isShown = false;
        _transform = transform;
    }

    public void SetIconPosition(Vector3 position, Quaternion rotation) {
        _transform.position = position;
        _transform.rotation = rotation;
    }

    public void Show() {
        if (_isShown) return;
        _isShown = true;
       
        _image.DOFade(1f, .5f).SetEase(Ease.InOutBack);
        _transform.DOScale(1f, .4f).SetEase(Ease.InOutBack);
    }

    public void Hide() {
        if (!_isShown) return;
        _isShown = false;

        _image.DOFade(0f, .5f).SetEase(Ease.InOutBack);
        _transform.DOScale(0f, .4f).SetEase(Ease.InOutBack);
    }
}
