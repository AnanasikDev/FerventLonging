using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static List<T> Shuffle<T>(this List<T> list) => list.OrderBy(_ => Random.value).ToList();
    public static T[] Shuffle<T>(this T[] array) => array.OrderBy(_ => Random.value).ToArray();

    public static Vector2 GenerateRandomPositionWithinBounds(this Bounds bounds)
    {
        return new Vector2(Random.value * bounds.size.x, Random.value * bounds.size.y) + bounds.min.ConvertTo2D();
    }

    public static Vector2[] GenerateRandomPositionsWithinBounds(this Bounds bounds, float areaFactor)
    {
        int number = (int)(bounds.size.x * bounds.size.y * areaFactor);

        Vector2[] result = new Vector2[number];
        for (int i = 0; i < number; i++)
        {
            result[i] = new Vector2(Random.value * bounds.size.x, Random.value * bounds.size.y) + bounds.min.ConvertTo2D();
        }

        return result;
    }
}