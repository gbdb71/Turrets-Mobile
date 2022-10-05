using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

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

    private Player _player;
    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;
    private List<Ammunition> _ammunition = new List<Ammunition>();

    private float _distanceBetweenObjects = 0.25f;
    private float _takeProgess = 0f;
    private float _delayTimer = 0f;
    private float _putTimer = 0.0f;
    private int _maxAmmo = 0;

    public BaseTurret NearTurret => _nearTurret;
    public BaseTurret TakedTurret => _takedTurret;
    public Transform backpackPoint;
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

    private List<BaseTurret> _ignoreTurrets = new List<BaseTurret>();

    private void Awake()
    {
        _player = GetComponent<Player>();

        UserData.OnUpgradeChanged += UpdateAmmoMax;
        UpdateAmmoMax(UpgradeType.AmmoCount, -1);
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
                        if(_ignoreTurrets.Contains(turret))
                        {
                            _ignoreTurrets.Remove(turret);
                            return;
                        }

                        _nearTurret = turret;
                    }
                    break;
                }
            case "Ammunition":
                {
                    if (other.TryGetComponent(out Ammunition ammunition))
                    {
                        if (_ammunition.Count > _maxAmmo - 1)
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
<<<<<<< HEAD
=======

                if (!_nearTurret.IndicatorTransform.gameObject.activeSelf)
                {
                    _nearTurret.IndicatorTransform.gameObject.SetActive(true);
                    _nearTurret.ChangeColorWhenSelected();
                }

                _takeProgess += Time.deltaTime;
                _nearTurret.IndicatorFill.fillAmount = _takeProgess;

                if (_takeProgess >= 1f)
                {
                    Take(_nearTurret);
                }

>>>>>>> feature/take-turret-indicator
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _nearTurret = null;
    }

    private void UpdateAmmoMax(UpgradeType type, int index)
    {
        if (type == UpgradeType.AmmoCount)
        {
            int upgradeIndex = _player.Data.User.UpgradesProgress[type];
            _maxAmmo = 20;//(int)_player.Data.UpgradesInfo.Upgrades.First(x => x.Type == type).Elements[upgradeIndex].Value;
        }
    }

    #region Ammo

    public void PutAmmo(Ammunition ammunition)
    {
        ammunition.transform.parent = backpackPoint.transform;

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

    [Header("Place Settings")]
    [SerializeField] private Transform placePosition;

    public void Place()
    {
        if (_takedTurret != null && CanPlace)
        {
            Vector3 targetPos = placePosition.position;
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
        _nearTurret = null;

        _takedTurret.transform.SetParent(_turretSlot);

        _takedTurret.transform.DOLocalRotate(Vector3.zero, 0.3f);
        _takedTurret.transform.DOLocalMove(Vector3.zero, 0.25f);

        _takedTurret.enabled = false;
    }

<<<<<<< HEAD
=======
    private void ResetProgress(BaseTurret turret)
    {
        turret.IndicatorTransform.gameObject.SetActive(false);
        _takeProgess = 0f;
        turret.ReturnColorWhenSelected();
    }

>>>>>>> feature/take-turret-indicator
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
<<<<<<< HEAD
                _ignoreTurrets.Add(newTurret);

=======
                newTurret.PlayUpgradeParticle();
>>>>>>> feature/take-turret-indicator

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
