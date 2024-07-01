using Godot;
using System;
using System.Diagnostics;

public partial class Player : CharacterBody2D
{

	public enum DodgeState {
		NO_DODGE, DODGE, COOLDOWN
	}

	public enum MeleeState {
		NO_MELEE, MELEE, COOLDOWN
	}

	Vector2 inputDirection;

	//Exported Attributes
	[Export]
	public float Speed { get; set; } = 300f;
	[Export]
	public float DodgeSpeed { get; set; } = 600f;
	[Export]
	public float DodgeTime { get; set; } = 10f;
	[Export]
	public float DodgeCooldown { get; set; } = 5f;
	[Export]
	public float MeleeDamage { get; set; } = 10f;
	[Export]
	public float Health { get; set; } = 50f;
	[Export]
	public float Shield { get; set; } = 0f;
	[Export]
	public float Defence { get; set; } = 0f;
	[Export]
	public float Resistance { get; set; } = 0f;

	[Export]
	public float InvincibilityFrames { get; set; } = 0.5f;
	[Export]
	public float knockbackPower { get; set; } = 100f;

	protected Timer InvincibilityTimer;
	protected Timer KnockbackTimer;
	private bool isInvincible;
	private bool knockbackActive;


	private bool isAlive;

	//Referenced Attributes
	Sprite2D sprite;

	AnimationTree animTree;
	//Dodge
	public float dodgeTimer;
	public float dodgeCooldownTimer;
	public Boolean isDodging;
	public Vector2 dodgeDirection;
	public DodgeState dodgeState;

	//Melee Attack
	public MeleeState meleeState;

    public override void _Ready()
    {
        base._Ready();
		sprite = GetNode<Sprite2D>("Sprite");
		animTree = GetNode<AnimationTree>("PlayerTree");
		dodgeTimer = DodgeTime;
		dodgeCooldownTimer = DodgeCooldown;
		dodgeState = DodgeState.NO_DODGE;
		meleeState = MeleeState.NO_MELEE;

		InvincibilityTimer = GetNode<Timer>("InvincibilityTimer");
		KnockbackTimer = GetNode<Timer>("KnockbackTimer");
		InvincibilityTimer.WaitTime = InvincibilityFrames;

		inputDirection = Vector2.Zero;
		animTree.Active = true;
		isAlive = true;
		isInvincible = false;
    }


    


	public void GetInput() {
		inputDirection = Input.GetVector("Left", "Right", "Up", "Down");
		CheckDodge(inputDirection);
		CheckAttack(inputDirection);
		if(dodgeState != DodgeState.DODGE) Velocity = inputDirection * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		if(Velocity != Vector2.Zero)
		animTree.Set("parameters/Idle/blend_position", Velocity);
		if(!knockbackActive) MoveAndSlide();
	}

    public override void _Process(double delta)
    {
        UpdateAnimation();
    }

    public void CheckDodge(Vector2 inputDir) {
		if(dodgeTimer <= 0f) {
			dodgeState = DodgeState.COOLDOWN;
			dodgeTimer = DodgeTime;
		}
		else if(dodgeState == DodgeState.DODGE) {
			dodgeTimer--;
		}
		else if(dodgeCooldownTimer <= 0f) {
			dodgeState = DodgeState.NO_DODGE;
			dodgeCooldownTimer = DodgeCooldown;
		}
		else if(dodgeState == DodgeState.COOLDOWN) {
			dodgeCooldownTimer--;
		}
		else if(Input.IsActionJustPressed("Dodge")) {
			dodgeState = DodgeState.DODGE;
			dodgeDirection = inputDir;
			Velocity = dodgeDirection * DodgeSpeed;
		}
	}

	public void CheckAttack(Vector2 inputDirection) {
		if(meleeState == MeleeState.NO_MELEE && Input.IsActionJustPressed("Weapon"))
		return;

		//GetNode<AnimatedSprite2D>("Sprite").Play("melee attack");
	}

	//used for dealing damage to the player
	public void OnEnemyOrAttackAttackedPlayer(Area2D hit) {
		if(!isInvincible) {
			if(hit is EnemyAttackComponent) {
				Debug.WriteLine("Attacking Enemy");
				EnemyAttackComponent attack = (EnemyAttackComponent) hit;
					TakeDamage(attack.damage, attack.attackType);
			} else if(hit is EnemyHitboxComponent) {
				Debug.WriteLine("Enemy");
					TakeContactDamage(Enemy.CONTACT_DAMAGE);
				}
			Knockback();
		} else {
			Debug.WriteLine("honh");
		}
	}

	public void UpdateAnimation() {
		if(Velocity == Vector2.Zero) {
			animTree.Set("parameters/conditions/idle", true);
			animTree.Set("parameters/conditions/isMoving", false);
		} else {
			animTree.Set("parameters/Move/blend_position", inputDirection);
			animTree.Set("parameters/conditions/idle", false);
			animTree.Set("parameters/conditions/isMoving", true);
		}
		if(Math.Abs(inputDirection.X) != Math.Abs(inputDirection.Y)) {
			animTree.Set("parameters/Move/blend_position", inputDirection);
			animTree.Set("parameters/idle/blend_position", inputDirection);
		}
	}

	//function for dealing damage to the player
	public void TakeDamage(float damage, EnemyAttackComponent.AttackType type) {
		float damageDealt = 0f;
		switch(type) {
			case EnemyAttackComponent.AttackType.Melee:
				damageDealt = damage - Defence;
				break;
			case EnemyAttackComponent.AttackType.Magic:
				damageDealt = damage - Resistance;
				break;
		}
		if(damageDealt <= 1) damageDealt = 1f;
		if(Shield <= 0f) {
			Health -= damageDealt;
		}
		else if(Shield < damageDealt) {
			float temp = Shield;
			Shield = 0f;
			damageDealt -= temp;
			Health -= damageDealt;
		}
		else {
			Shield -= damageDealt;
		}

		if(Health <= 0f) isAlive = false;
	}

	public void OnInvincibleTimeout() {
		isInvincible = false;
	}

	public void OnKnockbackTimeout() {
		knockbackActive = false;
	}

	
    private void TakeContactDamage(float contactDamage)
    {
		Debug.WriteLine("OW");
		if(Shield <= 0f) {
			Health -= contactDamage;
		}
		else if(Shield < contactDamage) {
			float temp = Shield;
			Shield = 0f;
			contactDamage -= temp;
			Health -= contactDamage;
		}
		else {
			Shield -= contactDamage;
		}

		if(Health <= 0f) isAlive = false;
    }

	private void Knockback() {
		if(!knockbackActive) {
			knockbackActive = true;
			KnockbackTimer.Start();
			Vector2 knockback = -Velocity;
			Velocity = knockback.Normalized() * knockbackPower;
			MoveAndSlide();
		}
	}

	private void Invincible() {
		if(!isInvincible) {
			isInvincible = true;
			InvincibilityTimer.Start();
		}
	}

}