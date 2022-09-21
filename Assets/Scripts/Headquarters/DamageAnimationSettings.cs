using UnityEngine;

[System.Serializable]
public class DamageAnimationSettings
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float strenght = 15;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float random = 25;

    public float Duration { get => duration; }
    public float Strenght { get => strenght; }
    public int Vibrato { get => vibrato; }
    public float Random { get => random; }
}
