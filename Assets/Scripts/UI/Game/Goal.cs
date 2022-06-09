using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Goal : MonoBehaviour {
    
    public Image icon;
    public TMP_Text value;

    public GoalData data;

    public void Init(GoalData goal) {
        data = goal;

        value.text = data.value.ToString();

        /*switch(goal.gType) {
            case GoalType.CollectGreen:
                icon.color = GameData.Instance.qBits.Find(q => q.qType == QBitType.GREEN).color;
                break;
            case GoalType.CollectBlue:
                icon.color = GameData.Instance.qBits.Find(q => q.qType == QBitType.BLUE).color;
                break;
            case GoalType.CollectRed:
                icon.color = GameData.Instance.qBits.Find(q => q.qType == QBitType.RED).color;
                break;
        }*/
        icon.sprite = data.icon;
    }

    
    public void Refresh() {
        GoalData newData = Field.Instance.goals.activeGoals.Find(g => g.gType == data.gType);
        if(newData == null)
            return;
        
        value.text = newData.value.ToString();
    }
}
