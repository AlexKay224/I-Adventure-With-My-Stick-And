using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class Enemy : CharacterBody2D
{	

	[Export]
	public static float CONTACT_DAMAGE = 5;

	private bool playerEntered;

    public override void _Ready()
    {
		playerEntered = false;
    }

    public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

	public void OnDetectPlayer(Area2D player) {
		if (player.GetParent().HasNode("AttackableHitbox")) {
			playerEntered = true;
		}
	}

	public void OnLeavePlayer(Area2D player) {
		if (player.GetParent().HasNode("AttackableHitbox")) {
			playerEntered = true;
		}	
	}

}
