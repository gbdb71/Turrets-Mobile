using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;
using System.Collections;
using Zenject;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField, Range(1, 100)] private int _gridWidth;
    [SerializeField, Range(1, 100)] private int _gridHeight;

    [Header("Cell Settings")]
    [Range(1, 100)]
    [SerializeField] private int _cellSize = 1;

    [Header("Path Settings")]
    [SerializeField, MinMaxSlider(15, 50)] private Vector2Int _pathLength;
    [SerializeField] private Offset _offset;

    [Header("Tiles")]
    [SerializeField] private GridCellObject[] _pathCellObjects;
    [SerializeField] private GridCellObject[] _sceneryCellObjects;
    [SerializeField] private GridCellObject _barrierPrefab;

    [Header("Buildings")]
    [SerializeField] private BuildingObject _basePrefab;
    [SerializeField] private List<BuildingObject> _buildingObjects;

    [Header("Barrier")]
    [SerializeField] private int _barrierRows;

    private Transform _mapParent;
    private Transform _pathParent;
    private Transform _barriersParent;
    private Transform _buildingsParent;

    private PathGenerator _pathGenerator;
    private Cell[,] _cells;
    [Inject] private DiContainer _container;

    private void Start()
    {
        _pathGenerator = new PathGenerator(_gridWidth, _gridHeight, _offset);

        _mapParent = new GameObject("Map").transform;
        _pathParent = new GameObject("Path").transform;
        _barriersParent = new GameObject("Barriers").transform;
        _buildingsParent = new GameObject("Buildings").transform;

        _barriersParent.SetParent(_mapParent);
        _pathParent.SetParent(_mapParent);
        _buildingsParent.SetParent(_mapParent);

        _cells = new Cell[_gridWidth, _gridHeight];

        StartCoroutine(GeneratePath());
    }

    private IEnumerator GeneratePath()
    {
        List<Vector2Int> path = _pathGenerator.GeneratePath();
        int size = path.Count;

        while (size < UnityEngine.Random.Range(_pathLength.x, _pathLength.y))
        {
            path = _pathGenerator.GeneratePath();
            size = path.Count;
        }

        yield return StartCoroutine(LayPathCells(path));

        if (_sceneryCellObjects.Length > 0)
            yield return StartCoroutine(LaySceneryCells());

        yield return StartCoroutine(LayBuildingCells());

        if (_barrierPrefab != null)
            yield return StartCoroutine(LayBarriersCells());

    }

    private IEnumerator LayPathCells(List<Vector2Int> path)
    {
        foreach (Vector2Int p in path)
        {
            int neighbourValue = _pathGenerator.GetCellNeighbourValue(p.x, p.y);
            GameObject cellPrefab = _pathCellObjects[neighbourValue].CellPrefab;

            Vector3 position = new Vector3(p.x, 0f, p.y);
            position *= _cellSize;

            GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, _pathParent);
            cell.name = $"[{p.x}, {p.y}]";
            cell.transform.rotation = Quaternion.Euler(0f, _pathCellObjects[neighbourValue].Rotation, 0f);

            _cells[p.x, p.y] = new Cell
            {
                Object = cell,
                Type = _pathCellObjects[neighbourValue].CellType
            };

            yield return null;
        }
    }

    private IEnumerator LaySceneryCells()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                if (_pathGenerator.CellIsEmpty(x, y))
                {
                    List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                    int randomCell = UnityEngine.Random.Range(0, _sceneryCellObjects.Length);

                    if (neighbours.Count > 0)
                        randomCell = 0;

                    Vector3 position = new Vector3(x, 0, y);
                    position *= _cellSize;

                    GameObject cell = Instantiate(_sceneryCellObjects[randomCell].CellPrefab, position, Quaternion.identity, _mapParent);
                    cell.name = $"[{x}, {y}]";

                    _cells[x, y] = new Cell
                    {
                        Object = cell,
                        Type = _sceneryCellObjects[randomCell].CellType
                    };

                    yield return null;
                }
            }
        }
    }

    private IEnumerator LayBarriersCells()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = -1; y > -_barrierRows; y--)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(_barrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }
        }

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = _gridHeight; y < (_gridHeight + _barrierRows); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(_barrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }

        }

        for (int x = -1; x > -_barrierRows; x--)
        {
            for (int y = (-_barrierRows + 1); y < (_gridHeight + _barrierRows); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(_barrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }
        }

        for (int x = _gridWidth; x < (_gridWidth + _barrierRows); x++)
        {
            for (int y = (-_barrierRows + 1); y < (_gridHeight + _barrierRows); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(_barrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }
        }
    }


    private IEnumerator LayBuildingCells()
    {
        BuildBase();

        foreach (var b in _buildingObjects)
        {
            Vector2Int size = b.Size;

            List<Vector2Int> positions = new List<Vector2Int>();

            for (int x = 1; x < (_gridWidth - 1); x++)
            {
                for (int y = 1; y < (_gridHeight - 1); y++)
                {
                    if (_cells[x, y].Type == CellType.Ground)
                    {
                        List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                        if (neighbours.Count > 0)
                            continue;

                        if (TilesIsEmpty(x, y, size.x, size.y))
                        {
                            positions.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }

            if (positions.Count > 0)
            {
                var pos = positions[Random.Range(0, positions.Count)];
                OccupyTiles(pos.x, pos.y, size.x, size.y);

                pos *= _cellSize;
                Instantiate(b.CellPrefab, new Vector3(pos.x, b.Offset, pos.y), Quaternion.identity, _buildingsParent);
            }
        }

        yield return null;
    }
    private void BuildBase()
    {
        Vector2Int p = _pathGenerator.PathCells[_pathGenerator.PathCells.Count - 1];


        OccupyTiles(p.x, p.y, _basePrefab.Size.x, _basePrefab.Size.y);
        p *= _cellSize;
        _container.InstantiatePrefab(_basePrefab.CellPrefab, new Vector3(p.x, _basePrefab.Offset, p.y), Quaternion.identity, _buildingsParent);
    }


    private bool TilesIsEmpty(int posX, int posY, int sizeX, int sizeY)
    {
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                int nextX = posX + x;
                int nextY = posY + y;

                if (nextX >= _gridWidth || nextY >= _gridHeight)
                    return false;

                if (_cells[nextX, nextY].IsBusy) return false;
            }
        }

        return true;
    }

    private void OccupyTiles(int posX, int posY, int sizeX, int sizeY)
    {
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                int nextX = posX + x;
                int nextY = posY + y;

                _cells[nextX, nextY].IsBusy = true;
            }
        }
    }
}
