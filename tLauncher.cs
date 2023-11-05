using Godot;
using System;

public partial class tLauncher : Node3D
{
	[Export]
	private PackedScene Shot;
	[Export]
	private Node3D loc;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void shoot()
	{
		// need to do more logic
		Shot.Instantiate<Node3D>();
	}
}
