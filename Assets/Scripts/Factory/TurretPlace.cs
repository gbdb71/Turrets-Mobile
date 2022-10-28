using DG.Tweening;
using UnityEngine;

public class TurretPlace : MonoBehaviour
{
    [SerializeField] private float _moveTime = 0.5f;
    [SerializeField, NotNull] private Transform _contentTransform;

    public BaseTurret PlacedTurret;
    
    public bool HasTurret => PlacedTurret != null;

    public void Place(BaseTurret turret)
    {
        turret.transform.SetParent(_contentTransform, true);
        turret.transform.DOMove(_contentTransform.position, _moveTime);
        PlacedTurret = turret;
    }
}
