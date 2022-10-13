using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour
{
    private const float _interactCheckTime = .025f;

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
    [SerializeField] private int _maxAmmo = 20;

    [Label("Interaction", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.5f, 3f)] private float _interactRadius = 2f;
    [SerializeField] private LayerMask _interactableMask;

    [Label("Place Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Transform _placePosition;
    [SerializeField] private Transform _backpackPoint;

    private Player _player;
    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;
    private List<Ammunition> _ammunition = new List<Ammunition>();

    private float _distanceBetweenObjects = 0.25f;
    private float _delayTimer = 0f;
    private float _putTimer = 0f;
    private float _interactTimer = 0f;
    private Collider[] _nearColliders = new Collider[5];

    public BaseTurret NearTurret => _nearTurret;
    public BaseTurret TakedTurret => _takedTurret;
    public bool HasTurret { get { return _turretSlot.childCount > 0 && TakedTurret != null; } }
    public bool CanPlace { get; private set; } = true;
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

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (_delayTimer > 0)
            _delayTimer -= Time.deltaTime;

        if (_putTimer >= 0)
            _putTimer -= Time.deltaTime;

        _interactTimer += Time.deltaTime;

        if (_interactTimer >= _interactCheckTime)
        {
            _interactTimer = 0;

            CheckInteract();
        }

        if (_nearTurret != null)
        {
            if (Vector3.Distance(_nearTurret.transform.position, transform.position) > _interactRadius)
            {
                _nearTurret.SetSelected(false);
                _nearTurret = null;
            }
        }

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

    private void CheckInteract()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, _interactRadius, _nearColliders, _interactableMask);

        for (int i = 0; i < count; i++)
        {
            Collider other = _nearColliders[i];

            if (_nearTurret == null || HasTurret)
            {
                if (other.TryGetComponent(out BaseTurret turret))
                {
                    if (_nearTurret != null)
                    {
                        _nearTurret.SetSelected(false);
                    }

                    _nearTurret = turret;
                    _nearTurret.SetSelected(true);

                    break;
                }
            }
            else
            {
                if (_nearTurret.gameObject == other.gameObject)
                {
                    if (_ammunition.Count > 0 && _putTimer <= 0 && _nearTurret.CanCharge)
                    {
                        ChargeTurret(_nearTurret);
                    }
                    else if (_delayTimer > 0f)
                    {
                        return;
                    }
                }
            }

            if (other.TryGetComponent(out Ammunition ammunition))
            {
                if (_ammunition.Count > _maxAmmo - 1)
                    return;

                PutAmmo(ammunition);
                break;
            }
        }
    }

    #region Ammo

    public void PutAmmo(Ammunition ammunition)
    {
        ammunition.transform.parent = _backpackPoint.transform;

        int index = _ammunition.Count;
        ammunition.enabled = false;

        _ammunition.Add(ammunition);

        Vector3 endPosition = new Vector3(0, (float)index * _distanceBetweenObjects, 0);
        Debug.Log("Index - " + index + " | End Position - " + endPosition);

        ammunition.transform.DOLocalRotate(Vector3.zero, _ammoRotationSpeed);
        ammunition.transform.DOLocalMove(endPosition, _ammoMoveSpeed);

    }

    [ContextMenu("Debug Index")]
    private void DebugIndex()
    {
        for (int i = 0; i < _ammunition.Count; i++)
        {
            _ammunition[i].transform.localPosition = Vector3.zero;
            _ammunition[i].transform.localEulerAngles = Vector3.zero;//new Vector3(Vector3.zero, _ammoRotationSpeed);
            _ammunition[i].enabled = false;
        }

        for (int i = 0; i < _ammunition.Count; i++)
        {
            Vector3 endPosition = new Vector3(0, i * _distanceBetweenObjects, 0);
            Debug.Log("Index - " + i + " | End Position - " + endPosition);
            _ammunition[i].transform.localPosition = endPosition;//, _ammoMoveSpeed);
        }
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
            Vector3 targetPos = _placePosition.position;
            targetPos.y = _placeOffset.y;

            BaseTurret turret = _takedTurret;

            turret.transform.DOJump(targetPos, 1f, 1, .6f).OnComplete(() =>
            {
                turret.enabled = true;
                turret.transform.SetParent(null);
            });

            turret.transform.DORotate(Vector3.zero, 1f);

            _takedTurret = null;
            _delayTimer = _takeDelay;
        }
    }

    public void Take()
    {
        if (_nearTurret == null)
            return;

        _takedTurret = _nearTurret;
        _nearTurret.SetSelected(false);
        _nearTurret = null;

        _takedTurret.transform.SetParent(_turretSlot);

        _takedTurret.transform.DOLocalRotate(Vector3.zero, 0.3f);
        _takedTurret.transform.DOLocalMove(Vector3.zero, 0.25f);

        _takedTurret.enabled = false;
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
                newTurret.PlayUpgradeParticle();

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
        _player.Map.MapGrid.GetXY(_takedTurret.transform.position, out int x, out int y);

        GridCell cell = _player.Map.MapGrid.GetObject(x, y);

        if (cell == null)
            return false;

        return cell.CanBuild() && cell.Type != CellType.Path;
    }

    #endregion
}
