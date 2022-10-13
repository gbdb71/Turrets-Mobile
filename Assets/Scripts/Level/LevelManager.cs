using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [Inject] private Data _data;
    [SerializeField] private List<LevelData> _levels = new List<LevelData>();

    public LevelData CurrentLevel => _levels[_data.User.CurrentLevel];

    public void NextLevel()
    {
        _data.User.CurrentLevel++;

        if(_data.User.CurrentLevel >= _levels.Count)
        {
            _data.User.CurrentLevel = 0;
        }
    }
}

