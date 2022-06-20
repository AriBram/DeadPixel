using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : EnemyBase {

    public Point_x4 currentPoint;
    public Point_x4 targetPoint;



    
    public void Init(EnemyType eType, Point_x4 point) {
        this.eType = eType;
        
        healthPoints = Random.Range(minHP, maxHP + 1);
        hpCaption.text = healthPoints.ToString();

        hasShield = Random.Range(0, 2) == 0 ? true : false;
        shield.gameObject.SetActive(hasShield);

        colorData = Field.Instance.GetRandomColor();
        heart.color = colorData.color;
        shield.color = colorData.color;
         
        currentPoint = point;
        foreach(var p in point.points) {
            p.isFree = false;
            p.canDrop = false;
            p.data = PointData.BigEnemy;
        }
        
        target = currentPoint.gameObject.GetComponent<Transform>();

        SetAttackPoints();
        SetMovePoints();
    }


    public override void SetAttackPoints() {
        attackPoints = new List<Coordinate>();
        switch(eType) {
            case EnemyType.Spider:
                attackPoints.Add(new Coordinate(currentPoint.points[0].x - 1, currentPoint.points[0].y));
                attackPoints.Add(new Coordinate(currentPoint.points[1].x + 1, currentPoint.points[1].y));
                attackPoints.Add(new Coordinate(currentPoint.points[2].x - 1, currentPoint.points[2].y));
                attackPoints.Add(new Coordinate(currentPoint.points[3].x + 1, currentPoint.points[3].y));
                break;
        }
    }

    public override void SetMovePoints() {
        availablePointsToMove = new List<Coordinate>();
        switch(eType) {
            case EnemyType.Spider:
                availablePointsToMove.Add(new Coordinate(currentPoint.x4 + 1, currentPoint.y4 + 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x4 - 1, currentPoint.y4 + 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x4 + 1, currentPoint.y4 - 1));
                availablePointsToMove.Add(new Coordinate(currentPoint.x4 - 1, currentPoint.y4 - 1));

                break;
        }
    }



    public override void ActivateMove() {
        if(!canMove)
            return;

        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        Coordinate playerCoordinate = new Coordinate(playerPoint.x, playerPoint.y);
        bool isPlayerInAttackRadius = IsPointInAttackRadius(playerCoordinate);
        Coordinate movePoint = new Coordinate(currentPoint.x4, currentPoint.y4);
        if(!isPlayerInAttackRadius)
            movePoint = FindShortestWayToPoint(playerCoordinate);
        
        Point_x4 p = MovementManager.Instance.Points_x4.Find(p => p.x4 == movePoint.x && p.y4 == movePoint.y);
        target = p.gameObject.GetComponent<Transform>();
        targetPoint = p;

        if(targetPoint.x4 == currentPoint.x4 && targetPoint.y4 == currentPoint.y4)
            EndMove();
    }

    public override void EndMove() {
        foreach(var p in currentPoint.points)
            p.Reset();

        currentPoint = targetPoint;
        foreach(var p in currentPoint.points) {
            p.isFree = false;
            p.canDrop = false;
            p.data = PointData.BigEnemy;
        }

        SetAttackPoints();
        SetMovePoints();
        Attack();
    }





    public override int GetDamageByPlayer(int damage, QBitType qType) {
        if(hasShield && qType == colorData.qType)
            return 0;

        int powerRemain = Mathf.Clamp(damage - healthPoints, 0, damage);

        healthPoints = Mathf.Clamp(healthPoints - damage, 0, healthPoints);
        if(healthPoints <= 0) {
            foreach(var point in currentPoint.points) {
                MovementPoint movePoint = MovementManager.Instance.Points.Find(p => p.x == point.x && p.y == point.y);
                movePoint.Reset();
            }
            
            Field.Instance.bigEnemiesItems.Remove(this);
            Field.Instance.deathsCounter[eType] += 1;
            Player.Instance.enemiesKilled[eType] += 1;
            Destroy(this.gameObject);
        }

        hpCaption.text = healthPoints.ToString();

        return powerRemain;
    }





    public override Coordinate FindShortestWayToPoint(Coordinate point) {
        Coordinate shortestWayPoint = new Coordinate(currentPoint.x4, currentPoint.y4);
        Coordinate firstC = new Coordinate(currentPoint.points[0].x, currentPoint.points[0].y);
        int currentDistance = 100;

        foreach(var p in currentPoint.points) {
            Coordinate c = new Coordinate(p.x, p.y);
            int newDistance = CountDistanceBetweenPoints(point, c);
            if(newDistance < currentDistance)
                currentDistance = newDistance;
        }

        foreach(var p in availablePointsToMove) {
            if(!CheckPoint_x4_ForAvailability(p))
                continue;

            int newDistance = CountDistanceBetweenPoints(point, p);
            if(newDistance < currentDistance) {
                shortestWayPoint = p;
                currentDistance = newDistance;
            }


            Point_x4 p4 = MovementManager.Instance.Points_x4.Find(pp => pp.x4 == p.x && pp.y4 == p.y);
            foreach(var movePoint in p4.points) {
                Coordinate c = new Coordinate(movePoint.x, movePoint.y);
                newDistance = CountDistanceBetweenPoints(point, c);
                if(newDistance < currentDistance) {
                    shortestWayPoint = new Coordinate(p.x, p.y);
                    currentDistance = newDistance;
                }
            }
        }

        Point_x4 new_p4 = MovementManager.Instance.Points_x4.Find(p => p.x4 == shortestWayPoint.x && p.y4 == shortestWayPoint.y);
        foreach(var movePoint in new_p4.points)
            EnemiesMovementManager.Instance.AddBusyPoint(new Coordinate(movePoint.x, movePoint.y));

        return shortestWayPoint;
    }


    private bool CheckPoint_x4_ForAvailability(Coordinate c) {
        Point_x4 p4 = MovementManager.Instance.Points_x4.Find(p => p.x4 == c.x && p.y4 == c.y);

        if(p4 == null)
            return false;
            

        foreach(var p in p4.points) {
            MovementPoint mpToCheck = currentPoint.points.Find(point => point.x == p.x && point.y == p.y);
            if(mpToCheck != null)
                continue;

            if(p == null)
                return false;

            if(p.isObstacle || p.isDestroyable || p.isEnemy || p.isDefect || p.isPlayer || p.isBigEnemy)
                return false;

            Coordinate coord = new Coordinate(p.x, p.y);
            if(EnemiesMovementManager.Instance.IsPointBusy(coord))
                return false;
        }

        return true;
    }
}
