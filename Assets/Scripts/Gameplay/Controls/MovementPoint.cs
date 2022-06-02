using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MovementPoint : MonoBehaviour {

    public int x;
    public int y;

    public Image frame;
    public Color active;
    public Color passive;

    public bool isActive;

    
    void Start() {
        Deactivate();
    }

    
    public void Activate() {
        isActive = true;
        frame.color = active;
    }

    public void Deactivate() {
        isActive = false;
        frame.color = passive;
    }
}
