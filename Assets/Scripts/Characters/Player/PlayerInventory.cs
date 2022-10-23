using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private const float _interactCheckTime = .01f;

    [Label("Backpack", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Transform _backpackPoint;
    [SerializeField] private float _distanceBetweenObjects = 0.25f;
    [SerializeField, Range(.1f, 2f)] private float _objectMoveSpeed = 0.5f;
    [SerializeField, Range(.1f, 2f)] private float _objectRotationSpeed = 0.25f;

    [Label("Turrets", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private Transform _turretSlot;
    [SerializeField] private Color _placeBlockColor;
    [SerializeField, Range(.2f, 1f)] private float _takeDelay = .5f;
    [SerializeField] private Vector3 _placeOffset;

    [Label("Interaction", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.5f, 3f)] private float _interactRadius = 2f;
    [SerializeField] private LayerMask _interactableMask;

    [Label("Place Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private Transform _placePosition;

    private BaseTurret _nearTurret;
    private BaseTurret _takedTurret;
    private List<IAbillity> _inventoryAbillities = new List<IAbillity>();

    private float _takeDelayTimer = 0f;
    private float _abillityDelayTimer = 0f;
    private float _putTimer = 0f;
    private float _interactTimer = 0f;
    private Collider[] _nearColliders = new Collider[10];

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

    private void Update()
    {
        UpdateTimers();

        UpdateAbillities();

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
            bool canPlace = true;

            if (CanPlace != canPlace)
            {
                CanPlace = canPlace;
                ChangeTurretColor();
            }
        }
    }

    private void UpdateAbillities()
    {
        for (int i = _inventoryAbillities.Count - 1; i >= 0; i--)
        {
            IAbillity abillity = _inventoryAbillities[i];

            if (abillity == null)
            {
                _inventoryAbillities.RemoveAt(i);
                continue;
            }

            if (abillity.CanActivate())
            {
                if (abillity.HasDelay() && _abillityDelayTimer > 0)
                    continue;

                _abillityDelayTimer = .8f;

                _inventoryAbillities.RemoveAt(i);
                abillity.Activate();

                break;
            }
        }

    }

    private void UpdateTimers()
    {
        if (_takeDelayTimer > 0)
            _takeDelayTimer -= Time.deltaTime;

        if (_putTimer >= 0)
            _putTimer -= Time.deltaTime;

        if (_abillityDelayTimer > 0)
            _abillityDelayTimer -= Time.deltaTime;

        _interactTimer += Time.deltaTime;

        if (_interactTimer >= _interactCheckTime)
        {
            _interactTimer = 0;

            CheckInteract();
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
                if (turret == _takedTurret)
                    return;

                if (_nearTurret != null)
                {
                    _nearTurret.SetSelected(false);
                }

                _nearTurret = turret;
                _nearTurret.SetSelected(true);
                break;
            }

            if (other.TryGetComponent(out IAbillity abillity))
            {
                if (_inventoryAbillities.Contains(abillity))
                    return;

                Transform abillityTransform = abillity.GetTransform();
                abillityTransform.GetComponent<Collider>().enabled = false;

                abillityTransform.SetParent(_backpackPoint.transform, true);
                abillityTransform.localScale = Vector3.one;

                int index = _inventoryAbillities.Count;

                _inventoryAbillities.Add(abillity);

                Vector3 endPosition = new Vector3(0, (float)index * _distanceBetweenObjects, 0);
                abillityTransform.DOLocalRotate(Vector3.zero, _objectRotationSpeed);
                abillityTransform.DOLocalMove(endPosition, _objectMoveSpeed);
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
            turret.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InOutBack);
            turret.transform.DOJump(targetPos, 2f, 1, .6f).OnComplete(() =>
            {
                turret.enabled = true;
                turret.transform.SetParent(null);
            });
            var s = DOTween.Sequence();
            s.Append(turret.transform.DOScale(new Vector3(1.3f, 0.7f, 1.3f), 0.1f)).SetDelay(0.5f);
            s.Append(turret.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

            turret.transform.DORotate(Vector3.zero, 1f);

            _takedTurret.SetSelected(false);
            _takedTurret = null;

            _takeDelayTimer = _takeDelay;
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
        // _takedTurret.transform.DOLocalMove(Vector3.zero, 0.25f);
        _takedTurret.transform.DOScale(new Vector3(0.8f, 1.3f, 0.8f), 0.3f);

        var s = DOTween.Sequence();
        s.Append(_takedTurret.transform.DOLocalJump(Vector3.zero, 1f, 1, 0.3f));
        s.Append(_takedTurret.transform.DOScale(new Vector3(1.3f, 0.7f, 1.3f), 0.1f));
        s.Append(_takedTurret.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

        _takedTurret.SetSelected(true);
        _takedTurret.enabled = false;

    }

    public void Upgrade()
    {
        if (CanUpgrade)
        {
            GameObject near = _nearTurret.gameObject;

            _takedTurret.transform.DOJump(near.transform.position, 1f, 1, .25f).OnComplete(() =>
            {
                BaseTurret newTurret = Instantiate(_takedTurret.NextGrade, near.transform.position, near.transform.rotation, null);
                newTurret.PlayUpgradeParticle();
                newTurret.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).From(0);


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

    #endregion
}
