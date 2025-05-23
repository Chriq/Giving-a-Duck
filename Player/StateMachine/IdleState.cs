using Godot;
using System;

public partial class IdleState : State {
    public override void Enter() {
        rb.Velocity = Vector2.Zero;
    }

}
