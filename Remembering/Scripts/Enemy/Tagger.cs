using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Tagger : Enemy
{

	[Export]
	public Node2D target;
	[Export]
	public float ChaseSpeed { get; set; } = 30f;
	[Export]
	public EnemyAttackComponent chaseAttack;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Velocity = new Vector2(1, 0).Rotated(new RandomNumberGenerator().Randf()) * Speed;
		SetPhysicsProcess(false);
		CallDeferred("WaitForPhysics");

		attacks = new List<EnemyAttackComponent>
        {
            chaseAttack
        };
	}

	public async void WaitForPhysics() {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		SetPhysicsProcess(true);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		Debug.WriteLine("Is monitorable: " + attacks.Find(a => a.name == "Chase").Monitorable);
	}

	public override void _PhysicsProcess(double delta) {
		if(playerEntered) {
			if(navAgent.IsNavigationFinished() && target.GlobalPosition == navAgent.TargetPosition) return;
			navAgent.TargetPosition = target.GlobalPosition;
			Velocity = GlobalPosition.DirectionTo(navAgent.GetNextPathPosition()) * ChaseSpeed;
		} else {
			Velocity = Velocity.Normalized().Rotated((new RandomNumberGenerator().Randf() - 0.25f) / 10) * Speed;
		}
		base._PhysicsProcess(delta);
	}

	public override void UpdateAnimation() {
		if(playerEntered) {
			animTree.Set("parameters/Chasing/blend_position", Velocity.X);
			animTree.Set("parameters/conditions/isIdle", false);
			animTree.Set("parameters/conditions/isChasing", true);
		} else {
			animTree.Set("parameters/Idle/blend_position", Velocity.X);
			animTree.Set("parameters/conditions/isChasing", false);
			animTree.Set("parameters/conditions/isIdle", true);
		}
	}

		public override void OnDetectPlayer(Area2D player) {
		if(player.Name == "AttackableHitbox") {
			target = (Node2D) player.GetParent();
			playerEntered = true;
			attacks.Find(a => a.name == "Chase").SetDeferred("monitorable", true);
			hitbox.SetDeferred("monitorable", false);
		} 
	}

	public override void OnLeavePlayer(Area2D player) {
		if (player.Name == "AttackableHitbox") {
			target = null;
			playerEntered = false;
			attacks.Find(a => a.name == "Chase").SetDeferred("monitorable", false);
			hitbox.SetDeferred("monitorable", true);
		}	
	}
}
