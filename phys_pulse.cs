using Godot;
using System;

public partial class phys_pulse : Node3D
{
	[Export]
	private float force = 10;
	private float instTime = 1.5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		instTime -= 0.25f;
		if(instTime <= 0 )
		{
			this.Dispose();
		}
	}

	private void _on_area_3d_body_entered(ILaunchable obj)
	{
		 if(obj is train_ball){
			//	launch the train in a direction with calculations and shit
			Vector3 velocity = new Vector3(0,0,0);
			Vector3 objPos = obj.getGlobalLoc();
			velocity = this.GlobalPosition - objPos;
			velocity = velocity.Normalized();
			

			// call function in train_ball, giving in a vector3 direction+magnitude
			obj.Launch(velocity);
		 }
	}
}
