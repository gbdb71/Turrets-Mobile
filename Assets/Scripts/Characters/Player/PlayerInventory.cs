using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Zenject;

public class PlayerInventory : MonoBehaviour
{
    [Label("Turrets", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private Transform _turretSlot;
    [SerializeField] private GameObject _upgradeEffect;
    [SerializeField] private Color _placeBlockColor;
    [SerializeField, Range(.2f, 1f)] private float _takeDelay = .5f;

    [Label("Ammunition", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Vector3 _placeOffset;
    [SerializeField, Range(.1f, 2f)] private float _ammoMoveSpeed = 0.5f;
    [SerializeField, Range(.1f, 2f)] private float _ammoRotationSpeed = 0.25f;
    [SerializeField, Range(.1f, 1f)] private float _ammoPutDelay = .5f;

    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;
    private float _takeProgess = 0f;

    private List<Ammunition> _ammunition = new List<Ammunition>();
    private float _distanceBetweenObjects = 0.25f;
    private int _ammoCount;
    private float _delayTimer = 0f;
    private float _putTimer = 0.0f;

    [Inject] private Map _map;

    public BaseTurret NearTurret => _nearTurret;
    public BaseTurret TakedTurret => _takedTurret;
    public Transform backpackPoint;
    public bool HasTurret { get { return _turretSlot.childCount > 0 && TakedTurret != null; } }
    public int AmmoCount { set { _ammoCount = Mathf.Clamp(value, 1, 99); } }
    public bool CanPlace { get; private set; }
    public bool CanUpgrade
    {
        get
        {
            return HasTurret && NearTurret != null &&
                NearTurret != TakedTurret &&
                NearTurret.NextGrade != null &&
                NearTurret.NextGrade == TakedTurret.NextGrade;
        }
    }


    private void Update()
    {
        if (_delayTimer > 0)
            _delayTimer -= Time.deltaTime;

        if (_putTimer >= 0)
            _putTimer -= Time.deltaTime;

        if (HasTurret)
        {
            bool canPlace = CheckPlace();

            if (CanPlace != canPlace)
            {
                CanPlace = canPlace;
                ChangeTurretColor();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Turret":
                {
                    if (other.TryGetComponent(out BaseTurret turret))
                    {
                        if (_nearTurret != null)
                            ResetProgress(_nearTurret);

                        _nearTurret = turret;
                    }
                    break;
                }
            case "Ammunition":
                {
                    if (other.TryGetComponent(out Ammunition ammunition))
                    {
                        if (_ammunition.Count > _ammoCount - 1)
                            return;

                        PutAmmo(ammunition);
                    }
                    break;
                }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_nearTurret == null || HasTurret) return;

        if (_nearTurret.gameObject == other.gameObject)
        {
            if (_ammunition.Count > 0 && _putTimer <= 0 && _nearTurret.CanCharge)
            {
                ChargeTurret(_nearTurret);
            }
            else
            {
                if (HasTurret || _delayTimer > 0f)
                    return;

                if (!_nearTurret.IndicatorTransform.gameObject.activeSelf)
                    _nearTurret.IndicatorTransform.gameObject.SetActive(true);

                _takeProgess += Time.deltaTime;
                _nearTurret.IndicatorFill.fillAmount = _takeProgess;

                if (_takeProgess >= 1f)
                {
                    Take(_nearTurret);
                }

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_nearTurret != null)
            ResetProgress(_nearTurret);

        _nearTurret = null;
    }

    #region Ammo

    public void PutAmmo(Ammunition ammunition)
    {
        ammunition.transform.parent = backpackPoint.transform;

        int index = _ammunition.Count;
        _ammunition.Add(ammunition);

        Vector3 endPosition = new Vector3(0, index * _distanceBetweenObjects, 0);

        ammunition.transform.DOLocalRotate(Vector3.zero, _ammoRotationSpeed);
        ammunition.transform.DOLocalMove(endPosition, _ammoMoveSpeed);

    }
    public void ChargeTurret(BaseTurret turret)
    {
        if (_ammunition.Count == 0 || turret == null)
            return;

        Ammunition ammunition = _ammunition[_ammunition.Count - 1];

        _ammunition.Remove(ammunition);

        ammunition.transform.DOMove(turret.transform.position, _ammoMoveSpeed).OnComplete(() =>
        {
            turret.Charge();
            Destroy(ammunition.gameObject);
        });

        _putTimer = _ammoPutDelay;
        _delayTimer = _takeDelay;

    }

    #endregion

    #region Turret

    public void Place()
    {
        if (_takedTurret != null && CanPlace)
        {
            Vector3 targetPos = _takedTurret.transform.position;
            targetPos.y = 0f;
            targetPos += _placeOffset;

            BaseTurret turret = _takedTurret;

            turret.transform.DOJump(targetPos, 1f, 1, .6f).OnComplete(() =>
            {
                turret.enabled = true;
                turret.transform.SetParent(null);
            });

            _takedTurret = null;

            _delayTimer = _takeDelay;

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
    private void ChangeTurretColor()
    {
        for (int i = 0; i < TakedTurret.Renderers.Length; i++)
        {
            TakedTurret.Renderers[i].material.color = CanPlace ? Color.white : _placeBlockColor;
        }
    }
    private bool CheckPlace()
    {
        _map.MapGrid.GetXY(_takedTurret.transform.position, out int x, out int y);

        GridCell cell = _map.MapGrid.GetObject(x, y);

        return cell.CanBuild() && cell.Type != CellType.Path;
    }

    #endregion
}
