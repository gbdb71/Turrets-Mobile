using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private const float _interactCheckTime = .01f;

    #region Serialized

    [Label("Backpack", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)] [SerializeField]
    private Transform _backpackPoint;

    [SerializeField] private float _distanceBetweenObjects = 0.25f;
    [SerializeField, Range(.1f, 2f)] private float _objectMoveSpeed = 0.5f;

    [Label("Turrets", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)] [SerializeField, NotNull]
    private Transform _turretSlot;

    [SerializeField, Range(.2f, 1f)] private float _takeDelay = .5f;

    [Label("Interaction", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(.5f, 3f)]
    private float _interactRadius = 2f;

    [SerializeField] private LayerMask _interactableMask;

    #endregion

    #region State

    private TurretPlace _nearPlace;
    private BaseTurret _takedTurret;
    private List<IAbillity> _inventoryAbillities = new List<IAbillity>();

    private float _takeDelayTimer = 0f;
    private float _autoInteractTimer = 0;
    private float _abillityDelayTimer = 0f;
    private float _putTimer = 0f;
    private float _interactTimer = 0f;
    private Collider[] _nearColliders = new Collider[10];

    #endregion
    
    #region Public

    public TurretPlace NearPlace => _nearPlace;
    public BaseTurret TakedTurret => _takedTurret;

    public bool HasTurret
    {
        get { return _turretSlot.childCount > 0 && TakedTurret != null; }
    }

    public bool CanPlace => !_isPlacing && HasTurret && _nearPlace != null;

    public bool CanTake
    {
        get
        {
            if (!HasTurret && _nearPlace != null && _nearPlace.HasTurret ||
                HasTurret && _nearPlace != null && _nearPlace.PlacedTurret != null &&
                _nearPlace.PlacedTurret.NextGrade != TakedTurret.NextGrade)
            {
                
                return true;
            }
            return false;
        }
    }
    
    public bool CanUpgrade
    {
        get
        {
            return HasTurret && NearPlace != null &&
                   NearPlace.PlacedTurret != null &&
                   NearPlace.PlacedTurret != TakedTurret &&
                   NearPlace.PlacedTurret.NextGrade != null &&
                   NearPlace.PlacedTurret.NextGrade == TakedTurret.NextGrade;
        }
    }

    #endregion
    
    #region Update
    
    private void Update()
    {
        UpdateTimers();

        UpdateAbillities();

        if (_interactTimer >= _interactCheckTime)
        {
            _interactTimer = 0;

            CheckInteract();
        }

        if (_autoInteractTimer >= 2)
        {
            if (_nearPlace == null)
            {
                _autoInteractTimer = -.3f;
                return;
            }

            if (CanUpgrade)
            {
                Upgrade();
                _autoInteractTimer = -.3f;
                return;
            }
            
            if (CanTake)
            {
                Take();
                _autoInteractTimer = -.3f;
            }
            
            if (CanPlace)
            {
                Place();
                _autoInteractTimer = -.3f;
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

        if (_autoInteractTimer < 2 && _nearPlace != null)
        {
            float progress = _autoInteractTimer / 2f;
            
            if (_nearPlace != null && _nearPlace.PlacedTurret != null)
            {
                _nearPlace.PlacedTurret.Fill.fillAmount = progress;
            }
            else if(HasTurret)
            {
                TakedTurret.Fill.fillAmount = progress;
            }
            
            _autoInteractTimer += Time.deltaTime;
        }
        
        _interactTimer += Time.deltaTime;
    }
    
    #endregion

    #region Interaction

    private void CheckInteract()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position + (transform.forward * .5f), _interactRadius,
            _nearColliders,
            _interactableMask);

        if (count == 0)
        {
            ResetPlaceSelected(_nearPlace);

            _nearPlace = null;

            if (HasTurret)
            {
                _takedTurret.Fill.fillAmount = 0;
            }
        }

        for (int i = 0; i < count; i++)
        {
            Collider other = _nearColliders[i];

            if (other.TryGetComponent(out TurretPlace place))
            {
                if (place == null)
                    continue;

                if (place == _nearPlace)
                    break;

                if (!HasTurret && !place.HasTurret)
                    continue;

                TurretPlace oldPlace = _nearPlace;
                _nearPlace = place;
                _autoInteractTimer = 0;

                ResetPlaceSelected(oldPlace);

                if (_nearPlace.HasTurret)
                {
                    if (HasTurret && !CanUpgrade)
                        return;

                    _nearPlace.PlacedTurret.SetSelected(true);
                }

                break;
            }

            if (other.TryGetComponent(out IAbillity abillity))
                TryTakeAbillity(abillity);
        }
    }

    private void TryTakeAbillity(IAbillity abillity)
    {
        if (_inventoryAbillities.Contains(abillity))
            return;

        Transform abillityTransform = abillity.GetTransform();
        abillityTransform.GetComponent<Collider>().enabled = false;

        int index = _inventoryAbillities.Count;

        Vector3 endPosition = new Vector3(0, (float)index * _distanceBetweenObjects, 0);

        abillityTransform.SetParent(_backpackPoint.transform, true);
        abillityTransform.localRotation = Quaternion.identity;
        abillityTransform.DOLocalMove(endPosition, _objectMoveSpeed);
        abillityTransform.DOScale(Vector3.one, _objectMoveSpeed * .8f).SetEase(Ease.InBack);

        _inventoryAbillities.Add(abillity);
    }

    private void ResetPlaceSelected(TurretPlace place)
    {
        if (place != null && place.PlacedTurret != null)
        {
            _nearPlace.PlacedTurret.SetSelected(false);
            _nearPlace.PlacedTurret.Fill.fillAmount = 0;
        }
    }
    
    #endregion

    #region Turret

    private bool _isPlacing;

    public void Place()
    {
        if (_nearPlace == null)
            return;

        if (_takedTurret != null && CanPlace)
        {
            BaseTurret turret = _takedTurret;
            Vector3 targetPos = _nearPlace.transform.position;
            TurretPlace targetPlace = _nearPlace;


            turret.Fill.fillAmount = 0;
            _isPlacing = true;
            _takedTurret = null;

            turret.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InOutBack);
            turret.transform.DOJump(targetPos, 2f, 1, .6f).OnComplete(() =>
            {
                turret.enabled = true;
                targetPlace.Place(turret);
            });

            var sequence = DOTween.Sequence();

            turret.transform.DORotate(Vector3.zero, 1f);
            sequence.Append(turret.transform.DOScale(new Vector3(1.3f, 0.7f, 1.3f), 0.1f)).SetDelay(0.5f);
            sequence.Append(turret.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f));

            sequence.OnComplete(() => _isPlacing = false);

            turret.SetSelected(false);
            _takeDelayTimer = _takeDelay;
        }
    }

    public void Take()
    {
        if (_nearPlace == null)
            return;

        if (HasTurret)
        {
            Place();
        }

        _takedTurret = _nearPlace.PlacedTurret;
        _takedTurret.Fill.fillAmount = 0;
        _nearPlace.PlacedTurret = null;
        _nearPlace = null;

        _takedTurret.transform.SetParent(_turretSlot);

        _takedTurret.transform.DOLocalRotate(Vector3.zero, 0.3f);
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
            _takedTurret.transform.SetParent(null);
            _takedTurret.transform.DOJump(_nearPlace.transform.position, 1f, 1, .25f).OnComplete(() =>
            {
                BaseTurret nextGrade = _takedTurret.NextGrade;

                Destroy(_takedTurret.gameObject);
                Destroy(_nearPlace.PlacedTurret.gameObject);
                _takedTurret = null;

                BaseTurret newTurret = Instantiate(nextGrade, _nearPlace.transform.position,
                    _nearPlace.PlacedTurret.transform.rotation, null);

                newTurret.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).From(0);
                _nearPlace.Place(newTurret);

                newTurret.PlayUpgradeParticle();
            });
        }
    }

    #endregion
}