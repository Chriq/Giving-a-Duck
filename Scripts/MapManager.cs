using Godot;
using System;
using System.Collections.Generic;

public partial class MapManager : Node {
    public static MapManager Instance;

    private const int TILE_SIZE = 16;
    private const int CHUNK_SIZE = 32;
    private Vector2I MAP_DIMENSIONS = new(2, 3); // (rows, cols)


    private int currentChunk;
    private List<int> loadedChunks = new();

    public override void _Ready() {
        Instance = this;

        // UpdateActive(new Vector2(312, 287));
        // UpdateActive(new Vector2(520, 287));
        //UpdateActive(new Vector2(836, 607));
    }

    public void UpdateActive(Vector2 pos) {

        int chunk = GetChunkFromPosition(pos);
        currentChunk = chunk;

        if (!loadedChunks.Contains(chunk)) {
            string path = $"res://Scenes/chunk_0{chunk}.tscn";
            ResourceLoader.LoadThreadedRequest(path);
            PackedScene scene = (PackedScene)ResourceLoader.LoadThreadedGet(path);

            Node2D c = scene.Instantiate<Node2D>();
            c.Position = pos - (pos % (CHUNK_SIZE * TILE_SIZE));

            GetTree().CurrentScene.AddChild(c);

            loadedChunks.Add(chunk);
        }

        // GD.Print("Player At: ", pos);
        // GD.Print("Chunk #: ", chunk);
        // GD.Print("Position: ", pos - (pos % (CHUNK_SIZE * TILE_SIZE)));
    }

    private int GetChunkFromPosition(Vector2 globalPos) {
        int x = (int)Mathf.Floor(globalPos.X / (CHUNK_SIZE * TILE_SIZE));
        int y = (int)Mathf.Floor(globalPos.Y / (CHUNK_SIZE * TILE_SIZE));

        return (int)(MAP_DIMENSIONS.Y * y + x);
    }

    private Vector2 GetChunkPositionFromId(int id) {
        float x = id % MAP_DIMENSIONS.X;
        float y = (id - x) * MAP_DIMENSIONS.Y;

        return new Vector2(x * CHUNK_SIZE * TILE_SIZE, y * CHUNK_SIZE * TILE_SIZE);
    }

    public int[] GetChucksToLoad(Vector2 playerPos) {
        //int chunk = GetChunkFromPosition(playerPos);


        // subtract 0.5
        float col = playerPos.X / (CHUNK_SIZE * TILE_SIZE) - 0.5f;
        float row = playerPos.Y / (CHUNK_SIZE * TILE_SIZE) - 0.5f;

        //int nextCol = Mathf.CeilToInt(col - (col % (int)col));
        //int nextRow = Mathf.CeilToInt(row - (row % (int)row));

        return [
            (int)(MAP_DIMENSIONS.Y * Mathf.Floor(row) + Mathf.Floor(col)),
            (int)(MAP_DIMENSIONS.Y * Mathf.Floor(row) + Mathf.Ceil(col)),
            (int)(MAP_DIMENSIONS.Y * Mathf.Ceil(row) + Mathf.Floor(col)),
            (int)(MAP_DIMENSIONS.Y * Mathf.Ceil(row) + Mathf.Ceil(col)),
        ];

        // int chunk = (int)(MAP_DIMENSIONS.Y * (int) row + (int) col);



        // List<int> chunks = new();
        // chunks.Add(chunk);
        // if (col % (int)col >= 0.8f) {
        //     chunks.Add(chunk + 1);
        // } else if(col % (int)col <= 0.2f) {
        //     chunks.Add(chunk - 1);
        // }


        // int idx = row * MAP_DIMENSIONS.Y + col;
        // GD.Print("Player SHOUDL be in chuck " + idx);

        //

    }

    // public float DistanceToChunk() {
    //     int[] chucksToCheck = { currentChunk + MAP_DIMENSIONS.Y,
    //                             currentChunk - MAP_DIMENSIONS.Y,
    //                             currentChunk + 1,
    //                             currentChunk - 1
    //     };

    //     foreach(int )
    // }

    // function distance(rect, p) {
    //     var dx = Math.max(rect.min.x - p.x, 0, p.x - rect.max.x);
    //     var dy = Math.max(rect.min.y - p.y, 0, p.y - rect.max.y);
    //     return Math.sqrt(dx*dx + dy*dy);
    // }
}
