using Godot;
using System;

public partial class EndMenu : Node {
    [Export] Button button;

    public override void _Ready() {
        button.Pressed += ToMainMenu;
    }

    public void ToMainMenu() {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
}
