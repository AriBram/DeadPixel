using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Destroyable : MonoBehaviour {

    public DestroyableType dType;

    public int x;
    public int y;
    public int healthPoints;
    public int startHp_Min;
    public int startHp_Max;

    public TMP_Text hpCaption;


    public void Init(MovementPoint point) {
        x = point.x;
        y = point.y;
        healthPoints = Random.Range(startHp_Min, startHp_Max + 1);

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

            if(dType == DestroyableType.SkeletonSpawner)
                Player.Instance.skeletonSpawnersDestroyed += 1;
        }

        hpCaption.text = healthPoints.ToString();
    }


    public void Use() {
        switch(dType) {
            case DestroyableType.SkeletonSpawner:
                Field.Instance.SpawnEnemy(EnemyType.Skeleton);
                break;
        }
    }
}
