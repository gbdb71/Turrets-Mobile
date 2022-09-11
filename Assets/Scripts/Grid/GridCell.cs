using UnityEngine;

public class GridCell
{
    private CellType _cellType;
    private Grid<GridCell> _grid;
    private Transform _transform;
    private int _x, _y;

    public CellType Type => _cellType;

    public GridCell(Grid<GridCell> grid, int x, int y, CellType type = CellType.Ground)
    {
       this._grid = grid;
       this._x = x;
       this._y = y;
       this._cellType = type;
    }

    public void SetTransform(Transform transform)
    {
        this._transform = transform;
        _grid.TriggerGridObjectChanged(_x, _y);
    }
    public void ClearTransform()
    {
        this._transform = null;
        _grid.TriggerGridObjectChanged(_x, _y);
    }

    public void SetType(CellType type)
    {
        this._cellType = type;
    }

    public bool CanBuild()
    {
        return _transform == null;
    }
}

