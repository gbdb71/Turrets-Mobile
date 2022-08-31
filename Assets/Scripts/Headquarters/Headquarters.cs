using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Headquarters : MonoBehaviour
{
    [Header("DATA Settings")]
    public UpgradesInfo upgradesInfo;
    public List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();

    [Header("Siew Settings")]
    public CanvasGroup baseGroup;

    [Inject]
    [SerializeField] private Player _player;

    public void Awake()
    {
        baseGroup.alpha = 0;
     //   InitializationButtons();
    }

    public void Start()
    {
        InitializationButtons();
    }

    #region Buttons
    private void InitializationButtons()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < upgradesInfo.upgrades.Count && upgradesInfo.upgrades[i] != null)
                upgradeButtons[i].Initialization(upgradesInfo.upgrades[i], this);
            else
            {
                Debug.Log($"No More Upgrade Info {i}");
                upgradeButtons[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    public void LoadData()
    {
        _player.Movement.Speed = PlayerPrefs.GetFloat($"Upgrade{UpgradeList.UpgradeType.Speed}"); ;
        _player.Movement.SpeedWithTurret = PlayerPrefs.GetFloat($"Upgrade{UpgradeList.UpgradeType.SpeedWithTurret}"); ;
    }

    public void ValuePassing(UpgradeList.UpgradeType type, float value)
    {
        switch(type)
        {
            case UpgradeList.UpgradeType.Speed:
                _player.Movement.Speed = value;
                break;

            case UpgradeList.UpgradeType.SpeedWithTurret:
                _player.Movement.SpeedWithTurret = value;
                break;

            case UpgradeList.UpgradeType.AmmoCount:

                break;

        }
    }

    #endregion

    #region View UI
    public void OpenViewGroup()
    {
        baseGroup.alpha = 1;
    }

    public void ClosedViewGroup()
    {
        baseGroup.alpha = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerInventory player))
        {
            Debug.Log(player);
            if (!player.HasTurret)
                OpenViewGroup();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerInventory player))
        {
            //if (!player.HasTurret)
            ClosedViewGroup();
        }
    }
    #endregion
}
