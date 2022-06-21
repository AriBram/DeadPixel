using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using TMPro;
using Spine.Unity;


public abstract class EnemyBase : MonoBehaviour {
    public EnemyType eType;

    [HideInInspector] public int healthPoints;
    public int minHP;
    public int maxHP;
    public int attackPower;

    public List<Coordinate> attackPoints;
    public List<Coordinate> availablePointsToMove;

    public Transform target;
    public Rigidbody2D rb;

    public float moveSpeed;

    public bool canMove;
    public GameObject canMoveIndicator;

    public TMP_Text hpCaption;
    public TMP_Text powerRemain;
    public Image shield;
    public Image heart;

    public bool hasShield;
    public QBitData colorData;

    public SkeletonGraphic activateIndicator;

    public class MoveEndEvent : UnityEvent { }
    [HideInInspector] public MoveEndEvent onMoveEnd = new MoveEndEvent();



    void Start() {
        rb = GetComponent<Rigidbody2D>();
        canMove = true;

        activateIndicator.Initialize(true);
        activateIndicator.AnimationState.SetAnimation(0, "fear", true);
    }



    void FixedUpdate() {
        if(GameplayController.Instance.IsEnemyMove)
            Move();
    }



    public void Move() {
        Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(temp);
    }




    public abstract void SetAttackPoints();

    public abstract void SetMovePoints();


    public virtual void SetActive(bool isActive) {
        switch(colorData.qType) {
            case QBitType.GREEN:
                activateIndicator.Skeleton.SetSkin("green");
                break;
            case QBitType.BLUE:
                activateIndicator.Skeleton.SetSkin("blue");
                break;
            case QBitType.RED:
                activateIndicator.Skeleton.SetSkin("red");
                break;
        }

        activateIndicator.Skeleton.SetSlotsToSetupPose();
        activateIndicator.LateUpdate();

        activateIndicator.gameObject.SetActive(isActive);
    }



    public abstract void ActivateMove();

    public abstract void EndMove();



    public virtual void Attack() {
        ShowAttackRadius();

        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        bool isPlayerInAttackRadius = attackPoints.Find(ap => ap.x == playerPoint.x && ap.y == playerPoint.y) == null ? false : true;
        QBitType playerQType = Player.Instance.colorType;

        if(isPlayerInAttackRadius && colorData.qType != playerQType)
            Player.Instance.health.GetDamage(attackPower);

        onMoveEnd.Invoke();
    }



    public abstract int GetDamageByPlayer(int damage, QBitType qType);



    public void ShowAttackRadius() {
        foreach(var ap in attackPoints) {
            MovementPoint mp = MovementManager.Instance.Points.Find(p => p.x == ap.x && p.y == ap.y);
            if(mp != null)
                mp.SetAttackedState();
        }
    }
    
    public bool IsPointInAttackRadius(Coordinate point) {
        foreach(var p in attackPoints) {
            if(point.x == p.x && point.y == p.y)
                return true;
        }

        return false;
    }

    public abstract Coordinate FindShortestWayToPoint(Coordinate point);

    public int CountDistanceBetweenPoints(Coordinate point1, Coordinate point2) {
        int distance = 0;
        distance += Mathf.Abs(point2.x - point1.x) + Mathf.Abs(point2.y - point1.y);
        return distance;
    }



    public void SetCanMove(bool canMove) {
        this.canMove = canMove;
        canMoveIndicator.SetActive(canMove);
    }


    public void RefreshHpCaption() {
        hpCaption.text = healthPoints.ToString();
    }

    public void SetPowerRemainCaption(int pr) {
        powerRemain.text = pr > 0 ? pr.ToString() : "";
    }
}
