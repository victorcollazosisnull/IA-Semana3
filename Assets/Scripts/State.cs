using UnityEngine;

public abstract class State
{
    protected FSMManager fsm;
    protected Vehicle steering;

    public State(FSMManager fsm)
    {
        this.fsm = fsm;
        this.steering = fsm.GetComponent<Vehicle>();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}