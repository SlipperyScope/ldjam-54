using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.Math;

/// <summary>
/// Represents the space between two concentric circles
/// </summary>
public struct Annulus
{
    /// <summary>
    /// Outer radius (inclusive)
    /// </summary>
    public Single R { get; set; }

    /// <summary>
    /// Inner radius (inclusive)
    /// </summary>
    public Single r { get; set; }

    public Annulus(Single r, Single R)
    {
        this.R = R;
        this.r = r;

        if (r > R)
        {
            GD.PushWarning("You have created an annulus with negative area. Is this intentional?");
        }

        if (r == R)
        {
            GD.PushWarning("You have created an annulus with zero area. Is this intentional?");
        }
    }

    /// <summary>
    /// Determines if a point is within the annulus, where the point 0,0 is the center of the annulus
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <returns>True if it is contained in the annulus</returns>
    public readonly Boolean Contains(Vector2 point)
    {
        var distanceSquared = Vector2.Zero.DistanceSquaredTo(point);
        return distanceSquared <= (R * R) && distanceSquared >= (r * r);
    }

    public override readonly String ToString() => $"{{ r = {r}, R = {R} }}";
}