using Godot;
using System;

public partial class EnemyAttackComponent : Area2D
{

	public enum AttackType { Melee, Magic }

	[Export]
	public float damage { get; set; }

	[Export]
	public AttackType attackType { get; set; } = AttackType.Melee;

	[Export]
	public string name;

	[Export]
	public CollisionShape2D hitbox;
}
