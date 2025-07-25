using Godot;
using System;

public static class Vector2Extensions
{
    public static Vector2 CartesianToIsometric(this Vector2 vector) => new(vector.X - vector.Y, (vector.X + vector.Y) / 2f);

    public static float GetIsometricAngleTo(this Vector2 from, Vector2 to)
    {
        Vector2 dir = to - from;

        Vector2 isoVector = CartesianToIsometric(dir);

        float angle = Mathf.RadToDeg(isoVector.Angle());
        if (angle < 0)
            angle += 360;

        return angle;
    }
}
