using Godot;
using System;
using System.Collections.Generic;

public partial class game_manager : Node
{
	public static List<player_info> Players = new List<player_info>();
	public int mouseState = 0;
    public static gameState gamestate;
    public enum gameState
    {
        Active,
        Waiting,
        Over
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        gamestate = gameState.Waiting;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// switches mouse mode between captured (control camera) and visible (use menus)
        if (Input.IsActionJustPressed("escape"))
        {
			if(mouseState== 0)
			{
                Input.MouseMode = Input.MouseModeEnum.Visible;
				mouseState = 1;
			}
			else
			{
                Input.MouseMode = Input.MouseModeEnum.Captured;
				mouseState = 0;
            }

        }
    }
}
