using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.NotTreesOrAxes.Menus;
public partial class DeceasedPlacard : Control
{
    public void UpdateInfo(DeceasedData data)
    {
        (var name, var method, var count) = data;

        GetNode<Label>("%TreeName").Text = name;
        GetNode<Label>("%Death").Text = method;
        GetNode<Label>("%Count").Text = count.ToString();
        GetNode<Label>("%Times").Text += count is not 1 ? "s" : " ";
    }
}

public record DeceasedData(String Name, String DeathMethod, Int32 Deaths);
