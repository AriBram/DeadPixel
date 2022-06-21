using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsInventoryUI : MonoBehaviour {
    
    public List<SkillItemUI> skillItems;


    void Start() {
        SkillsInventory.Instance.onDeactivate.AddListener(Refresh);
        SkillsInventory.Instance.onCooldown.AddListener(Refresh);

        foreach(var s in skillItems)
            s.onSkillActivate.AddListener(ActivateSkill);
    }

    public void Refresh() {
        foreach(var s in skillItems)
            s.Refresh();
    }


    private void ActivateSkill(SkillType sType) {
        SkillsInventory.Instance.ActivateSkill(sType);
        Refresh();
    }
}
