using Godot;
using System;

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
		if(healthComponent != null) {
			healthComponent.TakeDamage(attackPower);
		}
	}
}
