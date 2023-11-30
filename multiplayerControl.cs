using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

public partial class multiplayerControl : Control
{
    [Export]
    private int PORT = 8910;
    [Export]
    private string address = "127.0.0.1";
    private ENetMultiplayerPeer peer;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += ConnectedToServer;
        Multiplayer.ConnectionFailed += ConnectionFailed;

        GetNode<ServerBrowser>("ServerBrowser").JoinGame += joinGame;
    }

    // Multiplayer Signals

    /// <summary>
    /// Runs when the connection fails and only on the clients
    /// </summary>
    /// 
    private void ConnectionFailed()
    {
        GD.Print("CONNECTION FAILED");
    }
    /// <summary>
    /// Runs when the connection is successful and only on the clients
    /// </summary>
    /// 
    private void ConnectedToServer()
    {
        GD.Print("CONNECTED TO SERVER");
        RpcId(1,"SendPlayerInfo", GetNode<LineEdit>("Net/NameCon/Name").Text,Multiplayer.GetUniqueId());
    }
    /// <summary>
    /// Runs when a player disconnects and runs on all peers
    /// </summary>
    /// <param name="id">id of player that disconnected</param>
    /// 
    private void PeerDisconnected(long id)
    {
        GD.Print("PLAYER DISCONNECTED: " + id);
        foreach(var item in game_manager.Players)
        {
            if(item.name == id.ToString())
            {
                game_manager.Players.Remove(item);
            }
        }
        var player = GetTree().GetNodesInGroup("Player").ToList();
        foreach( var item in player)
        {
            if(item.Name == id.ToString())
            {
                string name = item.Name;
                item.QueueFree();
                Scene_Manager sc = GetNode<Scene_Manager>("ArenaScene");
                if (sc != null) sc.RemovePlayer(name);
            }
        }
    }
    /// <summary>
    /// Runs when a player connects and runs on all peers
    /// </summary>
    /// <param name="id">id of the player that connected</param>
    /// 
    private void PeerConnected(long id)
    {
        GD.Print("PLAYER CONNECTED: " + id);
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    // buttons
    /// <summary>
    /// Starts the game scene
    /// </summary>
    public void _on_start_game_button_down()
    {
        Rpc("StartGame");

    }
    /// <summary>
    /// runs on all peers
    /// </summary>
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void StartGame()
    {
        GetNode<ServerBrowser>("ServerBrowser").CleanUp();
        foreach (var item in game_manager.Players)
        {
            GD.Print(item.name + " is playing");
        }
        var scene = ResourceLoader.Load<PackedScene>("res://arena_scene.tscn").Instantiate<Node>();
        GetTree().Root.AddChild(scene);
        this.Hide();
    }

    private void HostGame()
    {
        peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(PORT, 8);
        if (error != Error.Ok)
        {
            GD.Print("ERROR CANNOT HOST: " + error.ToString());
            return;
        }
        peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);

        Multiplayer.MultiplayerPeer = peer;
        GD.Print("WAITING FOR PLAYERS...");

        GetNode<ServerBrowser>("ServerBrowser").SetupBroadcast(GetNode<LineEdit>("Net/NameCon/Name").Text + "'s Server");
    }
    public void _on_host_button_down()
    {
        HostGame();

        SendPlayerInfo(GetNode<LineEdit>("Net/NameCon/Name").Text, 1);
    }

    public void _on_join_button_down()
    {
        joinGame(address);
    }

    private void joinGame(string ip)
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(ip, PORT);

        peer.Host.Compress(ENetConnection.CompressionMode.Fastlz);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("JOINING SERVER");
    }

    // sends player info
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SendPlayerInfo(string name, int id)
    {
        player_info playerInfo = new player_info()
        {
            name = name,
            Id = id,
            kills = 0,
            deaths = 0
        };
        if(!game_manager.Players.Contains(playerInfo)) {
            game_manager.Players.Add(playerInfo);
        }
        if (Multiplayer.IsServer())
        {
            foreach(var item in game_manager.Players)
            {
                Rpc("SendPlayerInfo",item.name, item.Id);
            }
        }
    }

}
