using Godot;
using System;

public partial class PlayerUI : Panel
{

	[Export]
	public Player player;

	//access child nodes
	public ProgressBar healthBar;
	public RichTextLabel healthCount;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//sort out health
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthCount = GetNode<RichTextLabel>("HealthBar/HealthCount");
		UpdateHealth();
	}

	public void UpdateHealth() {
		healthBar.MaxValue = player.MaxHealth;
		healthBar.Value = player.health;
		healthCount.Text = "[center]" + player.health + " / " + player.MaxHealth + "[/center]";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		player.HealthChange += () => UpdateHealth();
	}
}
