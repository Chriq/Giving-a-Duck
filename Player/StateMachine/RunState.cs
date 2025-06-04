using Godot;
using System;

public partial class RunState : State {
    [Export] float speed = 120;
    [Export] float acceleration = 0.5f;
    [Export] float friction = 0.2f;

    public override void PhysicsDo(double delta) {
        float dir = Input.GetAxis("Left", "Right");

        if (dir != 0) {
            rb.Velocity = new Vector2(Mathf.Lerp(rb.Velocity.X, dir * speed, acceleration), rb.Velocity.Y);
        } else {
            rb.Velocity = new Vector2(Mathf.Lerp(rb.Velocity.X, 0f, friction), rb.Velocity.Y);
        }

        if (Mathf.Abs(rb.Velocity.X) < 25f) {
            rb.Velocity = new Vector2(0f, rb.Velocity.Y);
            complete = true;
        }
    }

    public override void Enter() {
        animator.Play("run");
    }

}
