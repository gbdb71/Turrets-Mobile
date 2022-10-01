using DG.Tweening;
using UnityEngine;

public class FactoryPlate : MonoBehaviour
{
    [SerializeField] private float _moveTime = 0.5f;
    [SerializeField, NotNull] private Transform _contentTransform;

    public Transform Content => _contentTransform.GetChild(0);

    public void Place(Transform gameObject)
    {
        gameObject.transform.SetParent(_contentTransform, true);
        gameObject.transform.DOMove(_contentTransform.position, _moveTime);
    }

    public bool CanPlace()
    {
        return _contentTransform.childCount == 0;
    }
}
