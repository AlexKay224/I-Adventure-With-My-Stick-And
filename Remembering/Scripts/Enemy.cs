using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class Enemy : CharacterBody2D
{	

	[Export]
	public static float CONTACT_DAMAGE = 5;

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}

}
