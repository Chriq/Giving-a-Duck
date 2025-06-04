using Godot;
using System;

public abstract partial class State : Node {
    public bool complete { get; protected set; }

    double startTime;
    double time => Time.GetUnixTimeFromSystem() - startTime;

    // Player Components
    protected PlayerController rb;
    protected AnimatedSprite2D animator;

    public virtual void Enter() { }
    public virtual void Do(double delta) { }
    public virtual void PhysicsDo(double delta) { }
    public virtual void Exit() { }

    public void Setup(PlayerController character, AnimatedSprite2D anim) {
        rb = character;
        animator = anim;
    }

    public void Initialize() {
        complete = false;
        startTime = Time.GetUnixTimeFromSystem();
    }
}
