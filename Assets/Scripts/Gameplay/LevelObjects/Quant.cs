using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;


public class Quant : MonoBehaviour {

    public int x;
    public int y;

    public SkeletonGraphic anim;


    void Awake() {
        anim.Initialize(true);
        anim.AnimationState.SetAnimation(0, "idle", true);
        SetActive(false);
    }
    
    
    public void Init(MovementPoint point) {
        this.x = point.x;
        this.y = point.y;
    }


    public void SetActive(bool isAct) {
        if(isAct) {
            anim.AnimationState.SetAnimation(0, "active", true);
            this.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        }
        else {
            anim.AnimationState.SetAnimation(0, "idle", true);
            this.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        }
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

        anim.AnimationState.SetAnimation(0, "destroy", false).Complete += e => Destroy(this.gameObject);
    }
}