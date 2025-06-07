using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerController : CharacterBody2D {
    public int playerId;
    public PlayerInfo info => GameManager.Instance.players.GetValueOrDefault(playerId);

    [Export] float speed = 120;
    [Export] float jumpSpeed = -180;
    [Export] float gravity = 400;
    [Export] float friction = 0.2f;
    [Export] float acceleration = 0.5f;

    [Export] MultiplayerSynchronizer sync;

    /** State Machine **/
    private StateMachine machine;
    [Export] RunState runState;
    [Export] IdleState idleState;
    [Export] AirState airState;
    [Export] WallState wallState;
    [Export] DashState dashState;

    [Export] AnimatedSprite2D sprite;

    [Export] Label playerLabel;

    private int jumps = 0;


    public override void _Ready() {
        sync.SetMultiplayerAuthority(playerId);

        idleState.Setup(this, sprite);
        runState.Setup(this, sprite);
        airState.Setup(this, sprite);
        wallState.Setup(this, sprite);
        dashState.Setup(this, sprite);

        machine = new();
        machine.Set(idleState);

        if (sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
            MapManager.Instance.UpdateActive(GlobalPosition);

            Camera2D cam = new Camera2D();
            //cam.Zoom = new Vector2(0.1f, 0.1f);
            AddChild(cam);
        }

        playerLabel.Text = !string.IsNullOrEmpty(info.name) ? info.name : "Duck " + playerId;

        Multiplayer.PeerDisconnected += DisconnectPlayer;
    }

    public void SelectState(float axis) {
        if (Input.IsActionJustPressed("Dash") && info.items.Contains(Item.DASH)) {
            machine.Set(dashState);
        }

        if (IsOnFloor()) {
            if (axis == 0 && machine.state.complete) {
                machine.Set(idleState);
            } else {
                machine.Set(runState);
            }
        } else if (IsOnWall() && (info.items.Contains(Item.WALL_JUMP) || info.items.Contains(Item.CLIMB))) {
            machine.Set(wallState);
        } else {
            machine.Set(airState);
        }
    }


    public override void _PhysicsProcess(double delta) {
        if (sync.GetMultiplayerAuthority() == Multiplayer.GetUniqueId()) {
            HandleJump();
            Move((float)delta);
            MapManager.Instance.UpdateActive(GlobalPosition);
        }
    }

    private void Move(float delta) {
        Velocity += new Vector2(0f, gravity * delta);

        float input = Input.GetAxis("Left", "Right");
        sprite.FlipH = input < 0f || (input == 0f && sprite.FlipH);

        SelectState(input);
        machine.state.PhysicsDo(delta);

        MoveAndSlide();
    }

    private void HandleJump() {
        if (Input.IsActionJustPressed("Jump")) {
            if (IsOnFloor()) {
                jumps = 0;
            }

            if (jumps < 1 || info.items.Contains(Item.DOUBLE_JUMP) && jumps < 2 && !IsOnWall()) {
                Velocity = new Vector2(Velocity.X, jumpSpeed);
                jumps++;
            }
        }
    }

    public bool HasItem(Item item) {
        return info.items.Contains(item);
    }

    private void DisconnectPlayer(long id) {
        if (id == playerId) {
            GameManager.Instance.players.Remove(playerId);
            QueueFree();
        }
    }
}
