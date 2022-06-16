using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quant : MonoBehaviour {

    public int x;
    public int y;

    
    public void Init(MovementPoint point) {
        this.x = point.x;
        this.y = point.y;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(GameplayController.Instance.IsMove) {
                Player.Instance.PickQuant();
                DestroyQuant();
            }
        }

        else if(other.CompareTag("Enemy")) {
            //DestroyQbit();
        }
    }


    public void DestroyQuant() {
        MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == this.x && p.y == this.y);
        point.Reset();
        Quant quantToRemove = Field.Instance.quantsItems.Find(q => q.x == this.x && q.y == this.y);
        Field.Instance.quantsItems.Remove(quantToRemove);
        Destroy(this.gameObject);
    }
}