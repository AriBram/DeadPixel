using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_MassiveAttack : SkillData {

    public override void PlayAbility() {
        QBitType currentColor = Player.Instance.lastMoveType;
        List<QBit> qBitsToDestroy = Field.Instance.qBits.FindAll(q => q.data.qType == currentColor);
        foreach(var q in qBitsToDestroy)
            q.DestroyQbit();
    }

}
