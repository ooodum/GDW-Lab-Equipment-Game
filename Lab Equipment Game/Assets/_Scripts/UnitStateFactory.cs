using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateFactory { 
    Dictionary<State, UnitState> states = new Dictionary<State, UnitState>();
    Troop troop;
    enum State {
        Idle,
        Chase,
        Attack,
        Die
    }

    public UnitStateFactory(Troop troop) { 
        this.troop = troop;

        states[State.Idle] = new UnitIdleState();
    }

    public UnitState Idle() {
        return states[State.Idle];
    }
}
