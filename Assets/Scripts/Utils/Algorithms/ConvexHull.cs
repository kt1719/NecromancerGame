using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Algorithms
{
    public static double cross(Vector2 O, Vector2 A, Vector2 B)
    {
        return (A.x - O.x) * (B.y - O.y) - (A.y - O.y) * (B.x - O.x);
    }

    public static Vector2[] GetConvexHull(List<Vector2> points)
    {
        if (points == null)
            return null;

        if (points.Count() <= 3)
            return points.ConvertTo<Vector2[]>();
        int n = points.Count(), k = 0;
        List<Vector2> H = new List<Vector2>(new Vector2[2 * n]);

        points.Sort((a, b) =>
                a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

        // Build lower hull
        for (int i = 0; i < n; ++i)
        {
            while (k >= 2 && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                k--;
            H[k++] = points[i];
        }

        // Build upper hull
        for (int i = n - 2, t = k + 1; i >= 0; i--)
        {
            while (k >= t && cross(H[k - 2], H[k - 1], points[i]) <= 0)
                k--;
            H[k++] = points[i];
        }

        return H.Take(k - 1).ToList().ConvertTo<Vector2[]>();
    }
}
