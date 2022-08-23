using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Range(5, 15f)]
    [SerializeField] private float _speed = 5f;
    [Range(1, 20)]
    [SerializeField] private float _rotationPerFrame = 10f;

    
    private Joystick _joystick;
	private CharacterController _cc;

    public bool IsMove { get; private set; }

    [Inject]
    public void Construct(Joystick joystick)
    {
        _joystick = joystick;
    }

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleGravity();
        Movement();
		Rotate();
	}



    Vector3 moveDir;
	private void Movement()
	{
		moveDir.Set(_joystick.Horizontal, 0, _joystick.Vertical);
		_cc.SimpleMove(moveDir * _speed);

        IsMove = moveDir.magnitude > 0;
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
            moveDir.y =