using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;


public class SkillItemUI : MonoBehaviour, IDragHandler {
    
    public SkillType skill;

    public Button skillBtn;

    public float y_offset;
    public Transform startPosition;

    public GameObject indicatorActivated;
    public GameObject icon;
    public GameObject cooldown;
    public TMP_Text cdValue;

    public class SkillActivatedEvent : UnityEvent<SkillType> { }
    [HideInInspector] public SkillActivatedEvent onSkillActivate = new SkillActivatedEvent();


    void Start() {
        skillBtn.onClick.AddListener(ActivateSkill);
        indicatorActivated.SetActive(false);
        cooldown.SetActive(false);
        cdValue.text = "";
    }


    public void Refresh() {
        SkillData sk = SkillsInventory.Instance.Skills.Find(s => s.sType == skill);
        if(sk != null) {
            if(sk.isActivated)
                icon.transform.position = new Vector3(startPosition.position.x, startPosition.position.y + y_offset, startPosition.position.z);
            else
                icon.transform.position = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
            indicatorActivated.SetActive(sk.isActivated);

            cooldown.SetActive(sk.cooldownRemain > 0);
            cdValue.text = sk.cooldownRemain > 0 ? sk.cooldownRemain.ToString() : "";
        }
        else {
            this.transform.position = new Vector3(startPosition.position.x, startPosition.position.y, startPosition.position.z);
            indicatorActivated.SetActive(false);
            cooldown.SetActive(false);
            cdValue.text = "";
        }
    }

    
    private void ActivateSkill() {
        onSkillActivate.Invoke(skill);
    }



    public void OnDrag(PointerEventData eventData) {
        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        DraggedDirection dir = GetDragDirection(dragVectorDirection);
        
        if(dir == DraggedDirection.Up)
            UI.Instance.ShowSkillPopup(skill);
    }

    private enum DraggedDirection {
        Up,
        Down,
        Right,
        Left
    }

    private DraggedDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
          draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;

        else
          draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;

        return draggedDir;
    }
}
