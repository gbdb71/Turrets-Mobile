using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Label("Speed", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(5, 15f)] private float _speed = 5f;
    [SerializeField, Range(1, 15f)] private float _speedWithTurret = 2f;

    [Label("Rotation", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 20)] private float _rotationPerFrame = 10f;

    [Inject] private Joystick _joystick;
	private CharacterController _cc;
    private Player _player;

    public float Speed { set { _speed = Mathf.Clamp(value, 1, 99); } }
    public float SpeedWithTurret {set { _speedWithTurret =Mathf.Clamp(value, 1, 99); } }
    public float MoveVelocity { get { return _cc.velocity.magnitude; } }
    public bool IsMove { get; private set; }


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

        //float speed = _player.Inventory.HasTurret ? _speedWithTurret : _speed; ;
        float speed = _player.Inventory.HasTurret ? _speedWithTurret : _speed; 

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
 