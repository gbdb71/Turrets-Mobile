using UnityEngine;
using DG.Tweening;

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
            direction = (_target.transform.position - transform.position).normalized;
            if (_yMove)
                direction.y += .1f;
            else
                direction.y = 0;
        }

        transform.position += direction * _speed * Time.deltaTime;
        transform.localRotation = Quaternion.LookRotation(direction);
    }
}

