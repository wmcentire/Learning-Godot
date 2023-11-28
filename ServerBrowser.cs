using Godot;
using LearningGodot;
using System;
using System.Text.Json;

public partial class ServerBrowser : Control
{
    [Export]
    PacketPeerUdp broadcaster = new PacketPeerUdp();
    [Export]
    PacketPeerUdp listener = new PacketPeerUdp();
    [Export]
    int listenPort = 8911;
    [Export]
    int hostPort = 8912;
    [Export]
    string broadcastAddress = "192.168.86.255";

    Timer broadcastTimer;
    ServerInfo serverInfo;

    public override void _Ready()
    {
        broadcastTimer = GetNode<Timer>("BroadcastTimer");
    }

    /// <summary>
    /// Starts the broadcasting
    /// </summary>
    /// <param name="name"></param>
    private void setupBroadcast(string name)
    {
        serverInfo = new ServerInfo()
        {
            Name = name,
            PlayerCount = game_manager.Players.Count
        };

        broadcaster.SetBroadcastEnabled(true);
        broadcaster.SetDestAddress(broadcastAddress, listenPort);

        var ok = broadcaster.Bind(hostPort);
        if(ok == Error.Ok)
        {
            GD.Print("Bound to Broadcast Port "+hostPort.ToString());
        }
        else
        {
            GD.Print("Failed to bind to Broadcast Port");
        }

        broadcastTimer.Start();
    }

    private void _on_broadcast_timer_timeout()
    {
        GD.Print("Broadcasting Game");
        serverInfo.PlayerCount = game_manager.Players.Count;

        string json = JsonSerializer.Serialize(serverInfo);
        var packet = json.ToAsciiBuffer();
    }

}
