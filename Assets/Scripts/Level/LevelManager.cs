using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [Inject] private Data _data;
    [SerializeField] private List<LevelData> _levels = new List<LevelData>();

    public LevelData CurrentLevel => _levels[_data.CurrentLevel];

    public void NextLevel()
    {
        _data.CurrentLevel++;

        if(_data.CurrentLevel >= _levels.Count)
        {
            _data.CurrentLevel = 0;
        }
    }
}

