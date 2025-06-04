using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// https://www.youtube.com/watch?v=e0JLO_5UgQo&t=227s&ab_channel=FinePointCGI
public partial class MultiplayerController : Node {
    public static MultiplayerController Instance;

    [Export] string address = "127.0.0.1";
    [Export] int port = 1234;

    [Export] bool dedicated_server = false;

    private ENetMultiplayerPeer peer;

    public override void _Ready() {
        if (Instance == null) Instance = this;

        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += PlayerConnectedToServer;
        Multiplayer.ConnectionFailed += PlayerConnectionFailed;

        if (OS.HasFeature("dedicated_server")) {
            GD.Print("Godot Dedicated Server Startup");
            Host();
        }
    }

    public void Host() {
        peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(port, 32, 0, 0, 0);
        if (error != Error.Ok) {
            GD.PrintErr("Error creating Host: " + error);
            return;
        }

        // Use compression to prioritize bandwidth vs CPU usage
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Waiting for players!");
        SendPlayerInfo(Multiplayer.GetUniqueId(), "Player " + Multiplayer.GetUniqueId());
    }

    public void Join() {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(address, port);
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Player connected!");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SendPlayerInfo(int id, string name) {
        if (!GameManager.Instance.players.ContainsKey(id)) {
            GameManager.Instance.players[id] = new(id, name);
        }

        if (Multiplayer.IsServer()) {
            foreach (PlayerInfo p in GameManager.Instance.players.Values) {
                Rpc(MethodName.SendPlayerInfo, p.id, p.name);
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void LoadGame() {
        // PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/main.tscn");
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/devtest.tscn");
        GetTree().ChangeSceneToPacked(scene);
    }

    public void Start() {
        Rpc(MethodName.LoadGame);
    }

    // Called on client only when connection fails
    private void PlayerConnectionFailed() {

    }


    // Called on client only when connected to server
    private void PlayerConnectedToServer() {
        RpcId(1, MethodName.SendPlayerInfo, Multiplayer.GetUniqueId(), "Player " + Multiplayer.GetUniqueId());
    }

    // Called on client and server when player connects
    private void PeerConnected(long id) {

    }

    // Called on client and server when player disconnects
    private void PeerDisconnected(long id) {
        GameManager.Instance.players.Remove(id);
        // TODO: remove player node from scene, or otherwise handle disconnection
    }
}