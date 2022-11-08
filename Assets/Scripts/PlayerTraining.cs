using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PlayerTraining : MonoBehaviour
{
    [SerializeField] private List<Transform> _tips;
    [SerializeField] private CinemachineVirtualCamera _portalCamera;
    [SerializeField] private TurretDamageAbillity _turretAbillity;
    [SerializeField] private AttackDroneAbillity _droneAbillity;
    [SerializeField] private TurretPlace _secondPlace;

    [Inject] private GameLogic _game;

    private Factory _factory;
    private WaveBar _waveBar;

    private bool _turretSpawned = false;
    private bool _turretTaked = false;
    private bool _abillityTaked = false;
    private bool _abillityActivated = false;
    private bool _turretPlaced = false;
    private bool _waveChanged = false;

    private void Awake()
    {
        _waveBar = FindObjectOfType<WaveBar>();
        _factory = FindObjectOfType<Factory>();

        FindObjectOfType<PointerManager>().enabled = false;
        
        Factory.OnTurretCreated += SetTurretSpawned;
        PlayerInventory.OnTurretTaked += SetTurretTaked;
        PlayerInventory.OnAbillityTaked += SetAbillityTaked;
        TurretPlace.OnTurretPlaced += SetTurretPlaced;
        LevelScenario.OnWaveChanged += SetWaveSchanged;

        _turretAbillity.OnActivated += SetAbillityAcivated;
        _droneAbillity.OnActivated += SetAbillityAcivated;
    }

    private void SetWaveSchanged(int index)
    {
        _waveChanged = true;
    }

    private void OnDestroy()
    {
        Factory.OnTurretCreated -= SetTurretSpawned;
        PlayerInventory.OnTurretTaked -= SetTurretTaked;
        PlayerInventory.OnAbillityTaked -= SetAbillityTaked;
        TurretPlace.OnTurretPlaced -= SetTurretPlaced;
        LevelScenario.OnWaveChanged -= SetWaveSchanged;
    }

    private void SetAbillityTaked(IAbillity obj)
    {
        _abillityTaked = true;
    }

    private void SetAbillityAcivated()
    {
        _abillityActivated = true;
    }

    private void SetTurretPlaced(TurretPlace place, BaseTurret obj)
    {
        if (!place.ShowRange)
            return;

        _turretPlaced = true;
    }

    private void SetTurretTaked(BaseTurret obj)
    {
        _turretTaked = true;
    }

    private void SetTurretSpawned(BaseTurret turret)
    {
        _turretSpawned = true;
    }

    private void Start()
    {
        _game.CanSpawnAbillites = false;
        _waveBar.gameObject.SetActive(false);

        for (int i = 0; i < _tips.Count; i++)
        {
            _tips[i].gameObject.SetActive(false);
        }

        StartCoroutine(nameof(Train));
    }

    private IEnumerator Train()
    {
        yield return CreateAndPlaceTurretTrain();

        _waveBar.gameObject.SetActive(true);

        ActivateTip(-1);

        while (!_game.IsReady)
        {
            yield return new WaitForEndOfFrame();
        }

        _game.SetReady(false);
        _portalCamera.Priority = 2;

        yield return new WaitForSeconds(1.5f);

        _portalCamera.Priority = -1;

        yield return CreateAndPlaceTurretTrain();
        yield return TakeAndActivateAbillityTrain(_turretAbillity);

        ActivateTip(-1);
        _game.SetReady(true);

        yield return new WaitForSeconds(3f);
        
        _game.SetReady(false);
        
        yield return TakeAndActivateAbillityTrain(_droneAbillity);

        ActivateTip(-1);
        
        _factory.enabled = true;
        _game.SetReady(true);
        _game.CanSpawnAbillites = true;

        _waveChanged = false;

        while (!_waveChanged)
        {
            yield return new WaitForEndOfFrame();
        }
        
        _secondPlace.gameObject.SetActive(true);
        _secondPlace.transform.DOScale(1f, .6f).From(0f).SetEase(Ease.Linear);
        
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator TakeAndActivateAbillityTrain(IAbillity abillity)
    {
        _abillityTaked = false;
        _abillityActivated = false;
        
        EnableAbillity(abillity);
        ActivateTip(3);

        while (!_abillityTaked)
        {
            yield return new WaitForEndOfFrame();
        }

        ActivateTip(2);

        while (!_abillityActivated)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void EnableAbillity(IAbillity abillity)
    {
        Transform abillityTransform = abillity.GetTransform();
        abillityTransform.gameObject.SetActive(true);
        Collider abillityCollider = abillityTransform.GetComponent<Collider>();

        abillityCollider.enabled = false;
        abillityTransform.DOScale(Vector3.one * 1.6f, .6f).From(Vector3.zero).SetEase(Ease.Linear);
        abillityTransform.DOJump(abillityTransform.position, 4f, 1, 1f)
            .OnComplete((() => abillityCollider.enabled = true));
    }

    private IEnumerator CreateAndPlaceTurretTrain()
    {
        _factory.enabled = true;
        
        _turretSpawned = false;
        _turretTaked = false;
        _turretPlaced = false;

        ActivateTip(0);

        while (!_turretSpawned)
        {
            yield return new WaitForEndOfFrame();
        }

        _factory.enabled = false;

        ActivateTip(1);

        while (!_turretTaked)
        {
            yield return new WaitForEndOfFrame();
        }

        ActivateTip(2);

        while (!_turretPlaced)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private Transform ActivateTip(int index)
    {
        for (int i = 0; i < _tips.Count; i++)
        {
            _tips[i].gameObject.SetActive(false);
        }

        if (index < 0)
            return null;

        Transform targetTip = _tips[index];
        targetTip.gameObject.SetActive(true);

        return targetTip;
    }
}