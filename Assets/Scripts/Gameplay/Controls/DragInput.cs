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

    private QBitType choosenType;

    public List<CoordinatesDirectionData> directionData;


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
        //Debug.Log("activated points count: " + activatedPoints.Count.ToString());
        MovementManager.Instance.SetMovementTrack(activatedPoints);
    }



    public bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
 
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer) {
                MovementPoint point = curRaysastResult.gameObject.GetComponent<MovementPoint>();

                QBit qBit = Field.Instance.qBits.Find(q => q.x == point.x && q.y == point.y);
                QBitType qType = QBitType.NONE;
                if(qBit != null)
                    qType = qBit.data.qType;

                if(!activatedPoints.Contains(point) && !point.isObstacle) {
                    if(Mathf.Abs(point.x - lastActivatedPoint.x) <= 1 && Mathf.Abs(point.y - lastActivatedPoint.y) <= 1) {
                        if(choosenType == QBitType.NONE)
                            choosenType = qType;

                        if(point.isQbit && qType == choosenType) {
                            point.Activate();
                            lastActivatedPoint.ActivateIndicator(GetMovemenetDirection(lastActivatedPoint, point));
                            lastActivatedPoint = point;
                            activatedPoints.Add(point);
                        }
                        else if(point.isDestroyable || point.isEnemy) {
                            if(!lastActivatedPoint.isDestroyable && !lastActivatedPoint.isEnemy && !lastActivatedPoint.isPlayer) {
                                point.Activate();
                                lastActivatedPoint.ActivateIndicator(GetMovemenetDirection(lastActivatedPoint, point));
                                lastActivatedPoint = point;
                                activatedPoints.Add(point);
                            }
                        }
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
        choosenType = QBitType.NONE;

        lastActivatedPoint = PlayerController.Instance.currentPoint;
        PlayerController.Instance.currentPoint.isFree = false;
        activatedPoints.Clear();
        activatedPoints.Add(lastActivatedPoint);

        foreach(var point in points) {
            point.Deactivate();
            point.ResetIndicators();
        }

        MovementManager.Instance.ClearMovementTrack();
    }


    
    
    public MovementDirection GetMovemenetDirection(MovementPoint point_1, MovementPoint point_2) {
        Coordinate diff = CountDifferenceBetweenPoints(point_1, point_2);
        MovementDirection dir = directionData.Find(d => d.difference.x == diff.x && d.difference.y == diff.y).direction;
        return dir;
    }
    
    public Coordinate CountDifferenceBetweenPoints(MovementPoint point_1, MovementPoint point_2) {
        return new Coordinate(point_2.x - point_1.x, point_2.y - point_1.y);
    }
}


[System.Serializable]
public class CoordinatesDirectionData {
    public Coordinate difference;
    public MovementDirection direction;
} 

