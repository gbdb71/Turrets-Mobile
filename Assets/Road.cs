using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class Road : MonoBehaviour
{
    [SerializeField] private RoadScenario _scenario;
    [HideInInspector] public SplineComputer Spline;

    private RoadScenario.State _scenarioState;
    private static List<Road> _instances = new List<Road>();

    public static IReadOnlyList<Road> Instances => _instances;   
    public RoadScenario.State ScenarioState => _scenarioState;
    

    private void Awake()
    {
        Spline = GetComponent<SplineComputer>();
        _instances.Add(this);
    }

    private void Start()
    {
        _scenarioState = _scenario.Begin(this);
    }

    private void OnDestroy()
    {
        _instances.Remove(this);
    }
}
