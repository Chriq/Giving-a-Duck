using Godot;
using System;

public partial class MultiplayerMenu : Control {
    [Export] TextEdit nameInput;
    [Export] Button host;
    [Export] Button join;
    [Export] Button start;

    public override void _Ready() {
        base._Ready();
        host.Pressed += Host;
        join.Pressed += Join;
        start.Pressed += Start;
        //nameInput.TextChanged += UpdateName;
    }

    private void Host() {
        MultiplayerController.Instance.Host();
    }

    private void Join() {
        MultiplayerController.Instance.Join();
    }

    private void Start() {
        MultiplayerController.Instance.Start();
    }

    private void UpdateName() {
        Multiplayer.MultiplayerPeer.SetMeta("PlayerName", nameInput.Text);
    }

    public string GetPlayerName() {
        return nameInput.Text;
    }
}