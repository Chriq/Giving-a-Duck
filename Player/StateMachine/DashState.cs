using Godot;
using System;

public partial class DashState : State {
    [Export] private float dashSpeed = 700f;

    private double cooldown = 0;

    public override void Enter() {
        GD.Print("Dash");
        float dir = Input.GetAxis("Left", "Right");
        rb.Velocity = new Vector2(dashSpeed * dir, 0);
    }

    public override void PhysicsDo(double delta) {
        cooldown += delta;

        if (rb.Velocity.X < 1f) {
            complete = true;
        }
    }

}
