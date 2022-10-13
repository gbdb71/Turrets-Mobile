using UnityEngine;

public class HomingProjectile : BaseProjectile
{
    private float _speed;
    private Transform _target;
    private bool _yMove = false;

    private void Start()
    {
        transform.position = _launchPoint;
    }

    public void SetY(bool enabled)
    {
        _yMove = enabled;
    }

    public void SetTarget(Transform enemy)
    {
        _target = enemy;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    protected override void Move()
    {
        Vector3 direction = transform.forward;

        if (_target != null)
        {
            Vector3 target = (_target.transform.position + (_yMove ? new Vector3(0, .15f, 0f) : Vector3.zero));
            direction = (target - transform.position).normalized;
        }

        transform.position += direction * _speed * Time.deltaTime;
        transform.localRotation = Quaternion.LookRotation(direction);
    }
}

