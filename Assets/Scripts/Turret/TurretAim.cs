﻿using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [Label("Aim", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(5, 20f)] private float _aimDistance = 10f;

    [Label("Rotations", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private Transform _turretBase = null;
    [SerializeField] private Transform _barrels = null;

    [Label("Elevation", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 500)] private float _elevationSpeed = 50f;
    [SerializeField, Range(0, 90)] private float MaxElevation = 0f;
    [SerializeField, Range(0, 90)] private float MaxDepression = 0f;

    [Label("Traverse", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 500)] private float _traverseSpeed = 60f;
    [SerializeField] private bool hasLimitedTraverse = false;
    [SerializeField, Range(0, 179), ShowIf(nameof(hasLimitedTraverse), true)] private float _leftLimit = 120f;
    [SerializeField, Range(0, 179), ShowIf(nameof(hasLimitedTraverse), true)] private float _rightLimit = 120f;
    [SerializeField, Range(1, 360)] private float aimedThreshold = 5f;

    [Label("Debug", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, LeftToggle] private bool _drawDebugRay = true;
    [SerializeField, LeftToggle] private bool _drawDebugArcs = false;


    private float limitedTraverseAngle = 0f;
    private float angleToTarget = 0f;
    private float elevation = 0f;

    private bool hasBarrels = false;

    private bool isAimed = false;
    private bool isBaseAtRest = false;
    private bool isBarrelAtRest = false;

    private bool _isIdle = false;
    private Vector3 _aimPos = Vector3.zero;

    /// <summary>
    /// True when the turret cannot rotate freely in the horizontal axis.
    /// </summary>
    public bool HasLimitedTraverse { get { return hasLimitedTraverse; } }

    /// <summary>
    /// True when the turret is idle and at its resting position.
    /// </summary>
    public bool IsTurretAtRest { get { return isBarrelAtRest && isBaseAtRest; } }

    /// <summary>
    /// True when the turret is aimed at the given <see cref="_aimPos"/>. When the turret
    /// is idle, this is never true.
    /// </summary>
    public bool IsAimed { get { return isAimed; } }

    /// <summary>
    /// Angle in degress to the given <see cref="_aimPos"/>. When the turret is idle,
    /// the angle reports 999.
    /// </summary>
    public float AngleToTarget { get { return _isIdle ? 999f : angleToTarget; } }

    /// <summary>
    /// Maximum aiming distance
    /// </summary>
    public float AimDistance { get { return _aimDistance; } }

    public Transform TurretBase { get { return _turretBase; } }
    public Transform Barrels { get { return _barrels; } }

    public Transform ArcRoot { get { return _barrels != null ? _barrels : _turretBase; } }

    public bool DrawDebugRay { get => _drawDebugRay; }
    public bool DrawDebugArcs { get => _drawDebugArcs;  }


    /// <summary>
    /// Return turret aim to base look
    /// </summary>
    /// <param name="idle"></param>
    public void SetIdle(bool idle)
    {
        _isIdle = idle;
    }

    /// <summary>
    /// Set turret aiming pos
    /// </summary>
    /// <param name="pos"></param>
    public void SetAim(Vector3 pos)
    {
        _aimPos = pos;
    }


    private void Awake()
    {
        hasBarrels = _barrels != null;
        if (_turretBase == null)
            Debug.LogError(name + ": TurretAim requires an assigned TurretBase!");
    }

    private void Update()
    {
        if (_isIdle)
        {
            if (!IsTurretAtRest)
                RotateTurretToIdle();
            isAimed = false;
        }
        else
        {
            RotateBaseToFaceTarget(_aimPos);

            if (hasBarrels)
                RotateBarrelsToFaceTarget(_aimPos);

            // Turret is considered "aimed" when it's pointed at the target.
            angleToTarget = GetTurretAngleToTarget(_aimPos);

            // Turret is considered "aimed" when it's pointed at the target.
            isAimed = angleToTarget < aimedThreshold;

            isBarrelAtRest = false;
            isBaseAtRest = false;
        }
    }

    private float GetTurretAngleToTarget(Vector3 targetPosition)
    {
        float angle = 999f;

        if (hasBarrels)
        {
            angle = Vector3.Angle(targetPosition - _barrels.position, _barrels.forward);
        }
        else
        {
            Vector3 flattenedTarget = Vector3.ProjectOnPlane(
                targetPosition - _turretBase.position,
                _turretBase.up);

            angle = Vector3.Angle(
                flattenedTarget - _turretBase.position,
                _turretBase.forward);
        }

        return angle;
    }

    private void RotateTurretToIdle()
    {
        // Rotate the base to its default position.
        if (hasLimitedTraverse)
        {
            limitedTraverseAngle = Mathf.MoveTowards(
                limitedTraverseAngle, 0f,
                _traverseSpeed * Time.deltaTime);

            if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                _turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
            else
                isBaseAtRest = true;
        }
        else
        {
            _turretBase.rotation = Quaternion.RotateTowards(
                _turretBase.rotation,
                transform.rotation,
                _traverseSpeed * Time.deltaTime);

            isBaseAtRest = Mathf.Abs(_turretBase.localEulerAngles.y) < Mathf.Epsilon;
        }

        if (hasBarrels)
        {
            elevation = Mathf.MoveTowards(elevation, 0f, _elevationSpeed * Time.deltaTime);
            if (Mathf.Abs(elevation) > Mathf.Epsilon)
                _barrels.localEulerAngles = Vector3.right * -elevation;
            else
                isBarrelAtRest = true;
        }
        else // Barrels automatically at rest if there are no barrels.
            isBarrelAtRest = true;
    }

    private void RotateBarrelsToFaceTarget(Vector3 targetPosition)
    {
        Vector3 localTargetPos = _turretBase.InverseTransformDirection(targetPosition - _barrels.position);
        Vector3 flattenedVecForBarrels = Vector3.ProjectOnPlane(localTargetPos, Vector3.up);

        float targetElevation = Vector3.Angle(flattenedVecForBarrels, localTargetPos);
        targetElevation *= Mathf.Sign(localTargetPos.y);

        targetElevation = Mathf.Clamp(targetElevation, -MaxDepression, MaxElevation);
        elevation = Mathf.MoveTowards(elevation, targetElevation, _elevationSpeed * Time.deltaTime);

        if (Mathf.Abs(elevation) > Mathf.Epsilon)
            _barrels.localEulerAngles = Vector3.right * -elevation;

#if UNITY_EDITOR
        if (_drawDebugRay)
            Debug.DrawRay(_barrels.position, _barrels.forward * localTargetPos.magnitude, Color.red);
#endif
    }

    private void RotateBaseToFaceTarget(Vector3 targetPosition)
    {
        Vector3 turretUp = transform.up;

        Vector3 vecToTarget = targetPosition - _turretBase.position;
        Vector3 flattenedVecForBase = Vector3.ProjectOnPlane(vecToTarget, turretUp);

        if (hasLimitedTraverse)
        {
            Vector3 turretForward = transform.forward;
            float targetTraverse = Vector3.SignedAngle(turretForward, flattenedVecForBase, turretUp);

            targetTraverse = Mathf.Clamp(targetTraverse, -_leftLimit, _rightLimit);
            limitedTraverseAngle = Mathf.MoveTowards(
                limitedTraverseAngle,
                targetTraverse,
                _traverseSpeed * Time.deltaTime);

            if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                _turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
        }
        else
        {
            _turretBase.rotation = Quaternion.RotateTowards(
                Quaternion.LookRotation(_turretBase.forward, turretUp),
                Quaternion.LookRotation(flattenedVecForBase, turretUp),
                _traverseSpeed * Time.deltaTime);
        }

#if UNITY_EDITOR
        if (_drawDebugRay && !hasBarrels)
            Debug.DrawRay(_turretBase.position,
                _turretBase.forward * flattenedVecForBase.magnitude,
                Color.red);
#endif
    }

#if UNITY_EDITOR
    // This should probably go in an Editor script, but dealing with Editor scripts
    // is a pain in the butt so I'd rather not.
    private void OnDrawGizmosSelected()
    {
        if (!_drawDebugArcs)
            return;

        if (_turretBase != null)
        {
            float kArcSize = _aimDistance;

            Color colorTraverse = new Color(1f, .5f, .5f, .1f);
            Color colorElevation = new Color(.5f, 1f, .5f, .1f);
            Color colorDepression = new Color(.5f, .5f, 1f, .1f);

            Transform arcRoot = _barrels != null ? _barrels : _turretBase;

            // Red traverse arc
            UnityEditor.Handles.color = colorTraverse;
            if (hasLimitedTraverse)
            {
                UnityEditor.Handles.DrawSolidArc(
                    arcRoot.position, _turretBase.up,
                    transform.forward, _rightLimit,
                    kArcSize);
                UnityEditor.Handles.DrawSolidArc(
                    arcRoot.position, _turretBase.up,
                    transform.forward, -_leftLimit,
                    kArcSize);
            }
            else
            {
                UnityEditor.Handles.DrawSolidArc(
                    arcRoot.position, _turretBase.up,
                    transform.forward, 360f,
                    kArcSize);
            }

            if (_barrels != null)
            {
                // Green elevation arc
                UnityEditor.Handles.color = colorElevation;
                UnityEditor.Handles.DrawSolidArc(
                    _barrels.position, _barrels.right,
                    _turretBase.forward, -MaxElevation,
                    kArcSize);

                // Blue depression arc
                UnityEditor.Handles.color = colorDepression;
                UnityEditor.Handles.DrawSolidArc(
                    _barrels.position, _barrels.right,
                    _turretBase.forward, MaxDepression,
                    kArcSize);
            }
        }
    }
#endif
}

