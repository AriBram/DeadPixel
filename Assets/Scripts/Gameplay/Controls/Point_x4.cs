using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point_x4 : MonoBehaviour {

    public int x4;
    public int y4;
    
    public List<MovementPoint> points;


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("BigEnemy")) {
            if(GameplayController.Instance.IsEnemyMove) {
                foreach(var p in points) {
                    if(p.isQbit) {
                        QBit qBitToDestroy = Field.Instance.qBits.Find(q => q.x == p.x && q.y == p.y);
                        qBitToDestroy.DestroyQbit();
                    }
                    /*Enemy e = other.gameObject.GetComponent<Enemy>();
                    if(e.targetPoint.x == this.x && e.targetPoint.y == this.y)
                        e.EndMove();*/
                }
            }
        }
    }
}
