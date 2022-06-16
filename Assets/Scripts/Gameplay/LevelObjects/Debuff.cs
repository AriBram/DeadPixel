using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff : MonoBehaviour {
    
    public int x4;
    public int y4;

    public Point_x4 point_x4;

    public DebuffType debuff;

    public int attackPower;


    public void Init(DebuffType debuff, Point_x4 point) {
        this.debuff = debuff;
        this.x4 = point.x4;
        this.y4 = point.y4;
        this.point_x4 = point;
        
        foreach(var p in point.points)
            p.debuff = this.debuff;
    }


    public void FindAndDebuffPlayer() {
        MovementPoint playerPoint = PlayerController.Instance.currentPoint;
        bool isPlayerInDebuffRadius = point_x4.points.Find(p => p.x == playerPoint.x && p.y == playerPoint.y) == null ? false : true;

        if(isPlayerInDebuffRadius) {
            switch(debuff) {
                case DebuffType.Puddle:
                    Player.Instance.health.ResetShields();
                    Player.Instance.health.GetDamage(attackPower);
                    break;
            }
        }
    }
}
