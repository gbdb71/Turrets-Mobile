using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;

public class MapGenerator : MonoBehaviour
{

    // TODO : REMOVE 
    [SerializeField] public LevelData levelData;

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

        while (size < UnityEngine.Random.Range(levelData.PathLength.x, levelData.PathLength.y))
        {
            path = _pathGenerator.GeneratePath();
            size = path.Count;
        }

        yield return StartCoroutine(LayPathCells(path));

        if (_sceneryCellObjects.Length > 0)
            yield return StartCoroutine(LaySceneryCells());

        yield return StartCoroutine(LayBuildingCells());

        if (levelData.BarrierRows > 0 && levelData.BarrierPrefab != null)
            yield return StartCoroutine(LayBarriersCells());

    }

    private IEnumerator LayPathCells(List<Vector2Int> path)
    {
        foreach (Vector2Int p in path)
        {
            int neighbourValue = _pathGenerator.GetCellNeighbourValue(p.x, p.y);

            if (_pathCellObjects.ContainsKey(neighbourValue))
            {
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
        for (int x = 0; x < levelData.GridWidth; x++)
        {
            for (int y = -1; y > -levelData.BarrierRows; y--)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(levelData.BarrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }
        }

        for (int x = 0; x < levelData.GridWidth; x++)
        {
            for (int y = levelData.GridHeight; y < (levelData.GridHeight + levelData.BarrierRows); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(levelData.BarrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }

        }

        for (int x = -1; x > -levelData.BarrierRows; x--)
        {
            for (int y = (-levelData.BarrierRows + 1); y < (levelData.GridHeight + levelData.BarrierRows); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(levelData.BarrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }
        }

        for (int x = levelData.GridWidth; x < (levelData.GridWidth + levelData.BarrierRows); x++)
        {
            for (int y = (-levelData.BarrierRows + 1); y < (levelData.GridHeight + levelData.BarrierRows); y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                position *= _cellSize;

                GameObject cell = Instantiate(levelData.BarrierPrefab.CellPrefab, position, Quaternion.identity, _barriersParent);
                cell.name = $"[{x}, {y}]";

                yield return null;
            }
        }
    }

    private IEnumerator LayBuildingCells()
    {
        BuildBase();

        foreach (var b in levelData.BuildingObjects)
        {
            Vector2Int size = b.Size;

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
                _container.InstantiatePrefab(b.CellPrefab, new Vector3(pos.x, b.Offset, pos.y), Quaternion.identity, _buildingsParent);
            }
        }

        yield return null;
    }
    private void BuildBase()
    {
        Vector2Int p = _pathGenerator.PathCells[_pathGenerator.PathCells.Count - 1];


        BuildingObject basePrefab = levelData.BasePrefab;
        OccupyTiles(p.x, p.y, basePrefab.Size.x, basePrefab.Size.y);

        p *= _cellSize;
        _container.InstantiatePrefab(basePrefab.CellPrefab, new Vector3(p.x, basePrefab.Offset, p.y), Quaternion.identity, _buildingsParent);
    }


    private bool TilesIsEmpty(int posX, int posY, int sizeX, int sizeY)
    {
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                int nextX = posX + x;
                int nextY = posY + y;

                if (nextX >= levelData.GridWidth || nextY >= levelData.GridHeight)
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
