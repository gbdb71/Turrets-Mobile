using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField, Range(1, 100)] private int _gridWidth;
    [SerializeField, Range(1, 100)] private int _gridHeight;

    [Header("Path Settings")]
    [SerializeField, MinMaxSlider(15, 50)] private Vector2Int _pathLength;
    [SerializeField] private Offset _offset;

    [Header("Tiles")]
    [SerializeField] private GridCellObject[] _pathCellObjects;
    [SerializeField] private GridCellObject[] _sceneryCellObjects;

    private Transform _mapParent;
    private Transform _pathParent;

    private PathGenerator _pathGenerator;

    private void Start()
    {
        _pathGenerator = new PathGenerator(_gridWidth, _gridHeight, _offset);

        _mapParent = new GameObject("Map").transform;
        _pathParent = new GameObject("Path").transform;
        _pathParent.SetParent(_mapParent);

        GeneratePath();
    }

    private void GeneratePath()
    {
        if (_pathLength == Vector2Int.zero)
            return;

        List<Vector2Int> path = _pathGenerator.GeneratePath();
        int size = path.Count;

        while (size < Random.Range(_pathLength.x, _pathLength.y))
        {
            path = _pathGenerator.GeneratePath();
            size = path.Count;
        }

        LayPathCells(path);
        LaySceneryCells(); 
    }

    private void LayPathCells(List<Vector2Int> path)
    {
        foreach (Vector2Int p in path)
        {
            int neighbourValue = _pathGenerator.GetCellNeighbourValue(p.x, p.y);
            GameObject cellPrefab = _pathCellObjects[neighbourValue].CellPrefab;

            GameObject cell = Instantiate(cellPrefab, new Vector3(p.x, 0f, p.y), Quaternion.identity, _pathParent);
            cell.name = $"[{p.x}, {p.y}]";
            cell.transform.rotation = Quaternion.Euler(0f, _pathCellObjects[neighbourValue].Rotation, 0f);
        }
    }

    private void LaySceneryCells()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                if (_pathGenerator.CellIsEmpty(x, y))
                {
                    List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                    int randomCell = Random.Range(0, _sceneryCellObjects.Length);

                    if (neighbours.Count > 0)
                        randomCell = 0;

                    GameObject cell = Instantiate(_sceneryCellObjects[randomCell].CellPrefab, new Vector3(x, 0, y), Quaternion.identity, _mapParent);
                    cell.name = $"[{x}, {y}]";
                }
            }
        }
    }

}
