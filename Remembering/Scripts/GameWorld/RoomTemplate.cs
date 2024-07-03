using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

public partial class RoomTemplate : Node2D
{

	[Export]
	public Godot.Collections.Array<PackedScene> spawnedEnemies { get; set; }
	private int numEnemies;

	private Node2D gateContainer;
	private Node2D entranceContainer;
	private Node2D enemySpawns;
	private Area2D playerDetector;

	private bool roomCleared;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		roomCleared = false;
		gateContainer = GetNode<Node2D>("Gates");
		entranceContainer = GetNode<Node2D>("Entrance");
		enemySpawns = GetNode<Node2D>("EnemySpawns");
		playerDetector = GetNode<Area2D>("PlayerDetector");

		numEnemies = enemySpawns.GetChildCount();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OpenGates() {
		foreach(var gate in gateContainer.GetChildren()) {
			if(gate.HasMethod("OpenGate")) gate.CallDeferred("OpenGate");
		}
	}

	public void CloseGates() {
		foreach(var gate in gateContainer.GetChildren()) {
			if(gate.HasMethod("CloseGate")) gate.CallDeferred("CloseGate");
		}
	}

	public void SpawnEnemies() {
		var spawns = enemySpawns.GetChildren();
		for(int i = 0; i < spawns.Count; i++) {
			Node2D spawn = (Node2D) spawns[i];
			PackedScene enemy_scn = spawnedEnemies[i];
			Enemy enemy = enemy_scn.Instantiate<Enemy>();
			enemy.TreeExited += OnEnemyDefeated;
			spawn.AddChild(enemy);

			enemy.GlobalPosition = spawn.GlobalPosition;
			CloseGates();
		}
	}

	public void OnEnemyDefeated() {
		numEnemies--;
		if(numEnemies <= 0) {
			OpenGates();
			roomCleared = true;
		}
	}

	public void OnPlayerDetected(Node2D body) {
		if(body is Player) {
			playerDetector.QueueFree();
			SpawnEnemies();
		}
	}
}
