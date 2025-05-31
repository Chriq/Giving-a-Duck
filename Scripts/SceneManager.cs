using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class SceneManager : Node2D {
    [Export] PackedScene playerPrefab;

    public override void _Ready() {
        foreach (PlayerInfo p in GameManager.Instance.players.Values) {
            SpawnPlayer(p.id);
        }
    }
    
    public void SpawnPlayer(int playerId) {
        if (GameManager.Instance.players.ContainsKey(playerId))
            if (GameManager.Instance.players[playerId].playerObject != null)
                return;

        /* Spawn Player */
        PlayerController player = playerPrefab.Instantiate<PlayerController>();
        player.playerId = playerId;
        GD.Print($"Peer {Multiplayer.GetUniqueId()} - PlayerSpawn ({playerId})");

        GetTree().CurrentScene.AddChild(player);
        
        // Random Spawn Point
        if (playerId == Multiplayer.GetUniqueId())
        {
            Array<Node> spawnPoints = GetTree().GetNodesInGroup("PlayerSpawn");
            
            Random rng = new Random();
            int index = 2; // rng.Next(spawnPoints.Count);

            GD.Print(index);

            Node2D spawn = spawnPoints[index] as Node2D;

            player.GlobalPosition = spawn.GlobalPosition;
        }
        else
        {
            player.GlobalPosition = new(-1000, -1000);
        }
        
        if (GameManager.Instance.players.ContainsKey(playerId))
        {
            GameManager.Instance.players[playerId].playerObject = player;
        }
        else
        {
            PlayerInfo info = new PlayerInfo(playerId);
            info.playerObject = player;

            GameManager.Instance.players.Add(playerId, info);
        }
        
        // GameManager.Instance.InitializeItems();
    }
}
