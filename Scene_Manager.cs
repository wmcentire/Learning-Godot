using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Scene_Manager : Node
{
	[Export]
	private PackedScene playerScene;
    [Export]
    private float respawnTime;

    private List<CharacterBody3D> players;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        spawnPlayers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        CheckPlayerState();
	}

    /// <summary>
    /// This runs when a new round is initiated.
    /// </summary>
	public void spawnPlayers() 
	{
        players = new List<CharacterBody3D>();
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

    public void playerDeath(string id)
    {

    }

    private void CheckPlayerState()
    {
        GD.Print();
        foreach (CharacterBody3D player in players)
        {
            switch (player.plrstt)
            {
                case CharacterBody3D.PlayerState.Alive:
                    GD.Print(player.Name + " is alive!");
                    break;
                case CharacterBody3D.PlayerState.Dead:
                    GD.Print(player.Name + " DIED!!!!");
                    break;
                case CharacterBody3D.PlayerState.Spectating: 
                    
                    break;

            }
        }
    }
}
