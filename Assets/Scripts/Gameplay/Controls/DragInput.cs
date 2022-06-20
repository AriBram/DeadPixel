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

    public List<MovementPoint> unavailablePoints = new List<MovementPoint>();

    private QBitType choosenType;
    private QBitType firstColorInCombo;
    private List<int> powerRemainBuffer;

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
        Player.Instance.SetLoopAnimation("wait_to_move");
    }

    public void OnPointerUp(PointerEventData eventData) {
        UpdateMovementPathData();
        UpdateEditableMovementPoint();
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

                if(!activatedPoints.Contains(point) && !point.isObstacle && !unavailablePoints.Contains(point)) {
                    List<MovementPoint> bigEnemyPoints = FindAvailablePointsToMoveFromBigEnemy();
                    bool isPointInBigEnemyRadius = bigEnemyPoints.Find(p => p.x == point.x && p.y == point.y) != null;
                    if( (Mathf.Abs(point.x - lastActivatedPoint.x) <= 1 && Mathf.Abs(point.y - lastActivatedPoint.y) <= 1) || isPointInBigEnemyRadius) {
                        if(choosenType == QBitType.NONE)
                            choosenType = qType;

                        if(firstColorInCombo == QBitType.NONE) {
                            firstColorInCombo = qType;
                            Player.Instance.SetSkeletonColor(firstColorInCombo);
                        }

                        if(point.isQbit && qType == choosenType) {
                            point.Activate();
                            lastActivatedPoint.ActivateIndicator(GetMovemenetDirection(lastActivatedPoint, point), choosenType);
                            lastActivatedPoint = point;
                            activatedPoints.Add(point);
                            UpdateMovementPathData();
                        }
                        else if(point.isDestroyable || point.isEnemy || point.isBigEnemy) {
                            int powerRemain = 0;
                            if(powerRemainBuffer.Count > 0)
                                powerRemain = powerRemainBuffer[powerRemainBuffer.Count - 1];
                            if(powerRemain > 0 || (!lastActivatedPoint.isDestroyable && !lastActivatedPoint.isEnemy && !lastActivatedPoint.isPlayer && !lastActivatedPoint.isBigEnemy) ) {
                                if(point.isEnemy) {
                                    Enemy en = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == point.x && e.currentPoint.y == point.y);
                                    if(en.hasShield && en.colorData.qType == choosenType)
                                        return true;

                                    int attackPower = CountAttackPower(activatedPoints.Count - 1);
                                    int pr = Mathf.Clamp(attackPower - en.healthPoints, 0, attackPower);
                                    powerRemainBuffer.Add(pr);
                                    if(attackPower >= en.healthPoints)
                                        choosenType = QBitType.NONE;

                                    en.hpCaption.text = Mathf.Clamp(en.healthPoints - attackPower, 0, en.healthPoints).ToString();

                                    if(pr > 0)
                                        en.SetPowerRemainCaption(pr);
                                }
                                else if(point.isDestroyable) {
                                    Destroyable des = Field.Instance.destroyables.Find(d => d.x == point.x && d.y == point.y);

                                    int attackPower = CountAttackPower(activatedPoints.Count - 1);
                                    int pr = Mathf.Clamp(attackPower - des.healthPoints, 0, attackPower);
                                    powerRemainBuffer.Add(pr);

                                    des.hpCaption.text = Mathf.Clamp(des.healthPoints - attackPower, 0, des.healthPoints).ToString();

                                    if(pr > 0)
                                        des.SetPowerRemainCaption(pr);
                                }
                                point.Activate();
                                lastActivatedPoint.ActivateIndicator(GetMovemenetDirection(lastActivatedPoint, point), choosenType);
                                lastActivatedPoint = point;
                                activatedPoints.Add(point);
                                UpdateMovementPathData();
                            }
                        }
                        else if(point.isQuant) {
                            choosenType = QBitType.NONE;
                            point.Activate();
                            lastActivatedPoint.ActivateIndicator(GetMovemenetDirection(lastActivatedPoint, point), choosenType);
                            lastActivatedPoint = point;
                            activatedPoints.Add(point);
                            UpdateMovementPathData();
                        }
                    }
                }

                else if(activatedPoints.Count >= 2) {
                    if(point.x == activatedPoints[activatedPoints.Count - 2].x && point.y == activatedPoints[activatedPoints.Count - 2].y) {
                        if(lastActivatedPoint.isBigEnemy) {
                            for(int i = 0; i < 4; i++) {
                                if(unavailablePoints.Count > 0)
                                    unavailablePoints.RemoveAt(unavailablePoints.Count - 1);
                            }
                        }
                        else if(lastActivatedPoint.isEnemy) {
                            Enemy en = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == lastActivatedPoint.x && e.currentPoint.y == lastActivatedPoint.y);
                            en.RefreshHpCaption();
                            powerRemainBuffer.RemoveAt(powerRemainBuffer.Count - 1);
                        }
                        else if(lastActivatedPoint.isDestroyable) {
                            Destroyable des = Field.Instance.destroyables.Find(d => d.x == lastActivatedPoint.x && d.y == lastActivatedPoint.y);
                            des.RefreshHpCaption();
                            powerRemainBuffer.RemoveAt(powerRemainBuffer.Count - 1);
                        }

                        lastActivatedPoint.Deactivate();
                        point.ResetIndicators();
                        activatedPoints.RemoveAt(activatedPoints.Count - 1);
                        lastActivatedPoint = point;
                        UpdateMovementPathData();

                        if(choosenType == QBitType.NONE && lastActivatedPoint.isQbit)
                            choosenType = qType;
                        else if(lastActivatedPoint.isEnemy) {
                            Enemy en = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == point.x && e.currentPoint.y == point.y);
                            int attackPower = CountAttackPower(activatedPoints.Count - 2);
                            if(attackPower >= en.healthPoints)
                                choosenType = QBitType.NONE;
                        }
                        else if(lastActivatedPoint.isBigEnemy) {
                            //big enemy cancel logic
                        }
                        else if(lastActivatedPoint.isQuant) {
                            choosenType = QBitType.NONE;
                        }
                        else if(lastActivatedPoint.isPlayer) {
                            choosenType = QBitType.NONE;
                            firstColorInCombo = QBitType.NONE;
                            Player.Instance.SetSkeletonColor(Player.Instance.colorType);
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
        firstColorInCombo = QBitType.NONE;
        Player.Instance.SetSkeletonColor(Player.Instance.colorType);

        lastActivatedPoint = PlayerController.Instance.currentPoint;
        PlayerController.Instance.currentPoint.isFree = false;
        activatedPoints.Clear();
        activatedPoints.Add(lastActivatedPoint);

        foreach(var point in points) {
            point.Deactivate();
            point.ResetIndicators();
        }

        powerRemainBuffer = new List<int>();

        MovementManager.Instance.ClearMovementTrack();

        unavailablePoints.Clear();

        Field.Instance.RefreshAllEnemiesHpCaptions();
    }


    
    
    public MovementDirection GetMovemenetDirection(MovementPoint point_1, MovementPoint point_2) {
        Coordinate diff = CountDifferenceBetweenPoints(point_1, point_2);
        MovementDirection dir = directionData.Find(d => d.difference.x == diff.x && d.difference.y == diff.y).direction;
        return dir;
    }
    
    public Coordinate CountDifferenceBetweenPoints(MovementPoint point_1, MovementPoint point_2) {
        return new Coordinate(Mathf.Clamp(point_2.x - point_1.x, -1, 1), Mathf.Clamp(point_2.y - point_1.y, -1, 1));
    }



    public void UpdateMovementPathData() {
        MovementManager.Instance.SetMovementTrack(activatedPoints);
        if(activatedPoints.Count <= 1)
            Player.Instance.SetLoopAnimation("idle");
        else
            Player.Instance.SetLoopAnimation("wait_to_move");
    }
    
    public void UpdateEditableMovementPoint() {
        foreach(var p in activatedPoints)
            p.canEditMovementPath = false;

        if(activatedPoints.Count > 1)
            lastActivatedPoint.canEditMovementPath = true;
    }



    public int CountAttackPower(int targetIndex) {
        int powerRemain = 0;
        if(powerRemainBuffer.Count > 0)
            powerRemain = powerRemainBuffer[powerRemainBuffer.Count - 1];

        int attackPower = powerRemain;
        for(int i = targetIndex; i >= 0; i--) {
            if(activatedPoints[i].data == PointData.None || activatedPoints[i].isQbit || activatedPoints[i].isQuant) //bug
                attackPower++;
            else
                return attackPower;
        }
        return attackPower;
    }



    public List<MovementPoint> FindAvailablePointsToMoveFromBigEnemy() {
        List<MovementPoint> output_mp_list = new List<MovementPoint>();

        if(!lastActivatedPoint.isBigEnemy)
            return output_mp_list;

        List<Point_x4> points_x4 = new List<Point_x4>();
        foreach(var p4 in MovementManager.Instance.Points_x4) {
            if(p4.points.Find(p => p.x == lastActivatedPoint.x && p.y == lastActivatedPoint.y) != null)
                points_x4.Add(p4);
        }

        List<MovementPoint> p4_mp_list = new List<MovementPoint>();
        foreach(var p4 in points_x4) {
            int counter = 0;
            foreach(var p in p4.points) {
                if(p.isBigEnemy)
                    counter++;
                if(counter == p4.points.Count)
                    p4_mp_list = p4.points;
            }
        }

        if(p4_mp_list.Count != 4)
            return output_mp_list;

        foreach(var point in p4_mp_list)
            unavailablePoints.Add(point);

        MovementPoint mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[0].x - 1 && p.y == p4_mp_list[0].y);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[0].x - 1 && p.y == p4_mp_list[0].y + 1);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[0].x && p.y == p4_mp_list[0].y + 1);
        if(mp != null)
            output_mp_list.Add(mp);

        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[1].x + 1 && p.y == p4_mp_list[1].y);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[1].x + 1 && p.y == p4_mp_list[1].y + 1);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[1].x && p.y == p4_mp_list[1].y + 1);
        if(mp != null)
            output_mp_list.Add(mp);

        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[2].x - 1 && p.y == p4_mp_list[2].y);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[2].x - 1 && p.y == p4_mp_list[2].y - 1);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[2].x && p.y == p4_mp_list[2].y - 1);
        if(mp != null)
            output_mp_list.Add(mp);

        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[3].x + 1 && p.y == p4_mp_list[3].y);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[3].x + 1 && p.y == p4_mp_list[3].y - 1);
        if(mp != null)
            output_mp_list.Add(mp);
        mp = MovementManager.Instance.Points.Find(p => p.x == p4_mp_list[3].x && p.y == p4_mp_list[3].y - 1);
        if(mp != null)
            output_mp_list.Add(mp);
        
        return output_mp_list;
    }
}


[System.Serializable]
public class CoordinatesDirectionData {
    public Coordinate difference;
    public MovementDirection direction;
} 

