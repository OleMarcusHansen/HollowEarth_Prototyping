using UnityEngine;

[System.Serializable]
public class ShapeSettings
{
    [Range(1, 1000)]
    public int radius = 100;
    public int numberOfPoints = 100;
    public float x1 = 0.1f;
    public float x2 = 1.2f;
}
