using UnityEngine;
using System;
/// <summary>
/// 可序列化的颜色
/// </summary>
[Serializable]
public struct Serialized_Color
{
    public float r, g, b, a;
    public Serialized_Color(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public override string ToString()
    {
        return $"({r},{g},{b},{a})";
    }
    public override int GetHashCode()
    {
        return this.ConverToUnityColor().GetHashCode();
    }
    public static implicit operator Serialized_Color(Color color)
    {
        return new Serialized_Color(color.r, color.g, color.b, color.a);
    }
    public static implicit operator Color(Serialized_Color color)
    {
        return new Color(color.r, color.g, color.b, color.a);
    }
}

public static class Serialization_ColorExtensions
{
    public static Color ConverToUnityColor(this Serialized_Color color)
    {
        return new Color(color.r, color.g, color.b, color.a);
    }

    public static Serialized_Color ConverToSerializationColor(this Color color)
    {
        return new Serialized_Color(color.r, color.g, color.b, color.a);
    }

}
