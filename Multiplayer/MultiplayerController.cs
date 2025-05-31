using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// https://www.youtube.com/watch?v=e0JLO_5UgQo&t=227s&ab_channel=FinePointCGI
public partial class MultiplayerController : Node {
    public static MultiplayerController Instance;

    [Export] string address = "127.0.0.1";
    [Export] int port = 1234;
    
    private ENetMultiplayerPeer peer;

    public override void _Ready() {
        if (Instance == null) Instance = this;

        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        Multiplayer.ConnectedToServer += PlayerConnectedToServer;
        Multiplayer.ConnectionFailed += PlayerConnectionFailed;
        
        if (OS.HasFeature("dedicated_server"))
        {
            GD.Print("Godot Dedicated Server Startup");
            Host();
        }
    }

    public void Host() {
        peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(port, 32, 0, 0, 0);
        
        if (error != Error.Ok) {
            // TODO: Show error on screen if not dedicated server
            GD.PrintErr("Error creating Host: " + error);
            return;
        }

        GD.Print($"Multiplayer host created on Port {port}");

        // Use compression to prioritize bandwidth vs CPU usage
        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
        
        GD.Print("Waiting for players!");

        if (!OS.HasFeature("dedicated_server"))
        {
            LoadGame();
            SendPlayerInfo(Multiplayer.GetUniqueId(), "Player " + Multiplayer.GetUniqueId());
        }
    }

    public void Join() {
        peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, port);
        
        if (error != Error.Ok) {
            // TODO: Show error on screen
            GD.Print($"Error creating Client: {error}");
            return;
        }

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;

        GD.Print("Player connecting!");
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SendPlayerInfo(int id, string name) {

        SceneManager scene = (SceneManager) GetTree().CurrentScene;
        if (scene != null)
        {
            GD.Print($"Peer {Multiplayer.GetUniqueId()} - PlayerInfo ({id} {name}) - spawn");
            scene.SpawnPlayer(id);
        }
        else if (!GameManager.Instance.players.ContainsKey(id)) {
            GameManager.Instance.players[id] = new(id);
            GD.Print($"Peer {Multiplayer.GetUniqueId()} - PlayerInfo ({id} {name}) - no scene");
        }

        if (Multiplayer.IsServer()) {
            foreach (PlayerInfo p in GameManager.Instance.players.Values) {
                Rpc(MethodName.SendPlayerInfo, p.id, p.name);
            }
        }
    }

    public void LoadGame() {
        // PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/main.tscn");
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://Scenes/test_jack.tscn");

        GetTree().ChangeSceneToPacked(scene);
    }

    // Called on client only when connection fails
    private void PlayerConnectionFailed() {
        GD.PrintErr("ConnectionFailed");
    }

    // Called on client only when connected to server
    private void PlayerConnectedToServer() {
        GD.Print("PlayerConnected");

        RpcId(1, MethodName.SendPlayerInfo, Multiplayer.GetUniqueId(), "Player " + Multiplayer.GetUniqueId());

        LoadGame();
    }

    // Called on client and server when player connects
    private void PeerConnected(long id) {
        GameManager.Instance.players[id] = new((int)id);
    }

    // Called on client and server when player disconnects
    private void PeerDisconnected(long id) {
        GameManager.Instance.players[id].playerObject.QueueFree();
        GameManager.Instance.players.Remove(id);
    }
}