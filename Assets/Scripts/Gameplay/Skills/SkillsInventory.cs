using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SkillsInventory : MonoBehaviour {

    public List<SkillData> Skills;

    public static SkillsInventory Instance { get; private set; }

    public class SkillsDeactivatedEvent : UnityEvent { }
    [HideInInspector] public SkillsDeactivatedEvent onDeactivate = new SkillsDeactivatedEvent();


    void Awake() {
        Instance = this;
    }

    
    public void ActivateSkill(SkillType sType) {
        SkillData skill = Skills.Find(s => s.sType == sType);
        bool wasActive = skill.isActivated;

        foreach(var s in Skills)
            s.isActivated = false;

        if(wasActive)
            return;
        
        if(skill.IsAvailable())
            skill.isActivated = true;
    }

    public void PlaySkills() {
        SkillData activatedSkill = Skills.Find(s => s.isActivated == true);
        if(activatedSkill == null)
            return;
        
        activatedSkill.PlayAbility();

        DeactivateAll();
    }

    public void DeactivateAll() {
        foreach(var s in Skills)
            s.isActivated = false;

        onDeactivate.Invoke();
    }
}
