using UnityEngine;

public class HomingProjectile : BaseProjectile
{
    private float _speed;
    private Transform _target;

    private void Start()
    {
        transform.position = _launchPoint;
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
            direction = (_target.transform.position - transform.position).normalized;
            direction.y = 0f;

        }

        transform.position += direction * _speed * Time.deltaTime;
    }
}

