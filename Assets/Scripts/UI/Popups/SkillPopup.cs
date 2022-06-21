using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class SkillPopup : MenuBase, IPointerClickHandler, IDragHandler {

    public RectTransform clickRect;

    public Image skillIcon;


    public void Init(SkillType sType) {
        SkillData skill = SkillsInventory.Instance.Skills.Find(s => s.sType == sType);
        skillIcon.sprite = skill.bigSprite;
    }


    public override void Show() {
        gameObject.SetActive(true);
    }

    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnBackPressed() {}


    public void OnPointerClick(PointerEventData eventData) {
        if (!RectTransformUtility.RectangleContainsScreenPoint(clickRect, eventData.position, eventData.pressEventCamera)) {
            UI.Instance.PopMenu();
        }
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        DraggedDirection dir = GetDragDirection(dragVectorDirection);
        
        if(dir == DraggedDirection.Down)
            UI.Instance.PopMenu();
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
