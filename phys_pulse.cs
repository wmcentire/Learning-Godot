using Godot;
using System;

public partial class phys_pulse : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_area_3d_body_entered(Node obj)
	{
		// if(obj is train_ball){
		//	launch the train in a direction with calculations and shit
		// }
	}
}
