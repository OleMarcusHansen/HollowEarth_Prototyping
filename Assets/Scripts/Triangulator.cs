using UnityEngine;
using DelaunatorSharp;

public class Triangulator
{
    public static int[] DelaunayTriangles(Vector3[] points, float radius, bool hollow)
    {
        IPoint[] projectedPoints = ProjectToFlat(points, radius, hollow);

        Delaunator delaunator = new Delaunator(projectedPoints);

        return delaunator.Triangles;
    }

    static IPoint[] ProjectToFlat(Vector3[] points, float radius, bool hollow)
    {
        IPoint[] projectedPoints = new IPoint[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i];
            float newX;
            float newY;
            if (hollow)
            {
                newX = point.x / (radius - point.y);
                newY = point.z / (radius - point.y);
            }
            else
            {
                newX = point.x / (radius + point.y);
                newY = point.z / (radius + point.y);
            }
            projectedPoints[i] = new Point(newX, newY);
        }
        return projectedPoints;
    }
}
