using Godot;
using System;

public partial class Beacon : Area2D {
    bool discovered = false;

    public override void _Ready() {
        BodyEntered += Discover;
    }

    public void Discover(Node n) {
        if (n is PlayerController) {
            discovered = true;
            GD.Print("Beacon Discovered!");
        }
    }

}
