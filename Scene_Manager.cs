using Godot;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography.X509Certificates;

public partial class Scene_Manager : Node
{
    // exports
	[Export]
	private PackedScene playerScene;
    [Export]
    private float respawnTime;
    [Export]
    private NodePath deathLocPath;
    [Export]
    private NodePath hpBarPath;
    [Export]
    private NodePath pointDispPath;

    private ProgressBar hpBar;
    private Label pointDisp;
    private Node3D deathLoc;
    private string plrAuthId;

    private List<CharacterBody3D> players;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        hpBar = GetNode<ProgressBar>(hpBarPath);
        deathLoc = GetNode<Node3D>(deathLocPath);
        pointDisp = GetNode<Label>(pointDispPath);
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
                if (currentPlayer.IsAuth)
                {
                    plrAuthId = currentPlayer.Name;
                    hpBar.MaxValue = currentPlayer.GetMaxHealthVal();
                    DisplayServer.WindowSetTitle(plrAuthId);
                }
            }
            index++;
        }
        

    }

    /// <summary>
    /// removes player from the list of players
    /// </summary>
    /// <param name="playerId"></param>
    public void RemovePlayer(string playerId)
    {
        foreach(var player in players)
        {
            if (player.Name == playerId)
            {
                players.Remove(player);
            }
        }
    }

    /// <summary>
    /// Returns a player node based off of the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private CharacterBody3D getPNodeById(string id)
    {
        foreach (var item in players)
        {
            if (item.Name == id) return item;
        }
        throw new Exception("No matching player info found for id: " + id);
    }

    /// <summary>
    /// Selects a random spawnpoint from the list of spawnpoints
    /// </summary>
    /// <returns></returns>
    private Node3D getRandSpawn()
    {
        var rand = new Random();
        int index = rand.Next(GetTree().GetNodesInGroup("PlayerSpawnPoints").Count);
        Godot.Collections.Array<Godot.Node> treeArray = GetTree().GetNodesInGroup("PlayerSpawnPoints");
        return (Node3D)treeArray[index];
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
    public void spawnPlayerById(string id)
    {
        GD.Print("Attempting to spawn player " + id);
        player_info temp = getPInfoById(id);
        if(temp != null)
        {
            CharacterBody3D player = getPNodeById(id);
            player.Show();
            player.RespawnPlayer(getRandSpawn().GlobalPosition);
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
    public void killPlayerById(string id)
    {
        GD.Print("Attempting to kill player " + id);
        player_info temp = getPInfoById(id);
        if(temp != null )
        {
            CharacterBody3D player = getPNodeById(id);
            player.Hide();
            temp.deaths++;
            player.plrstt = CharacterBody3D.PlayerState.Spectating;
            player.GlobalPosition = deathLoc.GlobalPosition;
        }
    }

    /// <summary>
    /// Takes a player and "kills" it
    /// </summary>
    /// <param name="player"></param>
    public void killPlayer(CharacterBody3D player)
    {
        GD.Print("Attempting to kill player " + player.Name);
        player_info temp = getPInfoById(player.Name);
        if (temp != null)
        {
            player.CollisionStateSwitch();
            player.Hide();
            temp.deaths++;
            player.plrstt = CharacterBody3D.PlayerState.Spectating;
            player.GlobalPosition = deathLoc.GlobalPosition;
        }
    }

    public void setHpBar(int hp)
    {
        hpBar.Value = hp;
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
                    //GD.Print(player.Name + " DIED!!!!");
                    //Rpc("killPlayerById", player.Name);
                    killPlayer(player);
                    break;
                case CharacterBody3D.PlayerState.Spectating:
                    //GD.Print(player.Name + "spectating");
                    player.Hide();
                    if (Input.IsActionJustPressed("pl_rspwn") && (player.Name == plrAuthId))
                    {
                        player.plrstt = CharacterBody3D.PlayerState.Respawning;
                    }
                    
                    break;
                case CharacterBody3D.PlayerState.Respawning:
                    spawnPlayerById(player.Name);

                    break;

            }
            if(player.Name == plrAuthId)
            {
                hpBar.Value = player.GetHealthVal();
                player_info curPlr = getPInfoById(player.Name);
                if (curPlr != null)
                {
                    pointDisp.Text = "Points: " + curPlr.points;
                }
            }
        }
    }
}
