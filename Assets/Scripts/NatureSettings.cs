using UnityEngine;

[System.Serializable]
public class NatureSettings
{
    [Range(1, 10)]
    public float noiseScale = 1f;
    public Vector3 centre;
    [Range(0, 1)]
    public float minValue = 0.5f;
    [Range(0, 90)]
    public int minLatitude = 0;
    [Range(0, 90)]
    public int maxLatitude = 10;
    [Range(0, 90)]
    public int optLatitude = 5;
    [Range(-100, 1050)]
    public int minAltitude = 10;
    [Range(-100, 1050)]
    public int maxAltitude = 10;
    [Range(-100, 1050)]
    public int optAltitude = 10;
    [Range(-1, 1)]
    public float density = 1;
}
