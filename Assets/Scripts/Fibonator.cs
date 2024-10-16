using UnityEngine;

public class Fibonator
{
    public Vector3[] Points;

    int radius;

    float x1 = 0.1f;
    float x2 = 1.2f;

    public Fibonator(int n, int r = 100, float x1 = 0.1f, float x2 = 1.2f)
    {
        radius = r;
        this.x1 = x1;
        this.x2 = x2;
        Points = GeneratePoints(n);
    }

    Vector3[] GeneratePoints(int n)
    {
        return NX(n, x1 + x2 * n); // X needs change for n bigger than 16000
        //return NX(n, 10 * Mathf.Log(n) + 1);
        //return NX(n, 75391.4f);
        //return NX(n, 75391.4f);
    }

    Vector3 SphericalCoordinate(double x, double y)
    {
        return new Vector3(
            (float)(System.Math.Cos(x) * System.Math.Cos(y)),
            (float)System.Math.Sin(y),
            (float)(System.Math.Sin(x) * System.Math.Cos(y))
        ) * radius;
    }

    public Vector3[] NX(int n, float x)
    {
        Vector3[] pts = new Vector3[n];
        double start = (-1f + 1f / (n - 1f));
        double increment = (2f - 2f / (n - 1f)) / (n - 1f);
        for (int j = 0; j < n; j++)
        {
            double s = start + j * increment;
            pts[j] = SphericalCoordinate(
                s * x,
                System.Math.PI / 2f * System.Math.Sign(s) * (1f - System.Math.Sqrt(1f - System.Math.Abs(s)))
            );
        }
        return pts;
    }
}
