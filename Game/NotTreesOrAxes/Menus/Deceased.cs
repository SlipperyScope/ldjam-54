using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.NotTreesOrAxes.Menus;

public partial class Deceased : Control
{
    [Export]
    private PackedScene PlacardScene;

    [Export]
    private PackedScene ReturnPlacardScene;

    private VBoxContainer Placards;
    private ScrollContainer Scroller;
    private AudioStreamPlayer Clicker;

    private List<DeceasedData> DeceasedData;
    private Int32 CurrentIndex;

    [Export]
    private Double AddInterval = 1d;

    public override void _EnterTree()
    {
        Placards = GetNode<VBoxContainer>("%Plackards");
        Scroller = GetNode<ScrollContainer>("%Scroller");
        Clicker = GetNode<AudioStreamPlayer>("%Clicker");
    }

    public void Populate(List<DeceasedData> data)
    {
        DeceasedData = data;
        CurrentIndex = 0;
        //Next = Time;
        this.Global().Beat += OnBeat;
    }

    private void OnBeat(Object sender, EventArgs e)
    {
        AddPlacard(CurrentIndex++);
        if (CurrentIndex >= DeceasedData.Count)
        {
            AddReturnPlacard();
            CurrentIndex = 0;
            this.Global().Beat -= OnBeat;
        }
    }

    private void AddPlacard(Int32 index)
    {
        var plackard = PlacardScene.Instantiate<DeceasedPlacard>();
        Placards.AddChild(plackard);
        Placards.MoveChild(plackard, 0);
        plackard.UpdateInfo(DeceasedData[index]);
        Clicker.Play();
    }

    private void AddReturnPlacard()
    {
        var plackard = ReturnPlacardScene.Instantiate<Control>();
        Placards.AddChild(plackard);
        Placards.MoveChild(plackard, 0);
        Clicker.Play();
    }
}
