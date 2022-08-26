using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UI_PLACE : MonoBehaviour
{
    [Inject]
    private Player _player;

    private CanvasGroup _group;
    private Button _button;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        _button = GetComponent<Button>();
        _button.onClick.AddListener(Place);
    }

    private void Update()
    {
        _group.alpha = _player.Inventory.HasTurret ? 1 : 0;
    }

    private void Place()
    {
        _player.Inventory.Place();
    }
}
