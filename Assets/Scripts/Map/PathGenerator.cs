using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Offset
{
    public int Top;
    public int Bottom;
    public int Right;

    public Offset(int top, int bottom, int right)
    {
        Top = top;
        Bottom = bottom;
        Right = right;
    }
}

public class PathGenerator : MonoBehaviour
{
    private readonly int _width, _height;
    private readonly Offset _offset;

    public List<Vector2Int> PathCells { get; private set; }

    public PathGenerator(int width, int height, Offset offset)
    {
        _width = width;
        _height = height;
        _offset = offset;
    }

    public List<Vector2Int> GeneratePath()
    {
        PathCells = new List<Vector2Int>();

        int x = 0;
        int y = (int)(_height / 2);

        while (x < (_width - _offset.Right))
        {
            PathCells.Add(new Vector2Int(x, y));

            bool validMove = false;

            while (!validMove)
            {
                int move = Random.Range(0, 3);

                if (move == 0 || x % 2 == 0 || x > ((_width - _offset.Right) - 2))
                {
                    x++;
                    validMove = true;
                }
                else if (move == 1 && CellIsEmpty(x, y + 1) && y < (_height - _offset.Top))
                {
                    y++;
                    validMove = true;
                }
                else if (move == 2 && CellIsEmpty(x, y - 1) && y > _offset.Bottom)
                {
                    y--;
                    validMove = true;
                }
            }
        }

        return PathCells;
    }

    public bool CellIsEmpty(int x, int y) => !PathCells.Contains(new Vector2Int(x, y));
    public bool CellIsTaken(int x, int y) => PathCells.Contains(new Vector2Int(x, y));


    public List<Vector2Int> GetCellNeighbours(int x, int y)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        if (CellIsTaken(x, y - 1))
        {
            cells.Add(new Vector2Int(x, y - 1));
        }

        if (CellIsTaken(x - 1, y))
        {
            cells.Add(new Vector2Int(x - 1, y));
        }

        if (CellIsTaken(x + 1, y))
        {
            cells.Add(new Vector2Int(x + 1, y));
        }

        if (CellIsTaken(x, y + 1))
        {
            cells.Add(new Vector2Int(x, y + 1));
        }

        return cells;
    }
    public int GetCellNeighbourValue(int x, int y)
    {
        int value = 0;

        if (CellIsTaken(x, y - 1))
        {
            value += 1;
        }

        if (CellIsTaken(x - 1, y))
        {
            value += 2;
        }

        if (CellIsTaken(x + 1, y))
        {
            value += 4;
        }

        if (CellIsTaken(x, y + 1))
        {
            value += 8;
        }

        return value;
    }
}
