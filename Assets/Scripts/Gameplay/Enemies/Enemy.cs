using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using TMPro;



public class Enemy : EnemyBase {

    public MovementPoint currentPoint;
    public MovementPoint targetPoint;




    public void Init(EnemyType eType, MovementPoint point) {
        BaseInit(eType);
         
        currentPoint = point;
        currentPoint.isFree = false;
        currentPoint.canDrop = false;
        currentPoint.data = PointData.Enemy;

        target = currentPoint.gameObject.GetComponent<Transform>();

        SetAttackPoints();
        SetMovePoints();
    }


    public override void SetAttackPoints() {
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

    public override void SetMovePoints() {
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



    public override void ActivateMove() {
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

    public override void EndMove() {
        currentPoint.Reset();
        currentPoint = targetPoint;
        currentPoint.isFree = false;
        currentPoint.canDrop = false;
        currentPoint.data = PointData.Enemy;
        SetAttackPoints();
        SetMovePoints();
        Attack();
    }





    public override void GetDamageByPlayer(int damage, QBitType qType) {
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





    public override Coordinate FindShortestWayToPoint(Coordinate point) {
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
}
