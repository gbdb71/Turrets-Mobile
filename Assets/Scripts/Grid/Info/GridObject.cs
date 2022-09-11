using UnityEngine;

public enum CellType
{
    Path,
    Ground,
    Barrier
}

[CreateAssetMenu( fileName = "GridObject", menuName = "TowerDefense/Grid Object")]
public class GridObject : ScriptableObject
{
    [SerializeField, AssetPreview] GameObject _prefab;
    [SerializeField, Range(-180, 180)] private int _rotation = 0;
    [SerializeField, SearchableEnum] private CellType _type = CellType.Path;

    public GameObject Prefab => _prefab;
    public int Rotation => _rotation;
    public CellType Type => _type;
}
