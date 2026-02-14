using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine {
    public EnemyState currentState;
    // Start is called before the first frame update

    public void initialize(EnemyState _startState) {
        currentState = _startState;
        currentState.Enter();
    }
    public void ChangeState(EnemyState _newState) {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
