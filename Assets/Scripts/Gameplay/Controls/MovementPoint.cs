using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class MovementPoint : MonoBehaviour, IDragHandler, IEndDragHandler {

    public int x;
    public int y;

    public Image frame;
    public Color active;
    public Color passive;
    public Color attacked;

    public bool isActive;

    public bool isFree;
    public bool canDrop;

    public bool isObstacle => data == PointData.Obstacle;
    public bool isDestroyable => data == PointData.Destroyable;
    public bool isQbit => data == PointData.QBit;
    public bool isDefect => data == PointData.Defect;
    public bool isEnemy => data == PointData.Enemy;
    public bool isBigEnemy => data == PointData.BigEnemy;
    public bool isPlayer => data == PointData.Player;

    public PointData data;

    public DebuffType debuff;

    public List<MovementDirectionData> directionIndicators;

    public class PointReachedEvent : UnityEvent { }
    [HideInInspector] public PointReachedEvent onPointReach = new PointReachedEvent();

    public bool canEditMovementPath;

    
    void Awake() {
        Deactivate();
        Reset();
        ResetIndicators();

        canEditMovementPath = false;
    }

    
    public void Activate() {
        isActive = true;
        frame.color = active;

        if(isQbit)
            Field.Instance.qBits.Find(q => q.x == this.x && q.y == this.y).SetActive(true);
    }

    public void Deactivate() {
        isActive = false;
        frame.color = passive;

        if(isQbit)
            Field.Instance.qBits.Find(q => q.x == this.x && q.y == this.y).SetActive(false);
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
                if(isQbit) {
                    QBit qBitToDestroy = Field.Instance.qBits.Find(q => q.x == this.x && q.y == this.y);
                    qBitToDestroy.DestroyQbit();
                }
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





    public void ResetIndicators() {
        foreach(var ind in directionIndicators)
            ind.indicator.SetActive(false);
    }

    public void ActivateIndicator(MovementDirection direction) {
        ResetIndicators();
        MovementDirectionData di = directionIndicators.Find(i => i.direction == direction);
        di.indicator.SetActive(true);
    }


    public void ResetDebuff() {
        debuff = DebuffType.None;
    }





    public void SetAttackedState() {
        StartCoroutine(ActivateAttackedState());
    }
    
    public IEnumerator ActivateAttackedState() {
        frame.color = attacked;

        yield return new WaitForSeconds(1.5f);

        frame.color = passive;
    }




    public void OnDrag(PointerEventData eventData) {
        if(!canEditMovementPath)
            return;
        
        bool isPointerOverUI = Player.Instance.input.IsPointerOverUIElement();
    }

    public void OnEndDrag(PointerEventData eventData) {
        if(!canEditMovementPath)
            return;

        Player.Instance.input.UpdateMovementPathData();
    }
}


public enum PointData {None, Obstacle, Destroyable, Enemy, Defect, QBit, Player, BigEnemy}

public enum MovementDirection {Up, Down, Right, Left, Up_Right, Up_Left, Down_Right, Down_Left}

public enum DebuffType {None, Puddle}


[System.Serializable]
public class MovementDirectionData {
    public MovementDirection direction;
    public GameObject indicator;
} 
