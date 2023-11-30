using Godot;
using LearningGodot;
using System;
using System.Linq;
using System.Security.AccessControl;
using System.Text.Json;

public partial class ServerBrowser : Control
{
    [Export]
    PacketPeerUdp broadcaster;
    [Export]
    PacketPeerUdp listener = new PacketPeerUdp();
    [Export]
    int listenPort = 8911;
    [Export]
    int hostPort = 8912;
    [Export]
    string broadcastAddress; //= "192.168.86.255";
    [Export]
    PackedScene ServerInfo;

    [Signal]
    public delegate void JoinGameEventHandler(string ip);

    Timer broadcastTimer;
    ServerInfo serverInfo;

    public override void _Ready()
    {
        broadcastTimer = GetNode<Timer>("BroadcastTimer");
        setupListener();
        string[] addressPieces = Godot.IP.GetLocalAddresses()[3].Split('.',4);
        GD.Print("found address: " + Godot.IP.GetLocalAddresses()[3]);
        broadcastAddress = addressPieces[0]+"."+addressPieces[1]+"."+addressPieces[2]+".255";
        GD.Print(broadcastAddress);
    }

    public override void _Process(double delta)
    {
        if(listener.GetAvailablePacketCount() > 0)
        {
            string serverIP = listener.GetPacketIP();
            int serverPort = listener.GetPacketPort();
            byte[] bytes = listener.GetPacket();

            // converting data from byte array to server info
            ServerInfo info = JsonSerializer.Deserialize<ServerInfo>(bytes.GetStringFromAscii());
            // printing for testing purposes
            GD.Print("server ip " 
                + serverIP 
                + " server port " 
                + serverPort 
                + " server info " 
                + bytes.GetStringFromAscii());

            Node currentNode = GetNode<VBoxContainer>("Panel/VBoxContainer").GetChildren().Where(x => x.Name == info.Name).FirstOrDefault();

            if (currentNode != null)
            {
                currentNode.GetNode<Label>("PlayerCount").Text = info.PlayerCount.ToString();
                currentNode.GetNode<Label>("Ip").Text = serverIP;

                return;
            }

            ServerBrowserInfoLine serverInfo = ServerInfo.Instantiate<ServerBrowserInfoLine>();
            serverInfo.Name = info.Name;
            serverInfo.GetNode<Label>("Name").Text = serverInfo.Name;
            serverInfo.GetNode<Label>("Ip").Text = serverIP;
            serverInfo.GetNode<Label>("PlayerCount").Text = info.PlayerCount.ToString();
            GetNode<VBoxContainer>("Panel/VBoxContainer").AddChild(serverInfo);

            serverInfo.JoinGame += _on_join_game;
        }
    }

    private void _on_join_game(string ip)
    {
        EmitSignal(SignalName.JoinGame, ip);
    }

    private void setupListener()
    {
        var ok = listener.Bind(listenPort);
        if(ok == Error.Ok)
        {
            GD.Print("Bound to Listen Port " + listenPort.ToString());
            GetNode<Label>("Label").Text = "Bound to listen port: true";
        }
        else
        {
            GD.Print("Failed to bind to Listen Port");
            GetNode<Label>("Label").Text = "Bound to listen port: false";

        }
    }

    /// <summary>
    /// Starts the broadcasting
    /// </summary>
    /// <param name="name"></param>
    public void SetupBroadcast(string name)
    {
        broadcaster = new PacketPeerUdp();
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

        broadcaster.PutPacket(packet);
    }

    /// <summary>
    /// Removes server from server browser
    /// </summary>
    public void CleanUp()
    {
        listener.Close();
        broadcastTimer.Stop();
        if(broadcaster != null) { broadcaster.Close(); }
    }
}
