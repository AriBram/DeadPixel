using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameplayState {IDLE, PREPARE, MOVE}


public class GameplayController : MonoBehaviour {

    public GameplayState State;

    public bool IsPrepare => State == GameplayState.PREPARE;
    public bool IsIdle => State == GameplayState.IDLE;
    public bool IsMove => State == GameplayState.MOVE;

    public static GameplayController Instance { get; private set; }

    
    void Start() {
        Instance = this;
    }

    
    void Update() {
        
    }


    public void SetPrepareState() {
        State = GameplayState.PREPARE;
    }

    public void SetIdleState() {
        State = GameplayState.IDLE;
    }

    public void SetMoveState() {
        State = GameplayState.MOVE;
    }
}
