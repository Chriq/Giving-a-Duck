using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MapManager : Node {
    public static MapManager Instance;

    private const int TILE_SIZE = 16;
    private const int CHUNK_SIZE = 32;
    private Vector2I MAP_DIMENSIONS = new(2, 4); // (rows, cols)


    public int currentChunk { get; private set; }
    private Dictionary<int, Node> loadedChunks = new();

    public override void _Ready() {
        Instance = this;
    }

    public void UpdateActive(Vector2 pos) {
        int playerChunk = GetChunkFromPosition(pos);
        currentChunk = playerChunk;

        int[] chunksToLoad = GetChucksToLoad(pos);

        foreach (int loadedChunk in loadedChunks.Keys) {
            if (!chunksToLoad.Contains(loadedChunk)) {
                loadedChunks[loadedChunk].QueueFree();
                loadedChunks.Remove(loadedChunk);
            }
        }

        foreach (int chuck in chunksToLoad) {
            if (!loadedChunks.Keys.Contains(chuck)) {
                string path = $"res://Scenes/chunk_0{chuck}.tscn";
                ResourceLoader.LoadThreadedRequest(path);
                PackedScene scene = (PackedScene)ResourceLoader.LoadThreadedGet(path);

                Node2D c = scene.Instantiate<Node2D>();
                c.Name = "Chunk" + chuck;
                c.Position = GetChunkPositionFromId(chuck);

                GetTree().CurrentScene.AddChild(c);

                loadedChunks.Add(chuck, c);
            }
        }
    }

    private int GetChunkFromPosition(Vector2 globalPos) {
        int x = (int)Mathf.Floor(globalPos.X / (CHUNK_SIZE * TILE_SIZE));
        int y = (int)Mathf.Floor(globalPos.Y / (CHUNK_SIZE * TILE_SIZE));

        return (int)(MAP_DIMENSIONS.Y * y + x);
    }

    private Vector2 GetChunkPositionFromId(int id) {
        float x = id % MAP_DIMENSIONS.Y;
        float y = id / MAP_DIMENSIONS.Y;

        //GD.Print($"{x}, {y}, id: {id}");

        return new Vector2(x * CHUNK_SIZE * TILE_SIZE, y * CHUNK_SIZE * TILE_SIZE);
    }

    public int[] GetChucksToLoad(Vector2 playerPos) {
        float col = playerPos.X / (CHUNK_SIZE * TILE_SIZE) - 0.5f;
        float row = playerPos.Y / (CHUNK_SIZE * TILE_SIZE) - 0.5f;

        List<int> chunksToCheck = new() {
            (int)(MAP_DIMENSIONS.Y * Mathf.Floor(row) + Mathf.Floor(col)),
            (int)(MAP_DIMENSIONS.Y * Mathf.Floor(row) + Mathf.Ceil(col)),
            (int)(MAP_DIMENSIONS.Y * Mathf.Ceil(row) + Mathf.Floor(col)),
            (int)(MAP_DIMENSIONS.Y * Mathf.Ceil(row) + Mathf.Ceil(col)),
        };

        foreach (int c in chunksToCheck.ToArray()) {
            if (c < 0 || c > MAP_DIMENSIONS.X * MAP_DIMENSIONS.Y - 1) {
                chunksToCheck.Remove(c);
            }
        }

        return chunksToCheck.ToArray();

    }

    public void ResetMap() {
        loadedChunks.Clear();
    }
}
