using UnityEngine;

public enum CellType
{
    Path,
    Ground,
    Barrier
}
public enum Dir : int
{
    BackLeft = -90,
    Down = 0,
    Left = 90,
    Up = 180,
    Right = 270,
}

[CreateAssetMenu( fileName = "GridObject", menuName = "TowerDefense/Grid Object")]
public class GridObject : ScriptableObject
{
    [SerializeField, AssetPreview] GameObject _prefab;
    [SerializeField] private Dir _dir;
    [SerializeField, SearchableEnum] private CellType _type = CellType.Path;

    public GameObject Prefab => _prefab;
    public Dir Dir => _dir;
    public CellType Type => _type;
}
