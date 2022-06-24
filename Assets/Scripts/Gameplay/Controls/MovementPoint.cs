using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Spine.Unity;


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

    public bool isEscapeFromLevel;

    public bool isObstacle => data == PointData.Obstacle;
    public bool isDestroyable => data == PointData.Destroyable;
    public bool isQbit => data == PointData.QBit;
    public bool isDefect => data == PointData.Defect;
    public bool isEnemy => data == PointData.Enemy;
    public bool isBigEnemy => data == PointData.BigEnemy;
    public bool isPlayer => data == PointData.Player;
    public bool isQuant => data == PointData.Quant;

    public PointData data;

    public DebuffType debuff;

    public List<MovementDirectionData> directionIndicators;

    public class PointReachedEvent : UnityEvent { }
    [HideInInspector] public PointReachedEvent onPointReach = new PointReachedEvent();

    public bool canEditMovementPath;

    public Button pointBtn;

    public TMP_Text powerRemain;

    
    void Awake() {
        Deactivate();
        Reset();
        ResetIndicators();

        canEditMovementPath = false;
    }

    void Start() {
        pointBtn.onClick.AddListener(ShowInfo);
        powerRemain.text = "";
    }

    
    public void Activate() {
        isActive = true;
        frame.color = active;

        if(isQbit)
            Field.Instance.qBits.Find(q => q.x == this.x && q.y == this.y).SetActive(true);
        else if(isQuant)
            Field.Instance.quantsItems.Find(q => q.x == this.x && q.y == this.y).SetActive(true);
        else if(isEnemy)
            Field.Instance.enemiesItems.Find(e => e.currentPoint.x == this.x && e.currentPoint.y == this.y).SetActive(true);
    }

    public void Deactivate() {
        isActive = false;
        frame.color = passive;

        if(isQbit)
            Field.Instance.qBits.Find(q => q.x == this.x && q.y == this.y).SetActive(false);
        else if(isQuant)
            Field.Instance.quantsItems.Find(q => q.x == this.x && q.y == this.y).SetActive(false);
        else if(isEnemy)
            Field.Instance.enemiesItems.Find(e => e.currentPoint.x == this.x && e.currentPoint.y == this.y).SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(isEscapeFromLevel)
                Field.Instance.MoveToNextLevel();

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
                else if(isQuant) {
                    Quant quantToDestroy = Field.Instance.quantsItems.Find(q => q.x == this.x && q.y == this.y);
                    quantToDestroy.DestroyQuant();
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

    public void ActivateIndicator(MovementDirection direction, QBitType qType) {
        ResetIndicators();
        MovementDirectionData di = directionIndicators.Find(i => i.direction == direction);
        ChangeIndicatorsColor(qType);
        di.indicator.SetActive(true);
    }


    public void ResetDebuff() {
        debuff = DebuffType.None;
    }

    public void ChangeIndicatorsColor(QBitType qType) {
        foreach(var data in directionIndicators) {
            SkeletonGraphic sg = data.indicator.GetComponent<SkeletonGraphic>();
            switch(qType) {
                case QBitType.GREEN:
                    sg.Skeleton.SetSkin("green");
                    break;
                case QBitType.BLUE:
                    sg.Skeleton.SetSkin("blue");
                    break;
                case QBitType.RED:
                    sg.Skeleton.SetSkin("red");
                    break;
                case QBitType.NONE:
                    sg.Skeleton.SetSkin("white");
                    break;
            }
            sg.Skeleton.SetSlotsToSetupPose();
            sg.LateUpdate();
        }
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
        Player.Instance.input.UpdateEditableMovementPoint();
    }



    private void ShowInfo() {
        if(isEnemy) {
            Enemy en = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == this.x && e.currentPoint.y == this.y);
            en.ShowAttackRadius();
        }
    }


    public void SetPowerRemainCaption(int pr) {
        powerRemain.text = pr > 0 ? pr.ToString() : "";
    }
}


public enum PointData {None, Obstacle, Destroyable, Enemy, Defect, QBit, Player, BigEnemy, Quant}

public enum MovementDirection {Up, Down, Right, Left, Up_Right, Up_Left, Down_Right, Down_Left, None}

public enum DebuffType {None, Puddle}


[System.Serializable]
public class MovementDirectionData {
    public MovementDirection direction;
    public GameObject indicator;
}
