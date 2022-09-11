using System;
using UnityEngine;

public class Grid<TGridObject>
{
    public event EventHandler<OnGridChangedEventArgs> OnGridChanged;
    public class OnGridChangedEventArgs : EventArgs
    {
        public int X;
        public int Y;
        public TGridObject Object;
    }

    private int _width;
    private int _height;

    private float _cellSize;

    private TGridObject[,] _grid;

    public Grid(int width, int height, float cellSize, Func<Grid<TGridObject>, int, int, TGridObject> createObject)
    {
        this._width = width;
        this._height = height;

        this._cellSize = cellSize;

        this._grid = new TGridObject[_width, _height];

        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                _grid[x, y] = createObject(this, x, y);
            }
        }
    }


    public void SetObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetObject(x, y, value);
    }
    public void SetObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            _grid[x, y] = value;

            if (OnGridChanged != null)
                OnGridChanged(this, new OnGridChangedEventArgs { X = x, Y = y, Object = GetObject(x, y) });
        }
    }
    public TGridObject GetObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            return _grid[x, y];
        }
        else
            return default(TGridObject);
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if(OnGridChanged != null) OnGridChanged(this, new OnGridChangedEventArgs { X = x, Y = y, Object = GetObject(x, y)});
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0f, y) * _cellSize;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x);
        y = Mathf.FloorToInt(worldPosition.z);
    }
}

