using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public MovementPoint currentPoint;

    public static PlayerController Instance { get; private set; }

    
    void Awake() {
        Instance = this;
    }


    void Update() {}
}
