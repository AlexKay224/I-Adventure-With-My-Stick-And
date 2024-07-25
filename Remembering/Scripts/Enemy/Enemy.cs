using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;

[Icon("res://Remembering/Art/Misc/Justin's Icons/32x32/skull.png")]
public partial class Enemy : CharacterBody2D
{	

	[Export]
	public static float CONTACT_DAMAGE = 5;
	[Export]
	public float Speed { get; set; }
	[Export]
	public AnimationTree animTree;
	[Export]
	public NavigationAgent2D navAgent;
	[Export]
	public EnemyHitboxComponent hitbox;
	
	protected bool playerEntered;

	public List<EnemyAttackComponent> attacks; 

    public override void _Ready()
    {
		playerEntered = false;
    }

	public override void _Process(double delta) {
		UpdateAnimation();
	}

    public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public virtual void OnDetectPlayer(Area2D player) {
		if (player is AttackableHitbox) {
			playerEntered = true;
		}
	}

	public virtual void OnLeavePlayer(Area2D player) {
		if (player is AttackableHitbox) {
			playerEntered = false;
		}	
	}



	public virtual void UpdateAnimation() {
		return;
	}

}
