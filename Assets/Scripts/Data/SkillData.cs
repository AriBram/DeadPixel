using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillData : MonoBehaviour {
    
    public SkillType sType;
    
    public Sprite bigSprite;
    
    public int memoryCost;
    public int cooldown;

    public int cooldownRemain;

    public bool isActivated;


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


public enum SkillType {DoubleMove, MassiveAttack}
