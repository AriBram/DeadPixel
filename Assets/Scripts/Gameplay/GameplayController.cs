using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameplayState {PREPARE, MOVE}


public class GameplayController : MonoBehaviour {

    public GameplayState State;

    public static GameplayController Instance { get; private set; }

    
    void Start() {
        Instance = this;
    }

    
    void Update() {
        
    }
}
