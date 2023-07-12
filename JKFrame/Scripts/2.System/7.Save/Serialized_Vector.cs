using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public struct Serialized_Vector3
{
    public float x, y, z;

    public Serialized_Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override string ToString()
    {
        return $"({x},{y},{z})";
    }
}

[Serializable]
public struct Serialized_Vector2
{
    public float x, y;

    public Serialized_Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public override string ToString()
    {
        return $"({x},{y})";
    }
}


public static class SerializedVectorExtensions
{

    public static Vector3 ConverToVector3(this Serialized_Vector3 sv3)
    {
        return new Vector3(sv3.x, sv3.y, sv3.z);
    }

    public static Serialized_Vector3 ConverToSVector3(this Vector3 v3)
    {
        return new Serialized_Vector3(v3.x, v3.y, v3.z);
    }


    public static Vector3Int ConverToVector3Int(this Serialized_Vector3 sv3)
    {
        return new Vector3Int((int)sv3.x, (int)sv3.y, (int)sv3.z);
    }

    public static Serialized_Vector3 ConverToSVector3Int(this Vector3Int v3)
    {
        return new Serialized_Vector3(v3.x, v3.y, v3.z);
    }


    public static Vector2 ConverToSVector2(this Serialized_Vector2 sv2)
    {
        return new Vector2(sv2.x, sv2.y);
    }

    public static Vector2Int ConverToVector2Int(this Serialized_Vector2 sv2)
    {
        return new Vector2Int((int)sv2.x, (int)sv2.y);
    }

    public static Serialized_Vector2 ConverToSVector2(this Vector2 v2)
    {
        return new Serialized_Vector2(v2.x, v2.y);
    }

    public static Serialized_Vector2 ConverToSVector2(this Vector2Int v2)
    {
        return new Serialized_Vector2(v2.x, v2.y);
    }


}