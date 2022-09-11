using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;
using System;

public class Map : MonoBehaviour
{
    [Label("Generate Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 5)] private float _maxWaitTime = 1.5f;

    [Label("Cell Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [Range(1, 100)]
    [SerializeField] private int _cellSize = 1;

    [Label("Tiles Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [BeginHorizontal]
    [SerializeField] private SerializedDictionary<int, GridObject> _pathObjects;
    [SpaceArea(5)]
    [SerializeField, ReorderableList(Foldable = true)] private GridObject[] _sceneryObjects;
    [EndHorizontal]

    private Grid<GridCell> _grid;
    private Transform _mapParent;
    private Transform _pathParent;
    private Transform _barriersParent;
    private Transform _buildingsParent;

    private PathGenerator _pathGenerator;
    [Inject] private Game _game;
    [Inject] private DiContainer _container;

    public Grid<GridCell> MapGrid => _grid;
    public PathGenerator PathGenerator => _pathGenerator;
    public event Action OnMapGenerated;


    private void Start()
    {
        _grid = new Grid<GridCell>(_game.CurrentLevel.GridWidth, _game.CurrentLevel.GridHeight, _cellSize, (Grid<GridCell> g, int x, int y) => new GridCell(g, x, y));
        _pathGenerator = new PathGenerator(_game.CurrentLevel.GridWidth, _game.CurrentLevel.GridHeight, _game.CurrentLevel.Offset);

        _mapParent = new GameObject("Map").transform;
        _pathParent = new GameObject("Path").transform;
        _barriersParent = new GameObject("Barriers").transform;
        _buildingsParent = new GameObject("Buildings").transform;

        _barriersParent.SetParent(_mapParent);
        _pathParent.SetParent(_mapParent);
        _buildingsParent.SetParent(_mapParent);

        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        List<Vector2Int> path = _pathGenerator.GeneratePath();
        int size = path.Count;

        float timer = 0;

        while (size < UnityEngine.Random.Range(_game.CurrentLevel.PathLength.x, _game.CurrentLevel.PathLength.y))
        {
            path = _pathGenerator.GeneratePath();
            size = path.Count;

            timer += Time.deltaTime;

            if (timer >= _maxWaitTime)
                break;
        }

        yield return StartCoroutine(LayPathObjects(path));

        if (_sceneryObjects.Length > 0)
            yield return StartCoroutine(LaySceneryObjects());

        yield return StartCoroutine(LayBuildingObjects());

        if (_game.CurrentLevel.BarrierRows > 0 && _game.CurrentLevel.BarrierPrefab != null)
            yield return StartCoroutine(LayBarrierObjects());

        OnMapGenerated?.Invoke();
    }

    private IEnumerator LayPathObjects(List<Vector2Int> path)
    {
        foreach (Vector2Int p in path)
        {
            int neighbourValue = _pathGenerator.GetCellNeighbourValue(p.x, p.y);

            if (_pathObjects.ContainsKey(neighbourValue))
            {
                SpawnGridCell(_pathObjects[neighbourValue], p.x, p.y);
            }
            else
            {
                Debug.LogWarning($"Path objects doesn't implement: {neighbourValue} value");
            }

            yield return null;
        }
    }

    private IEnumerator LaySceneryObjects()
    {
        for (int x = 0; x < _game.CurrentLevel.GridWidth; x++)
        {
            for (int y = 0; y < _game.CurrentLevel.GridHeight; y++)
            {
                GridCell cell = _grid.GetObject(x, y);

                if (cell.CanBuild())
                {
                    List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                    int randomCell = UnityEngine.Random.Range(0, _sceneryObjects.Length);

                    if (neighbours.Count > 0)
                        randomCell = 0;

                    SpawnGridCell(_sceneryObjects[randomCell], x, y, _mapParent);
                    yield return null;
                }
            }
        }
    }

    private IEnumerator LayBarrierObjects()
    {
        for (int x = 0; x < _game.CurrentLevel.GridWidth; x++)
        {
            for (int y = -1; y > -_game.CurrentLevel.BarrierRows; y--)
            {
                SpawnGridCell(_game.CurrentLevel.BarrierPrefab, x, y, _barriersParent);
                yield return null;
            }
        }

        for (int x = 0; x < _game.CurrentLevel.GridWidth; x++)
        {
            for (int y = _game.CurrentLevel.GridHeight; y < (_game.CurrentLevel.GridHeight + _game.CurrentLevel.BarrierRows); y++)
            {
                SpawnGridCell(_game.CurrentLevel.BarrierPrefab, x, y, _barriersParent);
                yield return null;
            }

        }

        for (int x = -1; x > -_game.CurrentLevel.BarrierRows; x--)
        {
            for (int y = (-_game.CurrentLevel.BarrierRows + 1); y < (_game.CurrentLevel.GridHeight + _game.CurrentLevel.BarrierRows); y++)
            {
                SpawnGridCell(_game.CurrentLevel.BarrierPrefab, x, y, _barriersParent);
                yield return null;
            }
        }

        for (int x = _game.CurrentLevel.GridWidth; x < (_game.CurrentLevel.GridWidth + _game.CurrentLevel.BarrierRows); x++)
        {
            for (int y = (-_game.CurrentLevel.BarrierRows + 1); y < (_game.CurrentLevel.GridHeight + _game.CurrentLevel.BarrierRows); y++)
            {
                SpawnGridCell(_game.CurrentLevel.BarrierPrefab, x, y, _barriersParent);
                yield return null;
            }
        }
    }

    private IEnumerator LayBuildingObjects()
    {
        SpawnBase();

        foreach (var b in _game.CurrentLevel.BuildingObjects)
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
        roadEndPoint.y -= 1;

        var headquartes = SpawnBuilding(_game.CurrentLevel.BasePrefab, roadEndPoint).GetComponent<Headquarters>();

        _container.Bind<Headquarters>().FromInstance(headquartes).AsSingle();
    }

    private GameObject SpawnBuilding(GridBuilding info, Vector2Int point)
    {
        if (info.Prefab == null)
        {
            Debug.LogWarning($"{info.name} prefab not found!");
            return null;
        }

        Vector3 worldPosition = _grid.GetWorldPosition(point.x, point.y);
        worldPosition.y = info.YOffset;

        GameObject building = _container.InstantiatePrefab(info.Prefab, worldPosition, Quaternion.Euler(0f, info.Rotation, 0f), _buildingsParent);

        OccupyCells(point, new Vector2Int(info.Size.x, info.Size.y), building);

        return building;
    }


    private void SpawnGridCell(GridObject gridObject, int x, int y, Transform parent = null)
    {

        Vector3 worldPosition = _grid.GetWorldPosition(x, y);

        Transform gameObj = Instantiate(gridObject.Prefab, worldPosition, Quaternion.Euler(0f, gridObject.Rotation, 0f)).transform;

        if (parent == null)
            parent = _mapParent;
        gameObj.SetParent(parent);
        gameObj.name = $"[{x}, {y}]";

        GridCell gridCell = _grid.GetObject(x, y);

        if (gridCell != null)
        {
            gridCell.SetType(gridObject.Type);
        }
    }


    private void OccupyCells(Vector2Int pos, Vector2Int size, GameObject obj)
    {
        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                int nextX = pos.x + x;
                int nextY = pos.y + y;

                if (nextX >= _game.CurrentLevel.GridWidth || nextY >= _game.CurrentLevel.GridHeight)
                    return;

                GridCell gridCell = _grid.GetObject(nextX, nextY);
                gridCell.SetTransform(obj.transform);
            }
        }
    }
    public bool CellsIsEmpty(Vector2Int pos, Vector2Int size)
    {
        for (int x = 0; x < size.x; ++x)
        {
            for (int y = 0; y < size.y; ++y)
            {
                int nextX = pos.x + x;
                int nextY = pos.y + y;

                if (nextX >= _game.CurrentLevel.GridWidth || nextY >= _game.CurrentLevel.GridHeight)
                    return false;

                if (_grid.GetObject(nextX, nextY).CanBuild() == false) return false;
            }
        }

        return true;
    }
    public List<Vector2Int> GetEmptyCells(Vector2Int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = 1; x < (_game.CurrentLevel.GridWidth - 1); x++)
        {
            for (int y = 1; y < (_game.CurrentLevel.GridHeight - 1); y++)
            {
                GridCell gridCell = _grid.GetObject(x, y);

                if (gridCell.CanBuild())
                {
                    List<Vector2Int> neighbours = _pathGenerator.GetCellNeighbours(x, y);

                    if (neighbours.Count > 0)
                        continue;

                    if (CellsIsEmpty(new Vector2Int(x, y), new Vector2Int(size.x, size.y)))
                    {
                        positions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return positions;
    }
}
