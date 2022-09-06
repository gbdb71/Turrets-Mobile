using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Offset
{
    public int Top;
    public int Bottom;

    public Offset(int top, int bottom)
    {
        Top = top;
        Bottom = bottom;
    }
}

public class PathGenerator : MonoBehaviour
{
    private readonly int _width, _height;
    private readonly Offset _offset;

    private List<Vector2Int> _pathCells;

    public PathGenerator(int width, int height, Offset offset)
    {
        _width = width;
        _height = height;
    }

    public List<Vector2Int> GeneratePath()
    {
        _pathCells = new List<Vector2Int>();

        int x = 0;
        int y = (int)(_height / 2);

        while (x < _width)
        {
            _pathCells.Add(new Vector2Int(x, y));

            bool validMove = false;

            while (!validMove)
            {
                int move = Random.Range(0, 3);

                if (move == 0 || x % 2 == 0 || x > (_width - 2))
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

        return _pathCells;
    }

    private bool CellIsEmpty(int x, int y) => !_pathCells.Contains(new Vector2Int(x, y));
    private bool CellIsTaken(int x, int y) => _pathCells.Contains(new Vector2Int(x, y));

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
