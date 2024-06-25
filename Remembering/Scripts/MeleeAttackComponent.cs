using Godot;
using System;

public partial class MeleeAttackComponent : Area2D
{
	[Export]
	private float baseDamage = 10f;
	[Export]
	private float baseRate = 1f;
	[Export]
	private AnimationPlayer animPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
