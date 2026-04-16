using UnityEngine;

public abstract class State
{
    protected FSMManager fsm;
    protected Vehicle steering;
    protected AIEye _AIEye;
    public State(FSMManager fsm)
    {
        this.fsm = fsm;
        this.steering = fsm.GetComponent<Vehicle>();
        this._AIEye = fsm.GetComponent<AIEye>();
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}