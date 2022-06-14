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
        UpdateMovementPathData();
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
                                if(point.isEnemy) {
                                    Enemy en = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == point.x && e.currentPoint.y == point.y);
                                    if(en.hasShield && en.colorData.qType == choosenType)
                                        return true;

                                    int attackPower = CountAttackPower(activatedPoints.Count - 1);
                                    if(attackPower >= en.healthPoints)
                                        choosenType = QBitType.NONE;
                                }
                                point.Activate();
                                lastActivatedPoint.ActivateIndicator(GetMovemenetDirection(lastActivatedPoint, point));
                                lastActivatedPoint = point;
                                activatedPoints.Add(point);
                            }
                        }
                    }
                }

                else if(activatedPoints.Count >= 2) {
                    if(point.x == activatedPoints[activatedPoints.Count - 2].x && point.y == activatedPoints[activatedPoints.Count - 2].y) {
                        lastActivatedPoint.Deactivate();
                        point.ResetIndicators();
                        activatedPoints.RemoveAt(activatedPoints.Count - 1);
                        lastActivatedPoint = point;

                        if(choosenType == QBitType.NONE && lastActivatedPoint.isQbit)
                            choosenType = qType;
                        else if(lastActivatedPoint.isEnemy) {
                            Enemy en = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == point.x && e.currentPoint.y == point.y);
                            int attackPower = CountAttackPower(activatedPoints.Count - 2);
                            if(attackPower >= en.healthPoints)
                                choosenType = QBitType.NONE;
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



    public void UpdateMovementPathData() {
        UpdateEditableMovementPoint();
        MovementManager.Instance.SetMovementTrack(activatedPoints);
    }
    
    public void UpdateEditableMovementPoint() {
        foreach(var p in activatedPoints)
            p.canEditMovementPath = false;

        if(activatedPoints.Count > 1)
            lastActivatedPoint.canEditMovementPath = true;
    }



    public int CountAttackPower(int targetIndex) {
        int attackPower = 0;
        for(int i = targetIndex; i >= 0; i--) {
            if(activatedPoints[i].data == PointData.None || activatedPoints[i].isQbit) //bug
                attackPower++;
            else
                return attackPower;
        }
        return attackPower;
    }
}


[System.Serializable]
public class CoordinatesDirectionData {
    public Coordinate difference;
    public MovementDirection direction;
} 

