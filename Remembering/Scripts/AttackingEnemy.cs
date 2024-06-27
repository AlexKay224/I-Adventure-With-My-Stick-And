using Godot;
using System;
using System.Collections.Generic;

public partial class AttackingEnemy : Enemy
{

	[Export]
	public EnemyAttackComponent attack { get; set; }

	protected Timer attackTimer;
	
	protected bool canAttack;

	[Export]
	public float attackCooldown { get; set; } = 5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		canAttack = true;
		attackTimer.WaitTime = attackCooldown;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void Attack(Vector2 playerLoc) {
		return;
	}
}
