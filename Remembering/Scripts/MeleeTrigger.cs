using Godot;
using System;

public partial class MeleeTrigger : Area2D
{

	public float damage;

	Player parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = (Player) GetNode<CharacterBody2D>("Player");
		damage = parent.MeleeDamage;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
