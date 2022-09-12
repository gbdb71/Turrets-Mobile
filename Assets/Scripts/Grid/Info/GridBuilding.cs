using UnityEngine;

[CreateAssetMenu(fileName = "GridBuilding", menuName = "TowerDefense/Grid Building")]
public class GridBuilding : GridObject
{
    [SerializeField, Min(1)] private Vector2Int _size;
    [SerializeField] private float _offset;

    public float YOffset => _offset;
    public Vector2Int Size => _size;


    public Vector2Int GetRotationOffset()
    {
        switch (this.Dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, Size.x);
            case Dir.BackLeft: return new Vector2Int(0, Size.x);
            case Dir.Up: return new Vector2Int(Size.x, Size.y);
            case Dir.Right: return new Vector2Int(Size.y, 0);
        }
    }
}

