using Godot;
using System;

public partial class EnemyAttackComponent : Area2D
{

	public enum AttackType { Melee, Magic }

	[Export]
	public float damage { get; set; } = 5f;

	[Export]
	public AttackType attackType { get; set; } = AttackType.Melee;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
