using Godot;
using System;

public partial class CharacterBody3D : Godot.CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float slideMultiplier = 2.2f;
	public const float JumpVelocity = 4.5f;
	private Camera3D camera = null;
	//private Spatial pl_mesh = null;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
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
		if (direction != Vector3.Zero)
		{
			float mlt = 1.0f;
			if(IsOnFloor() && Input.IsActionPressed("pl_mv_sld")) mlt = slideMultiplier;
			velocity.X = direction.X * Speed * mlt;
			velocity.Z = direction.Z * Speed * mlt;
		}
		else
		{
			float mlt = 1.0f;
			if(IsOnFloor() && Input.IsActionPressed("pl_mv_sld")) mlt = slideMultiplier;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed * mlt);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed * mlt);
		}

		Velocity = velocity;
		MoveAndSlide();
		
		
	}
	// Change direction based on mouse movements
	public override void _UnhandledInput(InputEvent @event){
		// got this code for the camera rotation from this site: 	
		// https://godotforums.org/d/26795-solved-third-person-camera-issue
		// I for the life of me could not figure out how to use the mouse movement for camera stuff
		 if (@event is InputEventMouseMotion eventMouseMotion)
		{
			eventMouseMotion = @event as InputEventMouseMotion;
			camPivot.RotateX(Mathf.deg2Rad(-eventMouseMotion.Relative.y * mouseSens));
			camPivot.RotateY(Mathf.deg2Rad(-eventMouseMotion.Relative.x * mouseSens));


			Vector3 cameraRotation = camPivot.RotationDegrees;
			cameraRotation.x = Mathf.Clamp(cameraRotation.x, -minAngle, maxAngle);
			cameraRotation.z = Mathf.Clamp(cameraRotation.z, 0, 0);

			camPivot.RotationDegrees = cameraRotation;
			GD.Print(new Vector3(cameraRotation.x, cameraRotation.y, cameraRotation.z));
			// why :(
		}
	}
}
