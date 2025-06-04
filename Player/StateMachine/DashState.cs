using Godot;
using System;

public partial class DashState : State {
    [Export] private float dashSpeed = 1000f;

    private double cooldown = 0;

    public override void Enter() {
        float dir = Input.GetAxis("Left", "Right");
        rb.Velocity = new Vector2(dashSpeed * dir, 0);
        animator.Play("fly");
    }

    public override void PhysicsDo(double delta) {
        cooldown += delta;

        if (rb.Velocity.X < 1f) {
            complete = true;
        }
    }

}
