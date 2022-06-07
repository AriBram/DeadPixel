using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UserData : MonoBehaviour {
    
    public int currentLevel;

    public static UserData Instance { get; private set; }


    void Awake() {
        Instance = this;
        currentLevel = 0;
    }
}
