using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UI_TAKE : MonoBehaviour
{
    [Inject]
    private Player _player;

    private CanvasGroup _group;
    private Button _button;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        _button = GetComponent<Button>();
        _button.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
        PlayerInventory playerInventory = _player.Inventory;
        TurretPlace nearPlace = playerInventory.NearPlace;

        bool canInteract = !playerInventory.HasTurret && nearPlace != null && nearPlace.HasTurret ||
                           playerInventory.HasTurret && nearPlace != null && nearPlace.CanPlace && nearPlace.PlacedTurret != null && nearPlace.PlacedTurret.NextGrade != playerInventory.TakedTurret.NextGrade;

        _group.alpha = canInteract ? 1 : 0;
        _group.blocksRaycasts = canInteract;
    }

    private void Upgrade()
    {
        _player.Inventory.Take();
    }
}
