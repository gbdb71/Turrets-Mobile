using System;
using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Label("Speed", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(5, 15f)] private float _speed = 5f;
    [SerializeField] private float _speedCoef = 0.2f;

    private float _speedWithTurret = 2f;
    private float _speedWithTurretCoef = 0.25f;

    [Label("Rotation", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 20)] private float _rotationPerFrame = 10f;

    [Inject] private Joystick _joystick;
    private CharacterController _cc;
    private Player _player;

    public float MoveVelocity => _cc.velocity.sqrMagnitude;
    public float LayerWeight;
    public bool IsMove { get; private set; }

    public float _distance = 0;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        UserData.OnUpgradeChanged += UpdateSpeed;

        UpdateSpeed(UpgradeType.Speed, -1);
        UpdateSpeed(UpgradeType.SpeedWithTurret, -1);
    }
    private void Update()
    {
        HandleGravity();
        Movement();
        Rotate();
    }

    private Vector3 moveDir;
    private void Movement()
    {
        moveDir.Set(_joystick.Horizontal, 0, _joystick.Vertical);
        float speed = _player.Inventory.HasTurret ? _speedWithTurret : _speed;
        LayerWeight = _player.Inventory.HasTurret ? 1 : 0;

        _cc.SimpleMove(moveDir * speed);

        IsMove = moveDir.magnitude > 0 && _cc.isGrounded;
    }

    private void Rotate()
    {
        if (moveDir.magnitude > 0)
        {
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);

            transform.rotation = Quaternion.Slerp(currentRot, targetRot, _rotationPerFrame * Time.deltaTime);
        }
    }
    private void HandleGravity()
    {
        if (_cc.isGrounded)
        {
            float groundedGravity = .05f;
            moveDir.y = groundedGravity;
        }
        else
        {
            moveDir.y = Physics.gravity.y;
        }
    }


    private void UpdateSpeed(UpgradeType type, int index)
    {
        switch (type)
        {
            case UpgradeType.Speed:
                _speed = GetUpgradedSpeed(type);
                break;
            case UpgradeType.SpeedWithTurret:
                _speedWithTurret = GetUpgradedSpeed(type);
                break;
            default:
                break;
        }
    }

    private float GetUpgradedSpeed(UpgradeType type)
    {
        int upgradeIndex = _player.Data.User.UpgradesProgress[type];
        return _player.Data.UpgradesInfo.Upgrades.First(x => x.Type == type).Elements[upgradeIndex].Value;
    }
}
