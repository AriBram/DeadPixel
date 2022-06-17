using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using TMPro;


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
    public Image shield;
    public Image heart;

    public bool hasShield;
    public QBitData colorData;

    public class MoveEndEvent : UnityEvent { }
    [HideInInspector] public MoveEndEvent onMoveEnd = new MoveEndEvent();



    void Start() {
        rb = GetComponent<Rigidbody2D>();
        canMove = true;

        healthPoints = Random.Range(minHP, maxHP + 1);
        hpCaption.text = healthPoints.ToString();

        hasShield = Random.Range(0, 2) == 0 ? true : false;
        shield.gameObject.SetActive(hasShield);

        colorData = Field.Instance.GetRandomColor();
        heart.color = colorData.color;
        shield.color = colorData.color;
    }



    void FixedUpdate() {
        if(GameplayController.Instance.IsEnemyMove)
            Move();
    }



    public void Move() {
        Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(temp);
    }






    public void BaseInit(EnemyType eType) {
        this.eType = eType;
        healthPoints = Random.Range(minHP, maxHP + 1);
    }


    public abstract void SetAttackPoints();

    public abstract void SetMovePoints();



    public abstract void ActivateMove();

    public abstract void EndMove();



    public virtual void Attack() {
        foreach(var ap in attackPoints) {
            MovementPoint mp = MovementManager.Instance.Points.Find(p => p.x == ap.x && p.y == ap.y);
            if(mp != null)
                mp.SetAttackedState();
        }

        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        bool isPlayerInAttackRadius = attackPoints.Find(ap => ap.x == playerPoint.x && ap.y == playerPoint.y) == null ? false : true;
        QBitType playerQType = Player.Instance.colorType;

        if(isPlayerInAttackRadius && colorData.qType != playerQType)
            Player.Instance.health.GetDamage(attackPower);

        onMoveEnd.Invoke();
    }



    public abstract void GetDamageByPlayer(int damage, QBitType qType);




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
}
