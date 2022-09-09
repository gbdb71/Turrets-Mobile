using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Label("Turrets", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private Transform _turretSlot;
    [SerializeField] private GameObject _upgradeEffect;

    [Label("Ammunition", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Vector3 _placeOffset;

    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;
    private float _takeProgess = 0f;
    [SerializeField] private float _putProgess = 0f;

    [SerializeField] private List<Ammunition> _ammunitionInBackpack = new List<Ammunition>();
    private float _distanceBetweenObjects = 0.25f;
    [SerializeField] private float _itemMoveTime = 0.5f;
    [SerializeField] private float _itemRotationTime = 0.25f;
    private int _ammoCount;

    public BaseTurret NearTurret => _nearTurret;
    public BaseTurret TakedTurret => _takedTurret;
    public Transform backpackPoint;
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

        if (other.CompareTag("Ammunition") && other.TryGetComponent(out Ammunition ammunition))
        {
            if (_ammunitionInBackpack.Count > _ammoCount - 1)
                return;

            PutAmmoInBackpack(ammunition);
            BackpackFilter();
        }
    }

    public void PutAmmoInBackpack(Ammunition ammunition)
    {
        ammunition.transform.parent = backpackPoint.transform;

        int index = _ammunitionInBackpack.Count;
        _ammunitionInBackpack.Add(ammunition);

        Vector3 endPosition = new Vector3(0, index * _distanceBetweenObjects, 0);

        ammunition.transform.DOLocalRotate(Vector3.zero, _itemRotationTime);
        ammunition.transform.DOLocalMove(endPosition, _itemMoveTime);

    }

    float putTime = 0.3f;
    private void OnTriggerStay(Collider other)
    {
        if (_nearTurret == null || HasTurret) return;

        if (putTime <= 0)
        {
            RemoveAmmoFromBackpack(_nearTurret);
            putTime = 0.5f;
        }
        else
            putTime -= Time.deltaTime;

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

    [ContextMenu("Filter")]
    public void BackpackFilter()
    {
        if (_ammunitionInBackpack.Count == 0)
            return;

        _ammunitionInBackpack.Clear();
        _ammunitionInBackpack.AddRange(backpackPoint.GetComponentsInChildren<Ammunition>());

        for (int i = 0; i < _ammunitionInBackpack.Count; i++)
            _ammunitionInBackpack[i].transform.localPosition = Vector3.zero;

        for (int i = 0; i < _ammunitionInBackpack.Count; i++)
            _ammunitionInBackpack[i].transform.localPosition = new Vector3(0, i * _distanceBetweenObjects, 0);
    }

    public void RemoveAmmoFromBackpack(BaseTurret baseTurret)
    {
        if (_ammunitionInBackpack.Count == 0 || baseTurret == null)
            return;

        BackpackFilter();
        Ammunition ammunition = _ammunitionInBackpack[_ammunitionInBackpack.Count - 1];
        ammunition.collider.enabled = false;
        _ammunitionInBackpack.Remove(ammunition);
        
        ammunition.transform.parent = baseTurret.transform;
        baseTurret.Charge(ammunition.ammoCount);

        //ammunition.transform.DOLocalRotate(Vector3.zero, _itemRotationTime);
        //ammunition.transform.DOLocalMove(Vector3.zero, _itemMoveTime);
        Destroy(ammunition.gameObject/*, _itemMoveTime + 0.05f*/);
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
