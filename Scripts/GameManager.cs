using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node {
    public static GameManager Instance;
    public Dictionary<long, PlayerInfo> players = new Dictionary<long, PlayerInfo>();

    public int playerAuthority = 1;

    public override void _Ready() {
        Instance = this;
    }
}