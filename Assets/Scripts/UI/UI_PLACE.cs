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
        bool canInteract = _player.Inventory.HasTurret && !_player.Inventory.CanUpgrade && _player.Inventory.CanPlace;

        _group.alpha = canInteract ? 1 : 0;
        _group.blocksRaycasts = canInteract;
    }

    private void Place()
    {
        _player.Inventory.Place();
    }
}
