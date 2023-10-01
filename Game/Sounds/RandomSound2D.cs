using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Game.Sounds;

/// <summary>
/// Can play a random sound at a randomized pitch
/// </summary>
[GlobalClass]
public partial class RandomSound2D : Node2D
{
    /// <summary>
    /// Sounds from which to pick a random one
    /// </summary>
    [Export]
    private Godot.Collections.Array<AudioStream> Sounds = new();

    /// <summary>
    /// Amount of maximum pitch variance
    /// </summary>
    [Export]
    private Single PitchVariation = 0.05f;

    [Export]
    private Boolean Play2D = true;

    [Export]
    private Single MaxDistance = 4000f;

    [Export(PropertyHint.ExpEasing, "attenuation")]
    private Single Attenuation = 0.5f;

    [Export(PropertyHint.Range, "-5,5,0.1,or_greater,or_less")]
    private Single Volume_dB = 1;

    private AudioStreamPlayer2D Player2D;

    private AudioStreamPlayer PlayerOmni;

    public override void _EnterTree()
    {
        if (Play2D is true)
        {
            Player2D = new AudioStreamPlayer2D();
            AddChild(Player2D);
            Player2D.MaxDistance = MaxDistance;
            Player2D.Attenuation = Attenuation;
            Player2D.VolumeDb = Volume_dB;
            Player2D.MaxPolyphony = 8;
        }
        else
        {
            PlayerOmni = new AudioStreamPlayer();
            AddChild(PlayerOmni);
            PlayerOmni.VolumeDb = Volume_dB;
            PlayerOmni.MaxPolyphony = 8;
        }
    }

    public override void _Ready()
    {
        //AddChild(Play2D is true ? Player2D : PlayerOmni);
    }

    public void Play()
    {
        if (Play2D is true)
        {
            Player2D.PitchScale = 1f - PitchVariation + GD.Randf() * PitchVariation;
            Player2D.Stream = Sounds[(Int32)(GD.Randi() % Sounds.Count)];
            Player2D.Play();
        }
        else
        {
            PlayerOmni.PitchScale = 1f - PitchVariation + GD.Randf() * PitchVariation;
            PlayerOmni.Stream = Sounds[(Int32)(GD.Randi() % Sounds.Count)];
            PlayerOmni.Play();
        }
    }
}
