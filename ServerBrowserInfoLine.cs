using Godot;
using System;

public partial class ServerBrowserInfoLine : HBoxContainer
{
    [Signal]
    public delegate void JoinGameEventHandler(string ip);
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        
    }

    private void _on_join_lan_button_down()
    {
        EmitSignal(SignalName.JoinGame, GetNode<Label>("Ip").Text);
    }
}
