using Godot;
using System;
using System.Diagnostics;

public partial class WeaponHitbox : Area2D
{

	[Export]
	public Player playerRef;

	protected float DamageMult;
	protected float WeaponDamage;
	protected float totalDamage;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DamageMult = playerRef.MeleeDamage;
		WeaponDamage = playerRef.currentWeapon.damage;
		totalDamage = WeaponDamage * DamageMult;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void OnEnemyHit(Area2D enemy) {
		if (enemy is EnemyHitboxComponent) {
			Debug.WriteLine("Hello");
			EnemyHitboxComponent e = (EnemyHitboxComponent) enemy;
			e.Damage(totalDamage);
		} else Debug.WriteLine("Miss: " + enemy.GetPath());
	}

}
