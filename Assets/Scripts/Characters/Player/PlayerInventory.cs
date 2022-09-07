using UnityEngine;
using DG.Tweening;

public class PlayerInventory : MonoBehaviour
{
    [Label("Turrets", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.1f, .3f)] private float _turrelTakeTime = 1.5f;
    [SerializeField, NotNull] private Transform _turretSlot;
    [SerializeField] private GameObject _upgradeEffect;

    [Label("Ammunition", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 100)] private int _maxAmmoCount;
    [SerializeField, NotNull] private Transform _ammoSlot;
    [SerializeField] private Vector3 _placeOffset;

    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;
    private float _takeProgess = 0f;
    private int _ammoCount;

    public BaseTurret NearTurret => _nearTurret;
    public BaseTurret TakedTurret => _takedTurret;
    public bool HasTurret { get { return _turretSlot.childCount > 0 && TakedTurret != null; } }
    public int AmmoCount { set { _ammoCount = Mathf.Clamp(value, 1, 99); } }
    public bool CanUpgrade { get { return HasTurret && NearTurret != null && NearTurret.NextGrade == TakedTurret.NextGrade; } }

    private void OnTriggerEnter(Collider other)
    {
        if (_nearTurret != null)
            ResetProgress(_nearTurret);

        if (other.CompareTag("Turret") && other.TryGetComponent(out BaseTurret turret))
        {
            _nearTurret = turret;

            if (HasTurret)
                return;

            _nearTurret.IndicatorTransform.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_nearTurret == null || HasTurret) return;

        if (_nearTurret.gameObject == other.gameObject)
        {
            _takeProgess += Time.deltaTime;
            _nearTurret.IndicatorFill.fillAmount = _takeProgess;

            if (_takeProgess >= 1f)
            {
                Take(_nearTurret);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_nearTurret != null)
            ResetProgress(_nearTurret);

        _nearTurret = null;
    }

    public void Place()
    {
        if (_takedTurret != null)
        {

            RaycastHit hit;

            if (Physics.Raycast(_takedTurret.transform.position, -_takedTurret.transform.up, out hit))
            {
                Vector3 targetPos = _takedTurret.transform.position;
                targetPos.y = hit.point.y;
                targetPos += _placeOffset;

                _takedTurret.transform.DOJump(targetPos, 1f, 1, .6f).OnComplete(() =>
                {
                    _takedTurret.enabled = true;
                    _takedTurret.transform.SetParent(null);
                    _takedTurret = null;
                });
            }

        }
    }

    private void Take(BaseTurret turret)
    {
        if (turret == _nearTurret)
            _nearTurret = null;

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

    public void Upgrade()
    {
        if (CanUpgrade)
        {
            GameObject near = _nearTurret.gameObject;

            _takedTurret.transform.DOJump(near.transform.position, 1f, 1, .25f).OnComplete(() =>
            {
                if (_upgradeEffect != null)
                {
                    _upgradeEffect.transform.position = _nearTurret.transform.position;
                    _upgradeEffect.gameObject.SetActive(true);

                }


                BaseTurret newTurret = Instantiate(_takedTurret.NextGrade, near.transform.position, near.transform.rotation, null);
                Destroy(_takedTurret.gameObject);
                Destroy(near);

                _takedTurret.transform.SetParent(null);
                _takedTurret = null;
            });
        }
    }
}
