using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hier wird die Logik fuer Spieler implementiert
public class PlayerStateMachine : MonoBehaviour, IUpdateObserver
{
    //state Variablen
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    //variables from PlayerController
    private Rigidbody _rigidbody;
    [SerializeField] private float _jumpForce = 3f;

    //getters and setters
    public PlayerBaseState CurrentState {get {return _currentState;} set { _currentState = value; } }
    //also getters and setters for all variables from PlayerController
    public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }
    public Rigidbody MyRigidbody { get { return _rigidbody; } set { _rigidbody = value; } }


    public void Awake()
    {
        //setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle();
        //erfuellen Aktionen beim Beitretten im Zustand
        _currentState.EnterState();
    }

    public void ObservedUpdate()
    {
        _currentState.UpdateState();
    }
}
