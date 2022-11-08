using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PointerManager : MonoBehaviour
{
    [SerializeField] PointerIcon _pointerPrefab;

    private Dictionary<WaveView, PointerIcon> _dictionary = new Dictionary<WaveView, PointerIcon>();
    [Inject] private Player _player;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;

        foreach (var view in FindObjectsOfType<WaveView>())
        {
            _dictionary.Add(view, Instantiate(_pointerPrefab, transform));
        }
    }

    private void LateUpdate()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);

        foreach (var kvp in _dictionary)
        {
            WaveView targetView = kvp.Key;
            PointerIcon pointerIcon = kvp.Value;

            Vector3 toEnemy = targetView.transform.position - _player.transform.position;
            Ray ray = new Ray(_player.transform.position, toEnemy);

            float rayMinDistance = Mathf.Infinity;
            int index = 0;

            for (int p = 0; p < 4; p++)
            {
                if (planes[p].Raycast(ray, out float distance))
                {
                    if (distance < rayMinDistance)
                    {
                        rayMinDistance = distance;
                        index = p;
                    }
                }
            }

            rayMinDistance = Mathf.Clamp(rayMinDistance, 0, toEnemy.magnitude);
            Vector3 worldPosition = ray.GetPoint(rayMinDistance);
            Vector3 position = _camera.WorldToScreenPoint(worldPosition);
            Quaternion rotation = GetIconRotation(index);

            if (toEnemy.magnitude > rayMinDistance)
            {
                pointerIcon.Show();
            }
            else
            {
                pointerIcon.Hide();
            }

            pointerIcon.SetIconPosition(position, rotation);
        }
    }

    private Quaternion GetIconRotation(int planeIndex)
    {
        switch (planeIndex)
        {
            case 0:
                return Quaternion.Euler(0f, 0f, 90f);
            case 1:
                return Quaternion.Euler(0f, 0f, -90f);
            case 2:
                return Quaternion.Euler(0f, 0f, 180);
            case 3:
                return Quaternion.Euler(0f, 0f, 0f);
            default: return Quaternion.identity;
        }
    }

    private void OnDisable()
    {
        foreach (var pointer in _dictionary)
        {
            pointer.Value.Hide();
        }
    }

    private void OnEnable()
    {
        foreach (var pointer in _dictionary)
        {
            pointer.Value.Show();
        }
    }
}