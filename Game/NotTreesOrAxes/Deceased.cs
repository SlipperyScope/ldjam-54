using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.NotTreesOrAxes;
public partial class Deceased : Control
{
    private const String PlackardPath = "res://NotTreesOrAxes/DeceasedPlackard.tscn";
    private PackedScene PlackardScene;

    private VBoxContainer Plackards;
    private ScrollContainer Scroller;
    private AudioStreamPlayer Clicker;

    private List<DeceasedData> DeceasedData;
    private Int32 CurrentIndex;

    [Export]
    private Double AddInterval = 1d;

    public override void _EnterTree()
    {
        PlackardScene = ResourceLoader.Load<PackedScene>(PlackardPath);
        Plackards = GetNode<VBoxContainer>("%Plackards");
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
        AddPlackard(CurrentIndex++);
        if (CurrentIndex >= DeceasedData.Count)
        {
            CurrentIndex = 0;
            this.Global().Beat -= OnBeat;
        }
    }

    private void AddPlackard(Int32 index)
    {
        var plackard = PlackardScene.Instantiate<DeceasedPlackard>();
        Plackards.AddChild(plackard);
        Plackards.MoveChild(plackard, 0);
        plackard.UpdateInfo(DeceasedData[index]);
        Clicker.Play();
    }
}
