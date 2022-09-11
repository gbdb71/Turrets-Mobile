using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [Inject] private Map _mapGenerator;
    [Inject] private Player _player;
  
    private void Awake()
    {
        if (_mapGenerator != null)
            _mapGenerator.OnMapGenerated += SpawnPlayer;
    }

    private void SpawnPlayer()
    {
        //List<Vector2Int> points = _mapGenerator.GetEmptyCells(Vector2Int.one);

        //int x, y;

        // _mapGenerator.MapGrid.GetXY(_headquarters.transform.position, out x, out y);

        //Vector2Int targetPos = new Vector2Int(x, y);
        //points.Sort((x, y) => { return (targetPos - x).sqrMagnitude.CompareTo((targetPos - y).sqrMagnitude); });

        //Vector2Int spawPoint = points.First();

        //_player.transform.position = _mapGenerator.MapGrid.GetWorldPosition(spawPoint.x, spawPoint.y);
        //_player.gameObject.SetActive(true);
    }
}
