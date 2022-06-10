using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using TMPro;



public class Enemy : MonoBehaviour {
    
    public EnemyType eType;

    [HideInInspector] public int healthPoints;
    public int minHP;
    public int maxHP;
    public int attackPower;

    public MovementPoint currentPoint;
    public MovementPoint targetPoint;

    public List<Coordinate> attackPoints;
    public List<Coordinate> availablePointsToMove;

    private Transform target;
    private Rigidbody2D rb;

    public float moveSpeed;

    public bool canMove;
    public GameObject canMoveIndicator;

    public TMP_Text hpCaption;
    public Image shield;
    public Image heart;
    public List<GameObject> skulls;

    public bool hasShield;
    public QBitData colorData;

    public class MoveEndEvent : UnityEvent { }
    [HideInInspector] public MoveEndEvent onMoveEnd = new MoveEndEvent();

    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        target = currentPoint.gameObject.GetComponent<Transform>();
        canMove = true;

        healthPoints = Random.Range(minHP, maxHP + 1);
        hpCaption.text = healthPoints.ToString();

        hasShield = Random.Range(0, 2) == 0 ? true : false;
        shield.gameObject.SetActive(hasShield);

        colorData = Field.Instance.GetRandomColor();
        heart.color = colorData.color;
        shield.color = colorData.color;

        foreach(var skull in skulls)
            skull.SetActive(false);
        
        for(int i = 0; i < attackPower; i++)
            skulls[i].SetActive(true);
    }

    void FixedUpdate() {
        if(GameplayController.Instance.IsEnemyMove)
            Move();
    }



    public void Move() {
        Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(temp);
    }





    public void Init(EnemyType eType, MovementPoint point) {
        this.eType = eType;
         
        currentPoint = point;
        currentPoint.isFree = false;
        currentPoint.canDrop = false;

        healthPoints = Random.Range(minHP, maxHP + 1);

        SetAttackPoints();
        SetMovePoints();
    }


    void SetAttackPoints() {
        attackPoints = new List<Coordinate>();
        switch(eType) {
            case EnemyType.Worm: case EnemyType.Skeleton: case EnemyType.Zombie:
                attackPoints.Add(new Coordinate(currentPoint.x + 1, currentPoint.y));
                attackPoints.Add(new Coordinate(currentPoint.x - 1, currentPoint.y));
                attackPoints.Add(new Coordinate(currentPoint.x, currentPoint.y + 1));
                attackPoints.Add(new Coordinate(currentPoint.x, currentPoint.y - 1));
                break;
            case EnemyType.Agent:
                List<MovementPoint> points = MovementManager.Instance.Points.FindAll(p => p.x == currentPoint.x || p.y == currentPoint.y);
                foreach(var p in points) {
                    if(p.x == currentPoint.x && p.y == currentPoint.y)
                        continue;
                    attackPoints.Add(new Coordinate(p.x, p.y));
                }
                break;
        }
    }

    void SetMovePoints() {
        availablePointsToMove = new List<Coordinate>();
        switch(eType) {
            case EnemyType.Worm: case EnemyType.Skeleton: case EnemyType.Agent:
                availablePointsToMove.Add(new Coordinate(currentPoint.x + 1, currentPoint.y));
                availablePointsToMove.Add(new Coordinate(currentPoint.x - 1, currentPoint.y));
                availablePointsToMove.Add(new Coordinate(currentPoint.x, currentPoint.y + 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x, currentPoint.y - 1));

                availablePointsToMove.Add(new Coordinate(currentPoint.x + 1, currentPoint.y + 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x - 1, currentPoint.y + 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x + 1, currentPoint.y - 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x - 1, currentPoint.y - 1));

                break;

            case EnemyType.Zombie:
                availablePointsToMove.Add(new Coordinate(currentPoint.x + 1, currentPoint.y));
                availablePointsToMove.Add(new Coordinate(currentPoint.x - 1, currentPoint.y));
                availablePointsToMove.Add(new Coordinate(currentPoint.x, currentPoint.y + 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x, currentPoint.y - 1));

                break;
        }
    }



    public void ActivateMove() {
        if(!canMove)
            return;

        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        Coordinate playerCoordinate = new Coordinate(playerPoint.x, playerPoint.y);
        bool isPlayerInAttackRadius = IsPointInAttackRadius(playerCoordinate);
        Coordinate movePoint = new Coordinate(currentPoint.x, currentPoint.y);
        if(!isPlayerInAttackRadius)
            movePoint = FindShortestWayToPoint(playerCoordinate);
        
        MovementPoint p = MovementManager.Instance.Points.Find(p => p.x == movePoint.x && p.y == movePoint.y);
        target = p.gameObject.GetComponent<Transform>();
        targetPoint = p;

        if(targetPoint.x == currentPoint.x && targetPoint.y == currentPoint.y)
            EndMove();
    }

    public void EndMove() {
        currentPoint.Reset();
        currentPoint = targetPoint;
        currentPoint.isFree = false;
        currentPoint.canDrop = false;
        currentPoint.data = PointData.Enemy;
        SetAttackPoints();
        SetMovePoints();
        Attack();
    }



    public void Attack() {
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



    public void GetDamageByPlayer(int damage, QBitType qType) {
        if(hasShield && qType == colorData.qType)
            return;

        healthPoints -= damage;
        if(healthPoints <= 0) {
            MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == currentPoint.x && p.y == currentPoint.y);
            point.Reset();
            Field.Instance.enemiesItems.Remove(this);
            Field.Instance.deathsCounter[eType] += 1;
            Player.Instance.enemiesKilled[eType] += 1;
            Destroy(this.gameObject);
        }

        hpCaption.text = healthPoints.ToString();
    }




    public bool IsPointInAttackRadius(Coordinate point) {
        foreach(var p in attackPoints) {
            if(point.x == p.x && point.y == p.y)
                return true;
        }

        return false;
    }

    public Coordinate FindShortestWayToPoint(Coordinate point) {
        Coordinate shortestWayPoint = new Coordinate(currentPoint.x, currentPoint.y);
        int currentDistance = CountDistanceBetweenPoints(point, shortestWayPoint);

        foreach(var p in availablePointsToMove) {
            if(EnemiesMovementManager.Instance.IsPointBusy(p))
                continue;

            MovementPoint movementPoint = MovementManager.Instance.Points.Find(pp => pp.x == p.x && pp.y == p.y);
            if(movementPoint == null)
                continue;

            if(movementPoint.isObstacle || movementPoint.isDestroyable || movementPoint.isEnemy || movementPoint.isDefect || movementPoint.isPlayer)
                continue;

            int newDistance = CountDistanceBetweenPoints(point, p);
            if(newDistance < currentDistance) {
                shortestWayPoint = p;
                currentDistance = newDistance;
            }
        }

        EnemiesMovementManager.Instance.AddBusyPoint(shortestWayPoint);

        return shortestWayPoint;
    }

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
