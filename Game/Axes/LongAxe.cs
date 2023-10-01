using System;
using Godot;

namespace Game.Axes;
public partial class LongAxe : Node2D, IAxe
{
    public Int32 GetDamage() { return 1; }

    public float getSwingTime() { return 1f; }

    public float getRange() { return 200; }

    public string getName() { return "Long Axe"; }
}