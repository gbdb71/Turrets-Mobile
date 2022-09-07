using UnityEngine;

[CreateAssetMenu(fileName = "GridBuilding", menuName = "TowerDefense/Grid Building")]
public class BuildingObject : GridCellObject
{
    [SerializeField, Min(1)] private Vector2Int _size;
    [SerializeField] private float _offset;

    public float Offset => _offset;
    public Vector2Int Size => _size;
}

