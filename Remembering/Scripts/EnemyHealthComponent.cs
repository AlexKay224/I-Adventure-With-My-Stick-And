using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class EnemyHealthComponent : Node2D
{

	[Export]
	private float MAX_HEALTH = 10f;
	private float health;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health = MAX_HEALTH;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void TakeDamage(float attackPower) {
		health -= attackPower;
		if(health <= 0) {
			GetParent().QueueFree();
		}
	}
}
