using Godot;
using System;

public partial class StateMachine {
    public State state;

    public void Set(State newState, bool forceReset = false) {
        if (state != newState || forceReset) {
            state?.Exit();
            state = newState;
            state.Initialize();
            state.Enter();
        }
    }
}
