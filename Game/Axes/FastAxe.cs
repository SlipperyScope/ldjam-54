using System;
using Godot;

namespace Game.Axes;
public partial class FastAxe : Node2D, IAxe
{
    public Int32 GetDamage() { return 1; }

    public float getSwingTime() { return .25f; }

    public float getRange() { return 85; }

    public string getName() { return "Fast Axe"; }
}