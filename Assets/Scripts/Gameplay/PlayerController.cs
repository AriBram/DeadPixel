using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public MovementPoint currentPoint;

    public static PlayerController Instance { get; private set; }

    
    void Start() {
        Instance = this;
    }


    void Update() {
        
    }
}
