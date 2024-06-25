using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{

	public enum DodgeState {
		NO_DODGE, DODGE, COOLDOWN
	}

	public enum MeleeState {
		NO_MELEE, MELEE, COOLDOWN
	}

	Vector2 inputDirection;

	//Exported Attributes
	[Export]
	public float Speed { get; set; } = 300f;
	[Export]
	public float DodgeSpeed { get; set; } = 600f;
	[Export]
	public float DodgeTime { get; set; } = 10f;
	[Export]
	public float DodgeCooldown { get; set; } = 5f;
	[Export]
	public float MeleeDamage { get; set; } = 10f;

	//Referenced Attributes
	Area2D MeleeTrigger;
	Sprite2D sprite;

	AnimationTree animTree;
	//Dodge
	public float dodgeTimer;
	public float dodgeCooldownTimer;
	public Boolean isDodging;
	public Vector2 dodgeDirection;
	public DodgeState dodgeState;

	//Melee Attack
	public MeleeState meleeState;

    public override void _Ready()
    {
        base._Ready();
		MeleeTrigger = GetNode<Area2D>("MeleeTrigger");
		sprite = GetNode<Sprite2D>("Sprite");
		animTree = GetNode<AnimationTree>("PlayerTree");
		dodgeTimer = DodgeTime;
		dodgeCooldownTimer = DodgeCooldown;
		dodgeState = DodgeState.NO_DODGE;
		meleeState = MeleeState.NO_MELEE;

		inputDirection = Vector2.Zero;
		animTree.Active = true;
    }


    


	public void GetInput() {
		inputDirection = Input.GetVector("Left", "Right", "Up", "Down");
		CheckDodge(inputDirection);
		CheckAttack(inputDirection);
		if(dodgeState != DodgeState.DODGE) Velocity = inputDirection * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		if(Velocity != Vector2.Zero)
		animTree.Set("parameters/Idle/blend_position", Velocity);

		MoveAndSlide();
	}

    public override void _Process(double delta)
    {
        UpdateAnimation();
    }

    public void CheckDodge(Vector2 inputDir) {
		if(dodgeTimer <= 0f) {
			dodgeState = DodgeState.COOLDOWN;
			dodgeTimer = DodgeTime;
		}
		else if(dodgeState == DodgeState.DODGE) {
			dodgeTimer--;
		}
		else if(dodgeCooldownTimer <= 0f) {
			dodgeState = DodgeState.NO_DODGE;
			dodgeCooldownTimer = DodgeCooldown;
		}
		else if(dodgeState == DodgeState.COOLDOWN) {
			dodgeCooldownTimer--;
		}
		else if(Input.IsActionJustPressed("Dodge")) {
			dodgeState = DodgeState.DODGE;
			dodgeDirection = inputDir;
			Velocity = dodgeDirection * DodgeSpeed;
		}
	}

	public void CheckAttack(Vector2 inputDirection) {
		if(meleeState == MeleeState.NO_MELEE && Input.IsActionJustPressed("Weapon"))
		GetNode<AnimatedSprite2D>("Sprite").Play("melee attack");
	}

	public void UpdateAnimation() {
		if(Velocity == Vector2.Zero) {
			animTree.Set("parameters/conditions/idle", true);
			animTree.Set("parameters/conditions/isMoving", false);
		} else {
			animTree.Set("parameters/Move/blend_position", inputDirection);
			animTree.Set("parameters/conditions/idle", false);
			animTree.Set("parameters/conditions/isMoving", true);
		}
		if(Math.Abs(inputDirection.X) != Math.Abs(inputDirection.Y)) {
			animTree.Set("parameters/Move/blend_position", inputDirection);
			animTree.Set("parameters/idle/blend_position", inputDirection);
		}
	}


}