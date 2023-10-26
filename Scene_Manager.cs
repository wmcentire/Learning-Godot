using Godot;
using System;

public partial class Scene_Manager : Node
{
	[Export]
	private PackedScene playerScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int index = 0;
		foreach(var item in game_manager.Players)
		{
			level currentPlayer = playerScene.Instantiate<level>();
			currentPlayer.Name = item.name;
			AddChild(currentPlayer);
			foreach (Node3D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
			{
				if(int.Parse(spawnPoint.Name) == index)
				{
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
