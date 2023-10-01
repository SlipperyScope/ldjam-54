using System;
using Godot;

namespace Game.Axes;
public partial class DefaultAxe : Node2D, IAxe
{
    public Int32 GetDamage() { return 1; }

    public float getSwingTime() { return 1f; }

    public float getRange() { return 85; }
    public string getName() { return "Default Axe"; }
}