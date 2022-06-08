using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Destroyable : MonoBehaviour
{
    public int x;
    public int y;
    public int healthPoints;

    public TMP_Text hpCaption;


    public void Init(MovementPoint point) {
        x = point.x;
        y = point.y;
        healthPoints = 2;

        hpCaption.text = healthPoints.ToString();
    }

    public void GetDamageByPlayer(int damage) {
        healthPoints -= damage;
        if(healthPoints <= 0) {
            MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == this.x && p.y == this.y);
            point.Reset();
            Destroyable fieldDestroyable = Field.Instance.destroyables.Find(d => d.x == this.x && d.y == this.y);
            if(fieldDestroyable != null)
                Field.Instance.destroyables.Remove(fieldDestroyable);
            Destroy(this.gameObject);
        }

        hpCaption.text = healthPoints.ToString();
    }
}
