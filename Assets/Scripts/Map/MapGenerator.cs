using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;
using System;

public class MapGenerator : MonoBehaviour
{
    // TODO : REMOVE 
    [SerializeField] public LevelData levelData;
    [Label("Generate Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 5)] private float _maxWaitTime = 1.5f;

    [Label("Cell Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [Range(1, 100)]
    [SerializeField] private int _cellSize = 1;

    [Label("Tiles Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [BeginHorizontal]
    [SerializeField] private SerializedDictionary<int, GridCellObject> _pathCellObjects;
    [SpaceArea(5)]
    [SerializeField, ReorderableList(Foldable = true)] private GridCellObject[] _sceneryCellObjects;
    [EndHorizontal]


    private Transform _mapParent;
    private Transform _pathParent;
    private Transform _barriersParent;
    private Transform _buildingsParent;

    private PathGenerator _pathGenerator;
    private Cell[,] _cells;
    [Inject] private DiContainer _container;


    public Cell[,] Cells => _cells;
    public int CellSize => _cellSize;
    public PathGenerator PathGenerator => _pathGenerator;
    public event Action OnMapGenerated;


    private void Start()
    {
        _pathGenerator = new PathGenerator(levelData.GridWidth, levelData.GridHeight, levelData.Offset);

        _mapParent = new GameObject("Map").transform;
        _pathParent = new GameObject("Path").transform;
        _barriersParent = new GameObject("Barriers").transform;
        _buildingsParent = new GameObject("Buildings").transform;

        _barriersParent.SetParent(_mapParent);
        _pathParent.SetParent(_mapParent);
        _buildingsParent.SetParent(_mapParent);

        _cells = new Cell[levelData.GridWidth, levelData.GridHeight];

        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        List<Vector2Int> path = _pathGenerator.GeneratePath();
        int size = path.Count;

        float timer = 0;

        while (size < UnityEngine.Random.Range(levelData.PathLength.x, levelData.PathLength.y))
        {
            path = _pathGenerator.GeneratePath();
            size = path.Count;

            timer += Time.deltaTime;

            if (timer >= _maxWaitTime)
                break;
        }

        yield return StartCoroutine(LayPathCells(path));

        if (_sceneryCellObjects.Length > 0)
            yield return StartCoroutine(LaySceneryCells());

        yield return StartCoroutine(LayBuildingCells());

        if (levelData.BarrierRows > 0 && levelData.BarrierPrefab != null)
            yield return StartCoroutine(LayBarriersCells());

        OnMapGenerated?.Invoke();
    }

    private IEnumerator LayPathCells(List<Vector2Int> path)
    {
        foreach (Vector2Int p in path)
        {
            int neighbourValue = _pathGenerator.GetCellNeighbourValue(p.x, p.y);

            if (_pathCellObjects.ContainsKey(neighbourValue))
            {
                GridCellObject info = _pathCellObjects[neighbourValue];

                Cell cell = SpawnCell(info, p);

                _cells[p.x, p.y] = cell;
            }

            yield return null;
        }
    }

    private IEnumerator LaySceneryCells()
    {
        for (int x = 0; x < levelData.GridWidth; x++)
        {
            for (int y = 0; y < levelData.GridHeight; y++)
            {
                if (_pathGenerator.CellIsEmpty(x, y))
                {
                    List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                    int randomCell = UnityEngine.Random.Range(0, _sceneryCellObjects.Length);

                    if (neighbours.Count > 0)
                        randomCell = 0;

                    Cell cell = SpawnCell(_sceneryCellObjects[randomCell], new Vector2Int(x, y));

                    _cells[x, y] = cell;
                    yield return null;
                }
            }
        }
    }

    private IEnumerator LayBarriersCells()
    {
        for (int x = 0; x < levelData.GridWidth; x++)
        {
            for (int y = -1; y > -levelData.BarrierRows; y--)
            {
                SpawnCell(levelData.BarrierPrefab, new Vector2Int(x, y));
                yield return null;
            }
        }

        for (int x = 0; x < levelData.GridWidth; x++)
        {
            for (int y = levelData.GridHeight; y < (levelData.GridHeight + levelData.BarrierRows); y++)
            {
                SpawnCell(levelData.BarrierPrefab, new Vector2Int(x, y));
                yield return null;
            }

        }

        for (int x = -1; x > -levelData.BarrierRows; x--)
        {
            for (int y = (-levelData.BarrierRows + 1); y < (levelData.GridHeight + levelData.BarrierRows); y++)
            {
                SpawnCell(levelData.BarrierPrefab, new Vector2Int(x, y));
                yield return null;
            }
        }

        for (int x = levelData.GridWidth; x < (levelData.GridWidth + levelData.BarrierRows); x++)
        {
            for (int y = (-levelData.BarrierRows + 1); y < (levelData.GridHeight + levelData.BarrierRows); y++)
            {
                SpawnCell(levelData.BarrierPrefab, new Vector2Int(x, y));
                yield return null;
            }
        }
    }

    private IEnumerator LayBuildingCells()
    {
        SpawnBase();

        foreach (var b in levelData.BuildingObjects)
        {
            Vector2Int size = b.Size;

            List<Vector2Int> positions = GetEmptyCells(b.Size);

            if (positions.Count > 0)
            {
                var pos = positions[UnityEngine.Random.Range(0, positions.Count)];

                SpawnBuilding(b, pos);
            }
        }

        yield return null;
    }

    private void SpawnBase()
    {
        Vector2Int roadEndPoint = _pathGenerator.PathCells[_pathGenerator.PathCells.Count - 1];
        var headquartes = SpawnBuilding(levelData.BasePrefab, roadEndPoint).GetComponent<Headquarters>();

        _container.Bind<Headquarters>().FromInstance(headquartes).AsSingle();
    }


    private GameObject SpawnBuilding(BuildingObject info, Vector2Int point)
    {
        if (info.CellPrefab == null)
        {
            Debug.LogWarning($"{info.name} prefab not found!");
            return null;
        }

        GameObject building = _container.InstantiatePrefab(info.CellPrefab, new Vector3(point.x * _cellSize, info.Offset, point.y * _cellSize), Quaternion.identity, _buildingsParent);
       
        OccupyTiles(point, new Vector2Int(info.Size.x, info.Size.y), building);

        return building;
    }
    private Cell SpawnCell(GridCellObject info, Vector2Int point)
    {
        if(info.CellPrefab == null)
        {
            Debug.LogWarning($"{info.name} prefab not found!");
            return null;
        }

        Cell cellPrefab = info.CellPrefab.GetComponent<Cell>();

        Vector3 position = new Vector3(point.x, 0, point.y);
        position *= _cellSize;

        Cell cell = Instantiate(cellPrefab, position, Quaternion.identity, _pathParent);
        cell.name = $"[{point.x}, {point.y}]";
        cell.transform.rotation = Quaternion.Euler(0f, info.Rotation, 0f);

        cell.Initialize(info.CellType, point);


        return cell;
    }
    private void OccupyTiles(Vector2Int pos, Vector2Int size, GameObject obj)
    {
        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                int nextX = pos.x + x;
                int nextY = pos.y + y;

                if (nextX >= levelData.GridWidth || nextY >= levelData.GridHeight)
                    return;

                _cells[nextX, nextY].SetObject(obj);
            }
        }
    }


    public bool TilesIsEmpty(Vector2Int pos, Vector2Int size)
    {
        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                int nextX = pos.x + x;
                int nextY = pos.y + y;

                if (nextX >= levelData.GridWidth || nextY >= levelData.GridHeight)
                    return false;

                if (_cells[nextX, nextY].IsBusy) return false;
            }
        }

        return true;
    }
    public List<Vector2Int> GetEmptyCells(Vector2Int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = 1; x < (levelData.GridWidth - 1); x++)
        {
            for (int y = 1; y < (levelData.GridHeight - 1); y++)
            {
                if (_cells[x, y].Type == CellType.Ground)
                {
                    List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                    if (neighbours.Count > 0)
                        continue;

                    if (TilesIsEmpty(new Vector2Int(x, y), new Vector2Int(size.x, size.y)))
                    {
                        positions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return positions;
    }

    public List<Cell> GetBuildingsCells<T>() where T : MonoBehaviour
    {
        List<Cell> buildings = new List<Cell>();

        for (int x = 0; x < Cells.GetLength(0); x++)
        {
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                var cell = Cells[x, y];

                if (cell.Object != null)
                {
                    if (cell.Object.TryGetComponent(out T component))
                        buildings.Add(cell);
                }
            }
        }

        return buildings;
    }
}
