using Godot;
using System;
using System.Threading;

public partial class tLauncher : Node3D, IWeapon
{
	[Export]
	private PackedScene Shot;
	//[Export]
	//private Node3D hitBox;
	[Export]
	private float reloadTime = 1.0f;
	[Export]
	private NodePath shotPath;
	private Node3D shotLoc;
	[Export]
	public bool didShoot = false; // allows gun to shoot across multiplayer sessions
	public string pID;
    private float reloading = 0;
	private bool readyToFire = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Shot.Hide();
		shotLoc = GetNode<Node3D>(shotPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float deltaF = (float)delta;
		// this is SUPPOSED to stop the player from spamming 
		if (!readyToFire)
		{
			reloading -= deltaF;
		}
		if(reloading <= 0)
		{
			readyToFire = true;
		}

		if(didShoot)
		{
			shoot();
		}
        
    }
	
	public void shoot()
	{
		if (readyToFire)
		{
			didShoot = false;
			readyToFire = false;
			reloading = reloadTime;
			//Shot.Instantiate<Node3D>();
			phys_hit shot = Shot.Instantiate<phys_hit>(); // enables hitbox
			GetTree().Root.AddChild(shot);
			shot.GlobalTransform = shotLoc.GlobalTransform;
			shot.id = pID;
        }
        else
		{
		}
	}

    public bool getShoot()
    {
        return didShoot;
    }

    public void setShoot(bool shoot)
    {
        didShoot = shoot;
    }

    public void setPID(string pID)
    {
        this.pID = pID;
    }
}
