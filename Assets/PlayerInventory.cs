using UnityEngine;
using DG.Tweening;

public class PlayerInventory : MonoBehaviour
{
    [Range(.1f, 3f)]
    [SerializeField] private float _turrelTakeTime = 1.5f;
    [SerializeField] private Transform _turretSlot;

    [SerializeField] private Vector3 _placeOffset;

    private BaseTurret _tempTurret;
    private BaseTurret _takedTurret;

    public bool HasTurret { get { return _turretSlot.childCount > 0; } }


    float _takeProgess = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (HasTurret) return;

        if (other.CompareTag("Turret") && other.TryGetComponent(out BaseTurret turret))
        {
            _tempTurret = turret;
            _tempTurret.IndicatorTransform.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_tempTurret == null || HasTurret) return;

        if (_tempTurret.gameObject == other.gameObject)
        {
            _takeProgess += Time.deltaTime;
            _tempTurret.IndicatorFill.fillAmount = _takeProgess;

            if (_takeProgess >= 1f)
            {
                Take(_tempTurret);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_tempTurret != null)
            ResetProgress(_tempTurret);

        _tempTurret = null;
    }

    public void Place()
    {
        if (_takedTurret != null)
        {
            _takedTurret.enabled = true;

            RaycastHit hit;

            if (Physics.Raycast(_takedTurret.transform.position, -_takedTurret.transform.up, out hit))
            {
                Vector3 targetPos = _takedTurret.transform.position;
                targetPos.y = hit.point.y;
                targetPos += _placeOffset;

                _takedTurret.transform.DOJump(targetPos, 1f, 1, .6f);
            }

            _takedTurret.transform.SetParent(null);
            _takedTurret = null;
        }
    }

    private void Take(BaseTurret turret)
    {
        _takedTurret = turret;
        _takedTurret.transform.SetParent(_turretSlot);

        _takedTurret.transform.DOMove(_turretSlot.transform.position, .25f).OnComplete(() =>
        {
            turret.transform.localPosition = new Vector3();
            turret.transform.localRotation = Quaternion.identity;
        });

        _takedTurret.enabled = false;

        ResetProgress(_takedTurret);
    }

    private void ResetProgress(BaseTurret turret)
    {
        turret.IndicatorTransform.gameObject.SetActive(false);
        _takeProgess = 0f;
    }
}
