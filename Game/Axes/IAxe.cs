using System;
using Godot;

namespace Game.Axes;

/// <summary>
/// Defines an Ax(e)
/// </summary>
public interface IAxe 
{
    public abstract Int32 GetDamage();
    public abstract float getSwingTime();
    public abstract float getRange();
    public abstract string getName();
    // public event EventHandler<Node2D> Felled;
    // public Boolean DoAHit(Int32 damage);
    // public Seed Spread();
    // public Node2D AsNode { get; }
}