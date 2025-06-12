using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// https://www.youtube.com/watch?v=e0JLO_5UgQo&t=227s&ab_channel=FinePointCGI
public partial class MultiplayerController : Node {
    public static MultiplayerController Instance;

    [Export] string address = "127.0.0.1";
    [Export] int port = 38257;

    [Export] bool dedicated_server = false;

    private ENetMultiplayerPeer peer;

    public override void _Ready() {
        if (Instance == null) Instance = this;

        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += PlayerConnectedToServer;
        Multiplayer.ConnectionFailed += PlayerConnectionFailed;
        Multiplayer.ServerDisconnected += ServerDisconnected;

        if (OS.HasFeature("dedicated_server")) {
            GD.Print("Godot Dedicated Server Startup");
            Host();
        }
    }

    public bool Host() {
        peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(port, 4, 0, 0, 0);
        if (error != Error.Ok) {
            GD.PrintErr("Error creating Host: " + error);
            return false;
        }

        // Use compression to prioritize bandwidth vs CPU usage
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Waiting for players!");

        if (!OS.HasFeature("dedicated_server")) {
            SendPlayerInfo(Multiplayer.GetUniqueId(), GetPlayerName());
        }

        return true;
    }

    public bool Join() {
        string ip = (GetTree().CurrentScene as MultiplayerMenu).GetIP();

        peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(!string.IsNullOrEmpty(ip) ? ip : address, port);

        if (error != Error.Ok) {
            return false;
        }

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Player connected!");
        return true;
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
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/test_christian.tscn");
        GetTree().ChangeSceneToPacked(scene);
    }

    public void Start() {
        if (GameManager.Instance.players.Count < 2) {
            (GetTree().CurrentScene as MultiplayerMenu).NotEnoughPlayers();
        } else {
            Rpc(MethodName.LoadGame);
        }
    }

    // Called on client only when connection fails
    private void PlayerConnectionFailed() {

    }


    // Called on client only when connected to server
    private void PlayerConnectedToServer() {
        RpcId(1, MethodName.SendPlayerInfo, Multiplayer.GetUniqueId(), GetPlayerName());
        (GetTree().CurrentScene as MultiplayerMenu).OnJoinConnected();
    }

    // Called on client and server when player connects
    private void PeerConnected(long id) {

    }

    // Called on client and server when player disconnects
    private void PeerDisconnected(long id) {
        // TODO: remove player node from scene, or otherwise handle disconnection
        if (GameManager.Instance.players.ContainsKey(id)) {
            GameManager.Instance.players.Remove(id);
        }

        if (GameManager.Instance.players.Count == 0) {
            GameManager.Instance.ClearAllData();
        }
    }

    private void ServerDisconnected() {
        DisconnectAllPlayers();
    }

    private void DisconnectAllPlayers() {
        GetTree().ChangeSceneToFile("res://Scenes/Ending.tscn");

        Multiplayer.MultiplayerPeer.Close();
        Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();

        GameManager.Instance.ClearAllData();
    }

    private string GetPlayerName() {
        string playerName = (GetTree().CurrentScene as MultiplayerMenu).GetPlayerName();
        return !string.IsNullOrEmpty(playerName) ? playerName : "Player " + Multiplayer.GetUniqueId();
    }
}