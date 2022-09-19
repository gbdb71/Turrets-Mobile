using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Label("Speed", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(5, 15f)] private float _speed = 5f;
    [SerializeField] private float _speedCoef = 0.2f;

    [SerializeField, Range(1, 15f)] private float _speedWithTurret = 2f;
    [SerializeField] private float _speedWithTurretCoef = 0.25f;

    [Label("Rotation", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 20)] private float _rotationPerFrame = 10f;

    [Inject] private Joystick _joystick;
	private CharacterController _cc;
    private Player _player;

    public float Speed { set { _speed = Mathf.Clamp(value, 1, 99); } }
    public float SpeedWithTurret {set { _speedWithTurret =Mathf.Clamp(value, 1, 99); } }
    public float MoveVelocity;
    public float LayerWeight;
    public bool IsMove { get; private set; }

    public float _distance = 0;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _player = GetComponent<Player>();
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
        MoveVelocity = Vector2.Distance(Vector2.zero, new Vector2(_joystick.Horizontal, _joystick.Vertical));

        float speed = _player.Inventory.HasTurret ? _speedWithTurret : _speed;
        LayerWeight = _player.Inventory.HasTurret ? 1 : 0;

        float animationSpeed = _player.Inventory.HasTurret ? _speedWithTurret * _speedWithTurretCoef : _speed * _speedCoef;
        _player.PlayerAnimations.AnimationSpeed = animationSpeed;

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
        if(_cc.isGrounded)
        {
            float groundedGravity = .05f;
            moveDir.y = groundedGravity;
        }
        else
        {
            moveDir.y = Physics.gravity.y;
        }
    }


}
 