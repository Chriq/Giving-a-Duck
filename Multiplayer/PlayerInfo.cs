using Godot;
using System;

public partial class PlayerInfo : Node {
    public int id;
    public string name;

    public PlayerInfo(int id, string name) {
        this.id = id;
        this.name = name;
    }
}
