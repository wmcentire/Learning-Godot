using Godot;
using System;

public partial class CharacterBody3D : Godot.CharacterBody3D
{
	private float Speed = 5.0f;
	[Export] public float walkSpeed = 7.0f;
	[Export] public float runSpeed = 21.0f;
	[Export] public float slideSpeed = 24.0f;
	[Export] public float mouseSens = 0.3f;
	[Export] public float minAngle = 50f;
	[Export] public float maxAngle = 50f;
	private bool isSliding = false;
	public const float JumpVelocity = 4.5f;
	private Camera3D camera = null;
	private Node3D pl_mesh = null;
	private Node3D camPivotX;
	private Node3D camPivotY;
	private IWeapon phys_gun;
	[Export] private NodePath pathY;
	[Export] private NodePath pathX;
	[Export] private NodePath pathCam;
	[Export] private NodePath pathMesh;
	[Export] private NodePath physGun;


	// gameplay values
	private int health;
	private int score;
	private int kills;
	private int deaths;

	private Vector3 syncPos = new Vector3(0,0,0);
	private Vector3 syncRot = new Vector3(0,0,0);

	public override void _Ready()
	{
		// THIS PART IS BROKEN
		// PLEASE FIX
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));

		camPivotY = GetNode<Node3D>(pathY);
//		camPivotX = GetNode<Node3D>(pathX);
		camera = GetNode<Camera3D>(pathCam);
        phys_gun = (IWeapon)GetNode<tLauncher>(physGun);


        // testing to see 
        if (camera == null)
		{
			GD.Print("cam not found");
		}
		else
		{
			GD.Print("cam found");
		}
		
		//pl_mesh = GetNode<Node3D>(pathMesh);


		Input.MouseMode = Input.MouseModeEnum.Captured;

		Speed = walkSpeed;
        if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			camera.MakeCurrent();
		}

    }


    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = 9.8f;

	public override void _PhysicsProcess(double delta)
	{
		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{

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
				phys_gun.shoot();
			}

			if (direction != Vector3.Zero)
			{
				if(IsOnFloor() && Input.IsActionPressed("pl_mv_sld")) Speed = slideSpeed;
				velocity.X = direction.X * Speed ;
				velocity.Z = direction.Z * Speed ;
			}
			else
			{
				if(IsOnFloor() && Input.IsActionPressed("pl_mv_sld")) Speed = slideSpeed;
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed );
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed );
			}

			Velocity = velocity;
			MoveAndSlide();
			syncPos = GlobalPosition;
		}
		else
		{
			GlobalPosition = GlobalPosition.Lerp(syncPos,.5f);
		}
		

    }
    // Change direction based on mouse movements
    public override void _UnhandledInput(InputEvent @event){
        // got this code for the camera rotation from this site: 	
        // https://godotforums.org/d/26795-solved-third-person-camera-issue
        // I for the life of me could not figure out how to use the mouse movement for camera stuff
        if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {
            // OLD 3RD PERSON CAMERA STUFF
            //if (@event is InputEventMouseMotion eventMouseMotion)
            //{
            //	eventMouseMotion = @event as InputEventMouseMotion;
            //	camPivotX.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.Y * mouseSens));
            //	camPivotY.RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * mouseSens));

            //	Vector3 cameraRotation = camPivotX.RotationDegrees;
            //	cameraRotation.X = Mathf.Clamp(cameraRotation.X, -minAngle, maxAngle);
            //	cameraRotation.Z = Mathf.Clamp(cameraRotation.Z, 0, 0);

            //	camPivotX.RotationDegrees = cameraRotation;
            //	GD.Print(new Vector3(cameraRotation.X, cameraRotation.Y, cameraRotation.Z));
            //	// why :(
            //	// yay visual studio!! :)
            //}

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
                GD.Print(new Vector3(cameraRotation.X, cameraRotation.Y, cameraRotation.Z));

				//GlobalRotation = new Vector3(cameraRotation.X, 0f,0f);
            }
            syncRot = GlobalRotation;

        }
        else
		{
            GlobalRotation = GlobalRotation.Lerp(syncRot, .5f);

        }
    }
}
