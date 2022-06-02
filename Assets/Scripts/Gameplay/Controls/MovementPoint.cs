using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class MovementPoint : MonoBehaviour {

    public int x;
    public int y;

    public Image frame;
    public Color active;
    public Color passive;

    public bool isActive;

    public bool isFree;

    public class PointReachedEvent : UnityEvent { }
    [HideInInspector] public PointReachedEvent onPointReach = new PointReachedEvent();

    
    void Awake() {
        isFree = true;
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


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(GameplayController.Instance.IsMove) {
                Deactivate();
                onPointReach.Invoke();
            }
        }
    }
}
