using System.Collections.Generic;

public partial class PlayerInfo {
    public int id;
    public string name;
    public List<Item> items = new();

    public PlayerInfo(int id, string name) {
        this.id = id;
        this.name = name;
    }
}
