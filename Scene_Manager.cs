using Godot;
using System;

public partial class Scene_Manager : Node
{
	[Export]
	private PackedScene playerScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        spawnPlayers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void spawnPlayers()
	{
        int index = 0;
        foreach (var item in game_manager.Players)
        {
            CharacterBody3D currentPlayer = playerScene.Instantiate<CharacterBody3D>();
            currentPlayer.Name = item.Id.ToString(); // giving the player a multiplayer id

            currentPlayer.SetName(item.name); // setting the display name

            AddChild(currentPlayer);
            // spawns in a character in each session for each player
            foreach (Node3D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
            {
                if (int.Parse(spawnPoint.Name) == index)
                {
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                }
            }
            index++;
        }
    }
}
