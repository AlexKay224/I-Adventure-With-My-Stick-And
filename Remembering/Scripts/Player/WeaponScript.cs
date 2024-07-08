using Godot;
using System;

[Icon("res://Remembering/Art/Misc/Justin's Icons/32x32/swords.png")]
public partial class WeaponScript : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
