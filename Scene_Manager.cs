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
            players.Add(currentPlayer);
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

    private CharacterBody3D getPNodeById(string id)
    {
        foreach (var item in players)
        {
            if (item.Name == id) return item;
        }
        throw new Exception("No matching player info found for id: " + id);
    }

    /// <summary>
    /// Searches the Players list in game_manager for a player_info that has the given id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private player_info getPInfoById(string id)
    {
        foreach(var item in game_manager.Players)
        {
            if(item.Id.ToString() == id) return item;
        }
        throw new Exception("No matching player info found for id: " + id);
    }

    /// <summary>
    /// Takes a player id, and spawns in a new instance of a player
    /// if there is a matching id in the list of playerInfo stored inside of game_manager
    /// </summary>
    /// <param name="id"></param>
    public void spawnPlayer(string id)
    {
        GD.Print("Attempting to spawn player " + id);
        player_info temp = getPInfoById(id);
        if(temp != null)
        {
            //CharacterBody3D currentPlayer = playerScene.Instantiate<CharacterBody3D>();
            //currentPlayer.Name = temp.Id.ToString(); // giving the player a multiplayer id

            //currentPlayer.SetName(temp.name); // setting the display name

            //AddChild(currentPlayer);
            //players.Add(currentPlayer);
            //  /\ OLD STUFF /\

            CharacterBody3D player = getPNodeById(id);
            player.Hide();
            temp.deaths++;
        }
        else
        {
            throw new Exception("No matching player info found for id: " + id);
        }
        
        
    }

    /// <summary>
    /// Takes a player id, and kills the player with matching id
    /// </summary>
    /// <param name="id"></param>
    public void killPlayer(string id)
    {
        GD.Print("Attempting to kill player " + id);
        player_info temp = getPInfoById(id);
        if(temp != null )
        {
            CharacterBody3D player = getPNodeById(id);
            player.
        }
    }
    /// <summary>
    /// Checks the PlayerState of every player instance stored in the players list
    /// </summary>
    private void CheckPlayerState()
    {
        //GD.Print();
        foreach (var player in players)
        {
            //GD.Print("it is what it is");
            switch (player.plrstt)
            {
                case CharacterBody3D.PlayerState.Alive:
                    //GD.Print(player.Name + " is alive!");
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
