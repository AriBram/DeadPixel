using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public enum EnemyType {Worm, Skeleton, Zombie, Agent, Spider}

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

    public class MoveEndEvent : UnityEvent { }
    [HideInInspector] public MoveEndEvent onMoveEnd = new MoveEndEvent();

    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        target = currentPoint.gameObject.GetComponent<Transform>();
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
        onMoveEnd.Invoke();
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
}
