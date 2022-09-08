using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private MapGenerator _mapGenerator;

    [Inject] private Player _player;

    private void Awake()
    {
        if (_mapGenerator != null)
            _mapGenerator.OnMapGenerated += SpawnPlayer;
    }

    private void SpawnPlayer()
    {
        List<Vector2Int> points = _mapGenerator.GetEmptyCells(Vector2Int.one);

        Vector2Int targetPos = _mapGenerator.GetBuildingsCells<Headquarters>().FirstOrDefault().Position;
        points.Sort((x, y) => { return (targetPos - x).sqrMagnitude.CompareTo((targetPos - y).sqrMagnitude); });

        Vector2Int spawPoint = points.First();
        spawPoint *= _mapGenerator.CellSize;

        _player.transform.position = new Vector3(spawPoint.x, 1f, spawPoint.y);
        _player.gameObject.SetActive(true);
    }
}
