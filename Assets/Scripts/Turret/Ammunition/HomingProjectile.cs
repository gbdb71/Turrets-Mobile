using ToolBox.Pools;
using UnityEngine;

public class HomingProjectile : BaseProjectile
{
    private float _speed;
    private Transform _target;
    private bool _yMove = false;

    private bool _initialized = false;

    public void SetY(bool enabled)
    {
        _yMove = enabled;
    }

    public void SetTarget(Transform enemy)
    {
        _target = enemy;
        _initialized = true;
    }

    private void OnDisable()
    {
        _initialized = false;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    private Vector3 _lastTarget;

    protected override void Move()
    {
        if (!_initialized) return;

        if (_target != null)
        {
            _lastTarget = _target.transform.position;
        }

        Vector3 direction = (_lastTarget - transform.position).normalized;

        if (!_yMove)
            direction.y = 0;
        else
            direction.y += .5f;

        transform.position += direction * _speed * Time.deltaTime;
        transform.localRotation = Quaternion.LookRotation(direction);

        if(TargetReached())
            gameObject.Release();
    }

    private bool TargetReached()
    {
        Vector3 target = _lastTarget;
        Vector3 current = transform.position;

        if(!_yMove)
        {
            target.y = 0;
            current.y = 0;
        }

        return target == current;
    }
}

