using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class SkillItemUI : MonoBehaviour {
    
    public SkillType skill;

    public Button skillBtn;

    public float y_offset;
    public Transform startPosition;

    public GameObject indicatorActivated;
    public GameObject icon;

    public class SkillActivatedEvent : UnityEvent<SkillType> { }
    [HideInInspector] public SkillActivatedEvent onSkillActivate = new SkillActivatedEvent();


    void Start() {
        skillBtn.onClick.AddListener(ActivateSkill);
        indicatorActivated.SetActive(false);
    }


    public void Refresh() {
        SkillData sk = SkillsInventory.Instance.Skills.Find(s => s.sType == skill);
        if(sk != null) {
            if(sk.isActivated)
                icon.transform.position = new Vector3(startPosition.position.x, startPosition.position.y + y_offset, startPosition.position.z);
            else
                icon.transform.position = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
            indicatorActivated.SetActive(sk.isActivated);
        }
        else {
            this.transform.position = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
            indicatorActivated.SetActive(false);
        }
    }

    
    private void ActivateSkill() {
        onSkillActivate.Invoke(skill);
    }
}
