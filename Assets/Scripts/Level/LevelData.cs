using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "TowerDefense/Level/Data")]
public class LevelData : ScriptableObject
{
    private readonly int _minLength = 10;
    private int _maxLength { get { return (_gridWidth * _gridHeight) / 5; } }

    [Label("Grid Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(1, 40), Suffix("cells")] private int _gridWidth;
    [SerializeField, Range(1, 30), Suffix("cells")] private int _gridHeight;

    [Label("Path Settings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, DynamicMinMaxSlider(nameof(_minLength), nameof(_maxLength)), Tooltip("Required size: (width * height) / 5")] private Vector2 _pathLength;
    [SerializeField] private Offset _offset;

    [Label("Buildings", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [NotNull]
    [SerializeField] private GridBuilding _basePrefab;
    [SerializeField, ReorderableList(Foldable = true)] private List<GridBuilding> _buildingObjects;

    [Label("Barriers", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, Range(0, 20)] private int _barrierRows = 4;
    [SerializeField, DisableIf(nameof(_barrierRows), 1, Comparison = UnityComparisonMethod.Less)] private GridObject _barrierPrefab;

    [Label("Barriers", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField, NotNull] private LevelScenario _levelScenario = default;

    [Label("Currencies", skinStyle: SkinStyle.Box, Alignment = TextAnchor.MiddleCenter)]
    [SerializeField] private int _constructionCurrency;

    public int ConstructionCurrency => _constructionCurrency;
    public int GridWidth { get => _gridWidth; }
    public int GridHeight { get => _gridHeight; }
    public Vector2Int PathLength { get => Vector2Int.RoundToInt(_pathLength); }
    public Offset Offset { get => _offset; }
    public GridBuilding HeadquartersPrefab { get => _basePrefab; }
    public List<GridBuilding> BuildingObjects { get => _buildingObjects; }
    public int BarrierRows { get => _barrierRows; }
    public GridObject BarrierPrefab { get => _barrierPrefab; }
    public LevelScenario LevelScenario { get => _levelScenario; }
    public RewardSettings RewardSettings { get => _rewardSettings; }
}
