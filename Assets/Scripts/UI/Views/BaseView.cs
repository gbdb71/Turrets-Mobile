using UnityEngine;
using Zenject;
using Cinemachine;

public abstract class BaseView : MonoBehaviour
{
    [Inject] protected CinemachineVirtualCamera _playerCamera;

    private void Update()
    {
        if (_playerCamera != null)
            transform.LookAt(_playerCamera.transform);

        UpdateLogic();
    }

    protected abstract void UpdateLogic();

}