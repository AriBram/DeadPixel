using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Transform target;
    private Rigidbody2D rb;

    public float moveSpeed;

    public List<MovementPoint> activatedPoints = new List<MovementPoint>();
    private int activeTargetIndex;
    private int maxPointIndex;

    private DragInput input;

    public int qBitsCollected_Green;
    public int qBitsCollected_Red;
    public int qBitsCollected_Blue;

    public static Player Instance { get; private set; }
    
    
    void Start() {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<DragInput>();

        activeTargetIndex = 0;
        maxPointIndex = 0;

        foreach(var point in MovementManager.Instance.Points)
            point.onPointReach.AddListener(SwitchTargetToNextPoint);

        qBitsCollected_Green = 0;
        qBitsCollected_Red = 0;
        qBitsCollected_Blue = 0;
    }


    void FixedUpdate() {
        if(GameplayController.Instance.IsMove)
            Move();
    }


    public void ActivateMove() {
        activatedPoints.Clear();

        foreach(var point in MovementManager.Instance.ActivatedPoints)
            activatedPoints.Add(point);

        if(activatedPoints.Count < 1)
            return;

        activeTargetIndex = 0;
        maxPointIndex = activatedPoints.Count - 1;

        SwitchTargetToNextPoint();

        GameplayController.Instance.SetMoveState();
    }

    public void Move() {
        Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(temp);
    }


    public void SwitchTargetToNextPoint() {
        if(activeTargetIndex + 1 > maxPointIndex) {
            GameplayController.Instance.SetPrepareState();
            PlayerController.Instance.currentPoint.isFree = true;
            PlayerController.Instance.currentPoint = activatedPoints[maxPointIndex];
            PlayerController.Instance.currentPoint.isFree = false;
            activeTargetIndex = 0;
            input.ClearMovementTrack();
            activatedPoints.Clear();
            return;
        }

        activeTargetIndex++;
        target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();
    }


    public void PickQBit(QBitData data) {
        switch(data.qType) {
            case QBitType.GREEN:
                qBitsCollected_Green++;
                break;
            case QBitType.RED:
                qBitsCollected_Red++;
                break;
            case QBitType.BLUE:
                qBitsCollected_Blue++;
                break;
        }
    }
}
