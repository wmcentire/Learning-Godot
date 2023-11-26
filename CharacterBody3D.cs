using Godot;
using System;

public partial class CharacterBody3D : Godot.CharacterBody3D //  I AM VERY SORRY ABOUT THE NAME
{
	private float Speed = 10.0f;
	[Export] public float walkSpeed = 7.0f;
	[Export] public float runSpeed = 21.0f;
	[Export] public float slideSpeed = 14.0f;
	[Export] public float mouseSens = 0.3f;
	[Export] public float minAngle = 50f;
	[Export] public float maxAngle = 50f;
	[Export] public float IFrameLen = 1.5f;
	
	private Vector3 camForm;
	private bool isSliding = false;
	public const float JumpVelocity = 10.0f;
	private Camera3D camera = null;
	private Node3D pl_mesh = null;
	private Node3D camPivotX;
	private Node3D camPivotY;
	private IWeapon phys_gun;
	private Label3D nameTag;
	private float IFrames = 0;
	public PlayerState plrstt;
	public bool IsAuth = false;
	private CollisionShape3D collisions;

	// relative pathways
    [Export] private NodePath pathY;
	[Export] private NodePath pathX;
	[Export] private NodePath pathCam;
	[Export] private NodePath pathMesh;
	[Export] private NodePath physGun;
	[Export] private NodePath namePath;
	[Export] private NodePath colPath;

	// gameplay values
	[Export]
	private string dispname;
    [Export]
    private int health;
	[Export]
	private int maxHealth = 100;

	// synced values
	private Vector3 syncPos = new Vector3(0,0,0);
	private Vector3 syncRot = new Vector3(0,0,0);
    private Vector3 camFormUniversal = new Vector3(0,0,0);
	private int syncHealth;
	

	public enum PlayerState
	{
		Alive,
		Spectating, // probably used later on
		Dead,
		Respawning
	}

    public void SetName(string Name)
	{
		dispname = Name;
	}

	public int GetHealthVal()
	{
		return health;
	}

	public int GetMaxHealthVal()
	{
		return maxHealth;
	}

	public override void _Ready()
	{
		
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));

		// getting objects from paths
		camPivotY = GetNode<Node3D>(pathY);
//		camPivotX = GetNode<Node3D>(pathX);
		camera = GetNode<Camera3D>(pathCam);
		collisions = GetNode<CollisionShape3D>(colPath);
        phys_gun = (IWeapon)GetNode<tLauncher>(physGun);
		phys_gun.setPID(Name);

        // testing to see 
        if (camera == null)
		{
			GD.Print("cam not found");
		}
		else
		{
			GD.Print("cam found");
		}

		Input.MouseMode = Input.MouseModeEnum.Captured;

		plrstt = PlayerState.Alive;

		health = maxHealth;

        Speed = walkSpeed;
        if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			camera.MakeCurrent();
			//nameTag.Hide();
			IsAuth = true;
            camForm = new Vector3(camera.GlobalPosition.X, camera.GlobalPosition.Y, camera.GlobalPosition.Z);

        }
        //nameTag = GetNode<Label3D>(namePath);
        //nameTag.Text = dispname;
    }

	public void playerDamage(int damage, string id)
	{
		//if (plrstt != PlayerState.Alive)
		{
			if (IFrames <= 0)
			{
				IFrames = IFrameLen;
				//GD.Print("Invincibility Frames set to " + IFrames);

				health -= damage;
				GD.Print("Player " + this.Name + " was damaged by " + id + " and has " + health + " health remaining!");
				if (health <= 0)
				{
					health = 0;
					if (id != this.Name)
					{
						GD.Print("hit by another");
						Rpc("AddPoints", id);
						//AddPoints(id);
					}
					else
					{
						GD.Print("hit by self");
					}
					plrstt = PlayerState.Dead;
				}
			}
			else
			{
				//GD.Print("Invincible for " + IFrames + " seconds");
			}
		}
	}

    public void RespawnPlayer(Vector3 spawnLoc)
	{
		health = maxHealth;
		GlobalPosition = spawnLoc;
		plrstt = PlayerState.Alive;
		CollisionStateSwitch();
	}

	// disables and enables the collision shape
	public void CollisionStateSwitch()
	{
		//if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())

		collisions.Disabled = !collisions.Disabled;
	}

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = 19.8f;

	// this is where the player movement and other physics is kept
	public override void _PhysicsProcess(double delta)
	{
		// checks to see if the player node matches the player session
		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			if (plrstt == PlayerState.Alive)  // checks to make sure player is alive
			{


				float actlSpd = Speed;
				Vector3 velocity = Velocity;

				// Add the gravity.
				if (!IsOnFloor())
					velocity.Y -= gravity * (float)delta;

				// Handle Jump.
				if (Input.IsActionJustPressed("pl_jmp") && IsOnFloor())
					velocity.Y = JumpVelocity;

				// Get the input direction and handle the movement/deceleration.
				// As good practice, you should replace UI actions with custom gameplay actions.
				Vector2 inputDir = Input.GetVector("pl_mv_lft", "pl_mv_rht", "pl_mv_fwd", "pl_mv_bck");
				Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
				// REPLACE ABOVE WITH NEW WAY TO GET DIRECTION STUFF

				if (Input.IsActionPressed("pl_rt_cm"))
				{

				}

				if (Input.IsActionPressed("pl_sht"))
				{
					phys_gun.setShoot(true);
				}

				if (direction != Vector3.Zero)
				{
					if (IsOnFloor() && Input.IsActionPressed("pl_mv_sld")) actlSpd = slideSpeed;
					velocity.X = direction.X * actlSpd;
					velocity.Z = direction.Z * actlSpd;
				}
				else
				{
					if (IsOnFloor() && Input.IsActionPressed("pl_mv_sld")) actlSpd = slideSpeed;
					velocity.X = Mathf.MoveToward(Velocity.X, 0, actlSpd);
					velocity.Z = Mathf.MoveToward(Velocity.Z, 0, actlSpd);
				}

				Velocity = velocity;
				MoveAndSlide();
				syncPos = GlobalPosition;
				syncRot = GlobalRotation;
				camFormUniversal = camForm;
				syncHealth = health;

			}
        }
        else
		{
			// syncing positions and rotations
			GlobalPosition = GlobalPosition.Lerp(syncPos,.5f);
            GlobalRotation = GlobalRotation.Lerp(syncRot, .5f);
			camForm = camForm.Lerp(camFormUniversal,.5f);
			health = syncHealth;


			// nametags always facing player
			//Vector3 tagDir = camForm - nameTag.GlobalPosition;

			//nameTag.GlobalRotation = tagDir.Normalized();
        }


    }

	// putting all the non-physics stuff that generally needs to be dealt with in here
    public override void _Process(double delta)
    {		
		if(IFrames > 0)
		{
			IFrames -= (float)delta;
			//GD.Print(IFrames);
		}
		
    }

    // Change direction based on mouse movements
    public override void _UnhandledInput(InputEvent @event){
        // got this code for the camera rotation from this site: 	
        // https://godotforums.org/d/26795-solved-third-person-camera-issue
        // I for the life of me could not figure out how to use the mouse movement for camera stuff
        if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {

            if (@event is InputEventMouseMotion eventMouseMotion)
            {
                eventMouseMotion = @event as InputEventMouseMotion;
                RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * mouseSens));
                camPivotY.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.Y * mouseSens));
				//Mathf.Clamp(camPivotY.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(45));
                Vector3 cameraRotation = camPivotY.RotationDegrees;
                cameraRotation.X = Mathf.Clamp(cameraRotation.X, -minAngle, maxAngle);
                cameraRotation.Z = Mathf.Clamp(cameraRotation.Z, 0, 0);

                camPivotY.RotationDegrees = cameraRotation;
                //GD.Print(new Vector3(cameraRotation.X, cameraRotation.Y, cameraRotation.Z));

				//GlobalRotation = new Vector3(cameraRotation.X, 0f,0f);
            }
            syncRot = GlobalRotation;

        }
        else
		{

        }
    }

	// RPC METHODS

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void AddPoints(string id)
	{
		game_manager.AddPoints(id);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void PlayerDeath()
	{
		
	}

}
