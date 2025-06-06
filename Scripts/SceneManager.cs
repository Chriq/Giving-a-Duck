using Godot;
using Godot.Collections;
using System.Linq;

public partial class SceneManager : Node2D {
    [Export] PackedScene playerPrefab;

    public override void _Ready() {
        Array<Node> spawnPoints = GetTree().GetNodesInGroup("PlayerSpawn");

        int index = 0;
        foreach (PlayerInfo p in GameManager.Instance.players.Values) {
            PlayerController player = playerPrefab.Instantiate<PlayerController>();
            player.playerId = p.id;
            player.Name = "Player" + p.id;
            AddChild(player);

            if (index < spawnPoints.Count) {
                Node2D spawn = spawnPoints[index] as Node2D;
                player.GlobalPosition = spawn.GlobalPosition;
            } else {
                GD.PrintErr("Not enough spawn points for players!");
            }

            index++;
        }

        GameManager.Instance.InitializeItems();
    }

    private Array<int> GetRandomSpawnChunks() {
        Array<int> chunks = new();
        for (int i = 0; i < Consts.NUM_TOTAL_CHUNKS; i++) {
            chunks[i] = i;
        }

        chunks.Shuffle();

        return chunks.Slice(0, GameManager.Instance.players.Count);
    }

}
