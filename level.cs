using Godot;
using System;

public partial class level : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//foreach(Node c in GD.())
		{
			//Mathf.DegToRad
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsKeyPressed(Key.Escape))
		{
			// exit mouse
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
