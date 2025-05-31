using Godot;
using System.Collections.Generic;

public partial class PlayerInfo {
    public int id;
    public string name;
    public List<Item> items = new();
    public Node playerObject = null;

    public PlayerInfo(int id) {
        this.id = id;
    }
}
