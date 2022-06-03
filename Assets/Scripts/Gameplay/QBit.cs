using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QBit : MonoBehaviour {

    public QBitData data;

    public int x;
    public int y;

    public Image frame;

    
    public void Init(QBitData data, MovementPoint point) {
        this.data = data;
        this.x = point.x;
        this.y = point.y;

        frame.color = data.color;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(GameplayController.Instance.IsMove) {
                Player.Instance.PickQBit(data);
                MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == this.x && p.y == this.y);
                point.Reset();
                QBit qBitToRemove = Field.Instance.qBits.Find(q => q.x == this.x && q.y == this.y);
                Field.Instance.qBits.Remove(qBitToRemove);
                Destroy(this.gameObject);
            }
        }
    }
}
