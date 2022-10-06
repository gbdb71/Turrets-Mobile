using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AbillitySystem : MonoBehaviour
{
    [SerializeField] private List<BaseAbillity> _abillities;
    [SerializeField] private Transform _content;
    [SerializeField] private Transform _cardsParent;

    [Inject] private Joystick _joystick;
    [Inject] private DiContainer _container;


    private void Awake()
    {
        LoadAbillities();
        LevelScenario.OnWaveChanged += ShowPanel;
    }

    private void LoadAbillities()
    {
        var abillities = Resources.LoadAll<BaseAbillity>("GameData/Abillities");

        for (int i = 0; i < abillities.Length; i++)
        {
            BaseAbillity card = _container.InstantiatePrefabForComponent<BaseAbillity>(abillities[i], _cardsParent);
            card.SetSystem(this);
            card.gameObject.SetActive(false);

            _abillities.Add(card);
        }
    }

    private void OnDestroy()
    {
        LevelScenario.OnWaveChanged -= ShowPanel;

        for (int i = 0; i < _abillities.Count; i++)
        {
            _abillities[i].Clear();
        }
    }

    public void OnAbillityActivated(BaseAbillity abillity)
    {
        _content.gameObject.SetActive(false);
        _joystick.gameObject.SetActive(true);
    }

    public void RemoveAbillity(BaseAbillity abillity)
    {
        if(abillity != null)
        {
            if(_abillities.Contains(abillity))
                _abillities.Remove(abillity);

            Destroy(abillity.gameObject);
        }
    }

    private void ShowPanel()
    {
        _content.gameObject.SetActive(true);

        ShowCards();
    }

    private void ShowCards()
    {
        int _activated = 0;
        bool moneyShowed = false;

        while (_activated < 3)
        {
            BaseAbillity abillity = _abillities[Random.Range(0, _abillities.Count)];

            if ((abillity as MoneyAbillity) == null)
            {
                if (!moneyShowed)
                    continue;
            }
            else
            {
                if (moneyShowed)
                    continue;

                moneyShowed = true;
            }

            if (!abillity.gameObject.activeSelf)
            {
                abillity.gameObject.SetActive(true);
                _activated++;
            }
        }

        _joystick.gameObject.SetActive(false);
    }
}
