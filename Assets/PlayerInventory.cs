using System;
using UnityEngine;
using DG.Tweening;

public class PlayerInventory : MonoBehaviour
{
    [Range(.1f, 3f)]
    [SerializeField] private float _turrelTakeTime = 1.5f;
    [SerializeField] private Transform _turretSlot;

    [SerializeField] private Vector3 _placeOffset;

    private BaseTurret _turret;

    public bool HasTurret { get { return _turretSlot.childCount > 0; } }


    float _takeProgess = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (HasTurret) return;

        if (other.CompareTag("Turret") && other.TryGetComponent(out BaseTurret turret))
        {
            _turret = turret;
            _turret.IndicatorTransform.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_turret == null && HasTurret) return;

        if (_turret.gameObject == other.gameObject)
        {
            _takeProgess += Time.deltaTime;
            _turret.IndicatorFill.fillAmount = _takeProgess;

            if (_takeProgess >= 1f)
            {
                Take(_turret);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasTurret)
        {
            ResetProgress(_turret);
        }
    }

    public void Place()
    {
        if (_turret != null)
        {
            _turret.enabled = true;

            RaycastHit hit;

            if (Physics.Raycast(_turret.transform.position, -_turret.transform.up, out hit))
            {
                Vector3 targetPos = _turret.transform.position;
                targetPos.y = hit.point.y;
                targetPos += _placeOffset;

                _turret.transform.DOJump(targetPos, 1.5f, 1, .6f);
            }
            _turret.transform.SetParent(null);
            _turret = null;
        }
    }

    private void Take(BaseTurret turret)
    {
        turret.transform.SetParent(_turretSlot);

        turret.transform.DOMove(_turretSlot.transform.position, .25f).OnComplete(() =>
        {
            turret.transform.localPosition = new Vector3();
            turret.transform.localRotation = Quaternion.identity;
        });

        ResetProgress(turret);
    }

    private void ResetProgress(BaseTurret turret)
    {
        turret.IndicatorTransform.gameObject.SetActive(false);
        turret.enabled = false;

        _takeProgess = 0f;
    }
}
