using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineComputer))]
public class Road : MonoBehaviour
{
    [HideInInspector] public SplineComputer Spline;

    private static List<Road> _instances = new List<Road>();
    public static IReadOnlyList<Road> Instances => _instances;   
    

    private void Awake()
    {
        Spline = GetComponent<SplineComputer>();
        _instances.Add(this);
    }

    private void OnDestroy()
    {
        _instances.Remove(this);
    }
}
