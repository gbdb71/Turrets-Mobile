using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    [Inject] private Map _mapGenerator;
    [Inject] private Player _player;

    private Headquarters _headquarters;

    private void Awake()
    {
        Map.OnMapGenerated += SpawnPlayer;
    }

    private void OnDestroy()
    {
        Map.OnMapGenerated -= SpawnPlayer;
    }

    private void SpawnPlayer()
    {
        _headquarters = FindObjectOfType<Headquarters>();

        List<Vector2Int> points = _mapGenerator.GetEmptyCells(Vector2Int.one, ignorePathCells: true);

        _mapGenerator.MapGrid.GetXY(_headquarters.transform.position, out int x, out int y);

        Vector2Int targetPos = new Vector2Int(x, y);
        points.Sort((x, y) => { return (targetPos - x).sqrMagnitude.CompareTo((targetPos - y).sqrMagnitude); });
        Vector2Int spawnXZ = points.First();

        Vector3 worldSpawn = _mapGenerator.MapGrid.GetWorldPosition(spawnXZ.x, spawnXZ.y);
        worldSpawn.y = 1f;

        _player.transform.position = worldSpawn;
        _player.gameObject.SetActive(true);
    }
}
