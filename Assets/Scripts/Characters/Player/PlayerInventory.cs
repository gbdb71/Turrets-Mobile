using UnityEngine;
using DG.Tweening;

public class PlayerInventory : MonoBehaviour
{
    private const float _interactCheckTime = .01f;

    [Label("Turrets", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private Transform _turretSlot;
    [SerializeField] private GameObject _upgradeEffect;
    [SerializeField] private Color _placeBlockColor;
    [SerializeField, Range(.2f, 1f)] private float _takeDelay = .5f;
    [SerializeField] private Vector3 _placeOffset;

    [Label("Interaction", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.5f, 3f)] private float _interactRadius = 2f;
    [SerializeField] private LayerMask _interactableMask;

    [Label("Place Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Transform _placePosition;

    private Player _player;
    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;

    private float _delayTimer = 0f;
    private float _putTimer = 0f;
    private float _interactTimer = 0f;
    private Collider[] _nearColliders = new Collider[3];

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
    }

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
