using Godot;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;

[Icon("res://Remembering/Art/Misc/Justin's Icons/32x32/character.png")]
public partial class Player : CharacterBody2D
{

	public enum DodgeState {
		NO_DODGE, DODGE, COOLDOWN
	}

	public enum MeleeState {
		NO_MELEE, MELEE
	}

	Vector2 inputDirection;
	Vector2 lastDirection;

	//Exported Attributes
	[ExportGroup("Attributes")]
	[Export]
	public float Speed { get; set; } = 300f;
	[Export]
	public float DodgeSpeed { get; set; } = 600f;
	[Export]
	public float DodgeTime { get; set; } = 10f;
	[Export]
	public float DodgeCooldown { get; set; } = 5f;
	[Export]
	public float MeleeDamage { get; set; } = 1f;
	[Export]
	public float MeleeRate { get; set; } = 1f;
	[Export]
	public float MaxHealth { get; set; } = 50f;
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

	[ExportGroup("Instances")]
	[Export]
	public ProgressBar healthbar;
	[Export]
	public Weapon currentWeapon;
	[Export]
	public AnimationPlayer weaponAnims;


	//Timers
	protected Timer InvincibilityTimer;
	protected Timer KnockbackTimer;
	protected Timer MeleeTimer;
	private bool isInvincible;
	private bool knockbackActive;


	private bool isAlive;

	//Custom Signals
	[Signal]
	public delegate void HealthChangeEventHandler();

	//Referenced Attributes
	Sprite2D sprite;

	AnimationTree animTree;
	//Dodge
	public float dodgeTimer;
	public float dodgeCooldownTimer;
	public Vector2 dodgeDirection;
	public DodgeState dodgeState;
	public float health;

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
		MeleeTimer = GetNode<Timer>("MeleeTimer");
		InvincibilityTimer.WaitTime = InvincibilityFrames;
		MeleeTimer.WaitTime = MeleeRate; //Note that this will have to be adjusted to multiply with weapon cooldown

		inputDirection = Vector2.Zero;
		lastDirection = Vector2.Down;
		animTree.Active = true;
		isAlive = true;
		isInvincible = false;
		knockbackActive = false;

		health = MaxHealth;
    }


    


	public void GetInput() {
		inputDirection = Input.GetVector("Left", "Right", "Up", "Down");
		if(inputDirection != Vector2.Zero) lastDirection = inputDirection;
		CheckDodge(inputDirection);
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

	public bool CheckAttack() {
		if(meleeState == MeleeState.NO_MELEE && Input.IsActionJustPressed("Weapon")) {
			meleeState = MeleeState.MELEE;
			MeleeTimer.Start();
			return true;
		} else return false;

		//GetNode<AnimatedSprite2D>("Sprite").Play("melee attack");
	}

	//used for dealing damage to the player
	public void OnEnemyOrAttackAttackedPlayer(Area2D hit) {
		if(!isInvincible) {
			if(hit.Name == "EnemyAttackComponent") {
				Debug.WriteLine("AAAA");
				EnemyAttackComponent attack = (EnemyAttackComponent) hit;
				TakeDamage(attack.damage, attack.attackType);
			} else if(hit is EnemyHitboxComponent) {
				Debug.WriteLine("Hello");	
				TakeContactDamage(Enemy.CONTACT_DAMAGE);
			} else Debug.WriteLine ("hit name: " + hit.Name);
			Knockback(hit.GlobalPosition - GlobalPosition);
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
		if(CheckAttack()) {
			AttackAnimation();
		} else {
			animTree.Set("parameters/conditions/Melee", false);
		}
		if(inputDirection != Vector2.Zero) {
			animTree.Set("parameters/Move/blend_position", inputDirection);
			animTree.Set("parameters/idle/blend_position", inputDirection);
			animTree.Set("parameters/Melee_Attack/blend_position", inputDirection);
		}
	}

	public void AttackAnimation() {
		animTree.Set("parameters/conditions/" + currentWeapon.playerState, true);
		if(currentWeapon != null) {
			GetNode<Sprite2D>("Weapon").Set("texture", currentWeapon.texture);
			string wa = currentWeapon.weaponAnimation + "_" + GetWeaponDirection();
			weaponAnims.Play(wa);
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
			health -= damageDealt;
		}
		else if(Shield < damageDealt) {
			float temp = Shield;
			Shield = 0f;
			damageDealt -= temp;
			health -= damageDealt;
		}
		else {
			Shield -= damageDealt;
		}

		EmitSignal(SignalName.HealthChange);
		if(health <= 0f) GameOver();
	}

	public void OnInvincibleTimeout() {
		isInvincible = false;
	}

	public void OnKnockbackTimeout() {
		knockbackActive = false;
	}

	public void OnMeleeTimeout() {
		meleeState = MeleeState.NO_MELEE;
	}

	
    private void TakeContactDamage(float contactDamage)
    {
		if(Shield <= 0f) {
			health -= contactDamage;
		}
		else if(Shield < contactDamage) {
			float temp = Shield;
			Shield = 0f;
			contactDamage -= temp;
			health -= contactDamage;
		}
		else {
			Shield -= contactDamage;
		}
		EmitSignal(SignalName.HealthChange);

		if(health <= 0f) GameOver();
    }

	private void Knockback(Vector2 dir) {
		if(!knockbackActive) {
			knockbackActive = true;
			KnockbackTimer.Start();
			Vector2 knockback = -dir;
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

	public void GameOver() {
		//this is not finished. It is a very scuffed way to end the game
		isAlive = false;
		Visible = false;
		knockbackActive = true;
	}




	//helper method to get weapon animation string from current direction
	public string GetWeaponDirection() {
		if(Math.Abs(lastDirection.X) > Math.Abs(lastDirection.Y)) { //left/right
			if(lastDirection.X > 0) return "right";
			else return "left";
		} else { //up/down
			if(lastDirection.Y > 0) return "back";
			else return "front";
		}
	}

}