using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UI_GRADE : MonoBehaviour
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
        bool canInteract = _player.Inventory.CanUpgrade;

        _group.alpha = canInteract ? 1 : 0;
        _group.interactable = canInteract;
    }

    private void Upgrade()
    {
        _player.Inventory.Upgrade();
    }
}
