using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defect : QBit
{
    public List<Coordinate> attackPoints;

    public int attackPower;

    public Color color;


    public void Init(MovementPoint point) {
        x = point.x;
        y = point.y;

        attackPoints = new List<Coordinate>();
        attackPoints.Add(new Coordinate(x + 1, y));
        attackPoints.Add(new Coordinate(x - 1, y));
        attackPoints.Add(new Coordinate(x, y + 1));
        attackPoints.Add(new Coordinate(x, y - 1));

        data = new QBitData(QBitType.DEFECT, color);
    }

    public void FindAndAttackPlayer() {
        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        bool isPlayerInAttackRadius = attackPoints.Find(ap => ap.x == playerPoint.x && ap.y == playerPoint.y) == null ? false : true;

        if(isPlayerInAttackRadius)
            Player.Instance.health.GetDamage(attackPower);
    }
}
