using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DragInput : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler {

    int UILayer;
    public Canvas parentCanvas;

    public List<MovementPoint> points = new List<MovementPoint>();

    public List<MovementPoint> activatedPoints = new List<MovementPoint>();
    public MovementPoint lastActivatedPoint;


    void Start() {
        UILayer = LayerMask.NameToLayer("Tile");
        
        foreach(var point in MovementManager.Instance.Points)
            points.Add(point);

        ClearMovementTrack();
    }
 
    void Update() {}

    public void OnDrag(PointerEventData eventData) {
        if(!GameplayController.Instance.IsPrepare)
            return;
        
        bool isPointerOverUI = IsPointerOverUIElement();
    }

    public void OnEndDrag(PointerEventData eventData) {
        //onDragEnd.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData) {
        ClearMovementTrack();
        IsPointerOverUIElement();
    }

    public void OnPointerUp(PointerEventData eventData) {
        //onDragEnd.Invoke();
    }



    public bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
 
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer) {
                MovementPoint point = curRaysastResult.gameObject.GetComponent<MovementPoint>();
                if(!activatedPoints.Contains(point)) {
                    if(Mathf.Abs(point.x - lastActivatedPoint.x) <= 1 && Mathf.Abs(point.y - lastActivatedPoint.y) <= 1) {
                        point.Activate();
                        lastActivatedPoint = point;
                        activatedPoints.Add(point);
                    }
                }
                return true;
            }
        }
        return false;
    }
 
 
    static List<RaycastResult> GetEventSystemRaycastResults() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    

    public void ClearMovementTrack() {
        lastActivatedPoint = PlayerController.Instance.currentPoint;
        activatedPoints.Clear();
        activatedPoints.Add(lastActivatedPoint);

        foreach(var point in points)
            point.Deactivate();

        MovementManager.Instance.ClearMovementTrack();
    }
}

