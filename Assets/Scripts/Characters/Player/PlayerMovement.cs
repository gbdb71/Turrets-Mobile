using System.Linq;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Label("Speed", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(5, 15f)] private float _speed = 5f;

    [Label("Rotation", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 20)] private float _rotationPerFrame = 10f;

    [Inject] private Joystick _joystick;
    private CharacterController _cc;
    private Player _player;

    public float _distance = 0;
    private Vector3 moveDir;

    public float MoveVelocity => _cc.velocity.sqrMagnitude;
    public float LayerWeight;
    public bool IsMove { get; private set; }

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _player = GetComponent<Player>();

        UserData.OnUpgradeChanged += UpdateSpeed;
        Game.OnGameFinished += DisableJoystick;
        Headquarters.OnDeath += DisableJoystick;
    }

    private void Update()
    {
        //if (/*_joystick.enabled &&*/ _joystick.gameObject.activeSelf)
        //{
            Movement();
            Rotate();
        //}

        HandleGravity();
    }

    private void Movement()
    {
        moveDir.Set(_joystick.Horizontal, 0, _joystick.Vertical);

        LayerWeight = _player.Inventory.HasTurret ? 1 : 0;

        float speed = _speed + _speed.Percent(SummableAbillity.GetValue(SummableAbillity.Type.PlayerMovementSpeed));
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

    private void DisableJoystick()
    {
        _joystick.gameObject.SetActive(false);
    }

    private void UpdateSpeed(UpgradeType type)
    {
        if (type == UpgradeType.Speed)
        {
            int upgradeIndex = _player.Data.User.UpgradesProgress[type];
            _speed = _player.Data.UpgradesInfo.Upgrades.First(x => x.Type == type).Elements[upgradeIndex].Value;
        }
    }
}
