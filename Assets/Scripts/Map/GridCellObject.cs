using UnityEngine;

public enum CellType
{
    Path,
    Ground,
}

[CreateAssetMenu( fileName = "GridCell", menuName = "TowerDefense/Grid Cell")]
public class GridCellObject : ScriptableObject
{
    [SerializeField, AssetPreview] private GameObject _cellPrefab;
    [SerializeField, Range(0, 360)] private int _rotation = 0;
    [SerializeField, SearchableEnum] private CellType _cellType = CellType.Path;

    public GameObject CellPrefab => _cellPrefab;
    public int Rotation => _rotation;
    public CellType CellType => _cellType;
}
