using Godot;
using System;
using System.Threading;

public partial class tLauncher : Node3D, IWeapon
{
	[Export]
	private PackedScene Shot;
	[Export]
	private Node3D loc;
	[Export]
	private float reloadTime = 2.0f;

	private float reloading = 0;
	private bool readyToFire = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!readyToFire)
		{
			reloading -= 0.5f;
		}
		if(reloading <= 0)
		{
			readyToFire = true;
		}
	}
	
	public void shoot()
	{
		// need to do more logic
		if (readyToFire)
		{
			readyToFire = false;
			reloading = reloadTime;
            Shot.Instantiate<Node3D>();
			GD.Print("Shooting");
        }
        else
		{
			GD.Print("Cannot Fire");
		}
	}
}
