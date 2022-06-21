using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData : MonoBehaviour {
    
    public int memoryCost;
    public int cooldown;

    private int cooldownRemain;

    public int isActivated;


    public virtual void Init() {
        cooldownRemain = 0;
    } 

    public abstract void PlayAbility();

    public void SetCooldown() {
        cooldownRemain = cooldown;
    }

    public void ReduceCooldown() {
        cooldownRemain--;
    }

    public bool IsAvailable() {
        return cooldownRemain <= 0;
    }
}