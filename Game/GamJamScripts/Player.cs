using Game.Andrew;
using Game.Axes;
using Game.Sounds;
using Game.Trees;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game;

public partial class Player : CharacterBody2D
{
    private Sprite2D axeSprite;
    private NavigationAgent2D nav;
    private AudioStreamPlayer2D chopSfx;
    private RandomSound2D Steppies;
    int speed = 300;
    float accel = 7.0f;

    public bool chopping = false;
    public bool fishing = false;

    private Double MaxStepInterval = 1d;
    private Double MinStepInterval = .25d;
    private Double NextStep = Double.MaxValue;
    private Double PrevStep = 0f;
    private Double Time = 0f;

    private IAxe Axe;
    
    //Just for testing
    private List<IAxe> TestAxes = new List<IAxe> { new DefaultAxe(), new StrongAxe(), new FastAxe(), new LongAxe() };
    private int testAxesIndex = 0;
    //

    public CanvasItem targetTree;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Global().SetPlayer(this);
        nav = GetNode<NavigationAgent2D>("PlayerNav");
        nav.VelocityComputed += Move;
        chopSfx = GetNode<AudioStreamPlayer2D>("ChopSfx");
        Steppies = GetNodeOrNull<RandomSound2D>($"%{nameof(Steppies)}") ?? throw new NullReferenceException($"Could not find {nameof(RandomSound2D)} node named {nameof(Steppies)}");

        Axe = new DefaultAxe();
        axeSprite = GetNode<Sprite2D>("Axe");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override async void _Process(double delta)
    {
        Time += delta;

        #region Steps
        if (Time >= NextStep)
        {
            Steppies.Play();
            PrevStep = Time;
        }

        var currentSpeed = Velocity.Length();

        if (currentSpeed > 0.01f)
        {
            NextStep = (1d - 1d / speed * Velocity.Length()) * (MaxStepInterval - MinStepInterval) + MinStepInterval + PrevStep;
        }
        else
        {
            NextStep = Double.MaxValue;
        } 
        #endregion

        Vector2 pos = GetGlobalTransformWithCanvas().Origin;
        if (targetTree != null && pos.DistanceTo(targetTree.GetGlobalTransformWithCanvas().Origin) < Axe.getRange()) {
            nav.TargetPosition = pos;
            if (!chopping)
            {
                chopping = true;
                Tween tween = GetTree().CreateTween();
                tween.TweenProperty(axeSprite, "visible", true, 0);
                tween.TweenProperty(axeSprite, "rotation_degrees", 0, Axe.getSwingTime());
                tween.TweenProperty(axeSprite, "visible", false, 0);
                tween.TweenProperty(axeSprite, "rotation_degrees", -90, 0);

                await ToSignal(GetTree().CreateTimer(Axe.getSwingTime()), "timeout");
                chopSfx.PitchScale = (Single)GD.RandRange(0.95d, 1.05d);
                chopSfx.Play();
                GD.Print("chop");
                if ((targetTree as ITree).DoAHit(Axe.GetDamage())) {
                    GD.Print("tree felled!");
                   // targetTree.CallDeferred("queue_free"); //should eventually change to stump and stop spawning
                    targetTree = null;
                }
                chopping = false;
            }
        }

        if (!chopping && pos.DistanceTo(nav.TargetPosition) > 10)
        {
            Vector2 direction = nav.GetNextPathPosition() - pos;
            direction = direction.Normalized();
            Velocity = Velocity.Lerp(direction * speed, (float)(accel * delta));
            nav.Velocity = Velocity;
        }
        else
        {
            nav.Velocity = new Vector2(0, 0);
        }
    }

    public void SetTarget(CanvasItem target) {
        if (!chopping) {
            targetTree = target;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.IsReleased() && !chopping)
            {
                targetTree = null;
                nav.TargetPosition = eventMouseButton.Position;
            }
        }

		if (@event is InputEventKey keyEvent && keyEvent.Pressed) {
			if (keyEvent.Keycode == Key.Space) TestChangeAxe();
		}
    }

    private void Move(Vector2 navVelocity)
    {
        Velocity = navVelocity;
        MoveAndSlide();
        if (Velocity.X != 0 || Velocity.Y != 0)
        {
            Rotation = Velocity.Angle();
        }
    }

    private void TestChangeAxe()
    {
        if (++testAxesIndex >= TestAxes.Count) {
            testAxesIndex = 0;
        }

        Axe = TestAxes.ElementAt(testAxesIndex);
        GD.Print("Switch axe to: " + Axe.getName());
    }
}
