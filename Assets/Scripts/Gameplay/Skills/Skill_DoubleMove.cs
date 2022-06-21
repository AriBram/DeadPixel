using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DoubleMove : SkillData {

    public int extraMovesValue;
    
    public override void Init() {

    }
    
    public override void PlayAbility() {
        Player.Instance.GiveExtraMoves(extraMovesValue);
    }
}
