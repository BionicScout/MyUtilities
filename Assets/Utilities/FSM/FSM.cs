public class FSM {
    public FSM_State currentState = null;

    public FSM(FSM_State startState) {
        currentState = startState;
        currentState.OnEnter();
    }

    public void Update() {
        currentState.Update();
    }

    public void PhysicsUpdate() {
        currentState.PhysicsUpdate();
    }

    public void LateUpdate() {
        currentState.LateUpdate();
    }

    public void ChangeStates(FSM_State newState) {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
}
