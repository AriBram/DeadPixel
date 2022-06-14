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
                }
                BigEnemy e = other.gameObject.GetComponent<BigEnemy>();
                Debug.Log("big enemy target: " + e.targetPoint.x4.ToString() + " " + e.targetPoint.y4.ToString() + "; this: " + this.x4.ToString() + " " + this.y4.ToString());
                if(e.targetPoint.x4 == this.x4 && e.targetPoint.y4 == this.y4)
                    e.EndMove();
            }
        }
    }
}
