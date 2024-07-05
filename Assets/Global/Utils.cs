﻿using UnityEngine;
using System;

public static class Utils
{
    public static Vector3 Multiply(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    public static Vector3 Divide(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
    public static Vector3 Abs(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
    public static Vector3 NullZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    public static Vector3 SetX(this Vector3 vector, float x) => new Vector3(x, vector.y, vector.z);
    public static Vector3 SetY(this Vector3 vector, float y) => new Vector3(vector.x, y, vector.z);
    public static Vector3 SetZ(this Vector3 vector, float z) => new Vector3(vector.x, vector.y, z);

    public static Vector2 Rotate(this Vector2 vector, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = vector.x;
        float ty = vector.y;
        vector.x = (cos * tx) - (sin * ty);
        vector.y = (sin * tx) + (cos * ty);
        return vector;
    }

    public static float SqrDistance2D(this Vector2 a, Vector2 b)
    {
        return (a - b).sqrMagnitude;
    }
    public static float SqrDistance2D(this Vector3 a, Vector3 b)
    {
        return (a - b).SqrMagnitudeXY();
    }
    public static float SqrMagnitudeXY(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y).sqrMagnitude;
    }

    public static Vector2 ConvertTo2D(this Vector3 vector3) => new Vector2(vector3.x, vector3.y);
    public static Vector3 ConvertTo3D(this Vector2 vector2) => new Vector3(vector2.x, vector2.y);


    public static Vector2 Multiply(this Vector2 a, Vector2 b)
    {
        var c = a* b;
        return new Vector2(a.x * b.x, a.y * b.y);
    }
    public static Vector2 Divide(this Vector2 a, Vector2 b)
    {
        return new Vector2(a.x / b.x, a.y / b.y);
    }
    public static Vector2 Abs(this Vector2 vector)
    {
        return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }


    public static Vector2Int Multiply(this Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x * b.x, a.y * b.y);
    }
    public static Vector2Int Divide(this Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x / b.x, a.y / b.y);
    }
    public static Vector2Int Abs(this Vector2Int vector)
    {
        return new Vector2Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
}