using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SkillsInventory : MonoBehaviour {

    public List<SkillData> Skills;

    public static SkillsInventory Instance { get; private set; }

    public class SkillsDeactivatedEvent : UnityEvent { }
    [HideInInspector] public SkillsDeactivatedEvent onDeactivate = new SkillsDeactivatedEvent();

    public class SkillsCooldownEvent : UnityEvent { }
    [HideInInspector] public SkillsCooldownEvent onCooldown = new SkillsCooldownEvent();


    void Awake() {
        Instance = this;
    }

    void Start() {
        foreach(var s in Skills) {
            s.isActivated = false;
            s.cooldownRemain = 0;
        }
    }

    
    public void ActivateSkill(SkillType sType) {
        SkillData skill = Skills.Find(s => s.sType == sType);
        bool wasActive = skill.isActivated;

        foreach(var s in Skills)
            s.isActivated = false;

        if(wasActive)
            return;
        
        if(skill.IsAvailable() && Player.Instance.memory.value >= skill.memoryCost)
            skill.isActivated = true;
    }

    public void PlaySkills() {
        SkillData activatedSkill = Skills.Find(s => s.isActivated == true);
        if(activatedSkill == null)
            return;
        
        Player.Instance.memory.Use(activatedSkill.memoryCost);
        activatedSkill.PlayAbility();
        activatedSkill.SetCooldown();

        DeactivateAll();
    }

    public void DeactivateAll() {
        foreach(var s in Skills)
            s.isActivated = false;

        onDeactivate.Invoke();
    }

    public void ReduceCooldown() {
        foreach(var s in Skills)
            s.ReduceCooldown();

        onCooldown.Invoke();
    }
}
