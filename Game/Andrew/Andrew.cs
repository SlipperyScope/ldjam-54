using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.Andrew;

/// <summary>
/// Andrew's test scene
/// </summary>
public partial class Andrew : Node2D
{
    public override void _Ready()
    {
        this.Global().HelloWorld();
    }
}
