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
    public bool canDrop;

    public bool isObstacle => data == PointData.Obstacle;
    public bool isDestroyable => data == PointData.Destroyable;
    public bool isQbit => data == PointData.QBit;
    public bool isDefect => data == PointData.Defect;
    public bool isEnemy => data == PointData.Enemy;
    public bool isPlayer => data == PointData.Player;

    public PointData data;

    public class PointReachedEvent : UnityEvent { }
    [HideInInspector] public PointReachedEvent onPointReach = new PointReachedEvent();

    
    void Awake() {
        Deactivate();
        Reset();
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

        else if(other.CompareTag("Enemy")) {
            if(GameplayController.Instance.IsEnemyMove) {
                Enemy e = other.gameObject.GetComponent<Enemy>();
                if(e.targetPoint.x == this.x && e.targetPoint.y == this.y)
                    e.EndMove();
            }
        }
    }


    public void Reset() {
        isFree = true;
        canDrop = true;
        data = PointData.None;
    }
}


public enum PointData {None, Obstacle, Destroyable, Enemy, Defect, QBit, Player}
