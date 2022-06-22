using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;


public class Destroyable : MonoBehaviour {

    public DestroyableType dType;

    public int x;
    public int y;
    public int healthPoints;
    public int startHp_Min;
    public int startHp_Max;

    public TMP_Text hpCaption;

    public SkeletonGraphic anim;


    public void Init(MovementPoint point) {
        x = point.x;
        y = point.y;
        healthPoints = Random.Range(startHp_Min, startHp_Max + 1);

        hpCaption.text = healthPoints.ToString();

        if(anim != null) {
            anim.Initialize(true);
            anim.AnimationState.SetAnimation(0, "idle", true);
        }
    }


    public int GetDamageByPlayer(int damage) {
        int powerRemain = Mathf.Clamp(damage - healthPoints, 0, damage);

        healthPoints = Mathf.Clamp(healthPoints - damage, 0, healthPoints);
        if(healthPoints <= 0) {
            MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == this.x && p.y == this.y);
            point.Reset();
            Destroyable fieldDestroyable = Field.Instance.destroyables.Find(d => d.x == this.x && d.y == this.y);
            if(fieldDestroyable != null)
                Field.Instance.destroyables.Remove(fieldDestroyable);
            
            if(anim != null) {
                anim.AnimationState.SetAnimation(0, "idle", false);
                anim.AnimationState.AddAnimation(0, "destroy", false, 0).Complete += e => Destroy(this.gameObject);
            }
            else
                Destroy(this.gameObject);

            if(dType == DestroyableType.SkeletonSpawner)
                Player.Instance.skeletonSpawnersDestroyed += 1;
        }

        hpCaption.text = healthPoints.ToString();

        return powerRemain;
    }


    public void Use() {
        switch(dType) {
            case DestroyableType.SkeletonSpawner:
                Field.Instance.SpawnEnemy(EnemyType.Skeleton, false);
                break;
        }
    }


    public void RefreshHpCaption() {
        hpCaption.text = healthPoints.ToString();
    }
}
