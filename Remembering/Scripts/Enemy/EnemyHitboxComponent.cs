using Godot;
using System;
using System.Diagnostics;

public partial class EnemyHitboxComponent : Area2D
{
	[Export]
	EnemyHealthComponent healthComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void Damage(float attackPower) {
		Debug.WriteLine("ow");
		Debug.WriteLine("Current Health: " + healthComponent.GetHealth());
		if(healthComponent != null) {
			healthComponent.TakeDamage(attackPower);
		}
	}

	
}
