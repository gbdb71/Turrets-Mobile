using System;
using DG.Tweening;
using UnityEngine;

public class TurretPlace : MonoBehaviour
{
    [SerializeField] private float _moveTime = 0.5f;
    [SerializeField, NotNull] private Transform _contentTransform;

    public BaseTurret PlacedTurret;
    public TurretCanvas Canvas { get; private set; }
    public bool HasTurret => PlacedTurret != null;

    private void Awake()
    {
        Canvas = GetComponentInChildren<TurretCanvas>();
    }

    public void Place(BaseTurret turret)
    {
        turret.transform.SetParent(_contentTransform, true);
        turret.transform.DOMove(_contentTransform.position, _moveTime);
        
        PlacedTurret = turret;
        
        if(PlacedTurret.Canvas != null && PlacedTurret.Canvas.Range != null)
            PlacedTurret.Canvas.Range.fillAmount = 0;
    }
}
