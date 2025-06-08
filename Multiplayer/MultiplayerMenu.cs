using Godot;
using System;

public partial class MultiplayerMenu : Control {
    [Export] TextEdit nameInput;
    [Export] Button host;
    [Export] Button join;
    [Export] Button start;
    [Export] TextEdit ipInput;
    [Export] Label errorLabel;

    public override void _Ready() {
        base._Ready();
        host.Pressed += Host;
        join.Pressed += Join;
        start.Pressed += Start;

        start.Disabled = true;
    }

    private void Host() {
        host.Disabled = true;
        if (MultiplayerController.Instance.Host()) {
            host.Disabled = true;
            join.Disabled = true;
            ipInput.Clear();
            ipInput.Editable = false;
            errorLabel.Text = "Host Created!";
            start.Disabled = false;
        } else {
            host.Disabled = false;
            errorLabel.Text = "Error, couldn't create host! Shut down any prior instances and retry.";
        }
    }

    private void Join() {
        join.Disabled = true;
        host.Disabled = true;
        if (!MultiplayerController.Instance.Join()) {
            host.Disabled = false;
            join.Disabled = false;
            errorLabel.Text = "Error, couldn't join game! Make sure you have the correct IP and your host set up Port Forwarding on port 38257";
        } else if (string.IsNullOrEmpty(ipInput.Text)) {
            host.Disabled = false;
            join.Disabled = false;
            errorLabel.Text = "Joining your localhost is for development testing only. Host a server, or join your friends'!";
        }
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

    public string GetIP() {
        return ipInput.Text;
    }

    public void OnJoinConnected() {
        errorLabel.Text = "Connected to Host!";
        start.Disabled = false;
    }
}