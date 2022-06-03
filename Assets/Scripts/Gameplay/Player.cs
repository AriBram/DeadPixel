using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


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

    public Image root;
    public QBitType colorType;

    public static Player Instance { get; private set; }

    public class AwakeEvent : UnityEvent { }
    [HideInInspector] public AwakeEvent onAwake = new AwakeEvent();
    
    
    void Awake() {
        Instance = this;
    }
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<DragInput>();

        activeTargetIndex = 0;
        maxPointIndex = 0;

        foreach(var point in MovementManager.Instance.Points)
            point.onPointReach.AddListener(SwitchTargetToNextPoint);

        qBitsCollected_Green = 0;
        qBitsCollected_Red = 0;
        qBitsCollected_Blue = 0;

        SetColorByType(QBitType.GREEN);

        onAwake.Invoke();
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
            EndMove();
            return;
        }

        activeTargetIndex++;
        target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();
    }


    public void PickQBit(QBitData data) {
        switch(data.qType) {
            case QBitType.GREEN:
                qBitsCollected_Green++;
                SetColorByType(QBitType.GREEN);
                break;
            case QBitType.RED:
                qBitsCollected_Red++;
                SetColorByType(QBitType.RED);
                break;
            case QBitType.BLUE:
                qBitsCollected_Blue++;
                SetColorByType(QBitType.BLUE);
                break;
        }
    }


    public void EndMove() {
        GameplayController.Instance.SetPrepareState();
        PlayerController.Instance.currentPoint.isFree = true;
        PlayerController.Instance.currentPoint.canDrop = true;
        PlayerController.Instance.currentPoint = activatedPoints[maxPointIndex];
        PlayerController.Instance.currentPoint.isFree = false;
        PlayerController.Instance.currentPoint.canDrop = false;
        activeTargetIndex = 0;
        input.ClearMovementTrack();
        activatedPoints.Clear();

        Field.Instance.DropTiles();
        Field.Instance.FillFreePoints();
    }


    public void SetColorByType(QBitType qType) {
        QBitData qData = GameData.Instance.qBits.Find(q => q.qType == qType);
        root.color = qData.color;
        colorType = qType;
    }



    public void SetSpawnPosition(Coordinate pos) {
        MovementPoint newPoint = MovementManager.Instance.Points.Find(p => p.x == pos.x && p.y == pos.y);
        Transform newPointTransform = newPoint.gameObject.GetComponent<Transform>();
        this.transform.position = new Vector3(newPointTransform.position.x, newPointTransform.position.y, newPointTransform.position.z);
        PlayerController.Instance.currentPoint.isFree = true;
        PlayerController.Instance.currentPoint.canDrop = true;
        PlayerController.Instance.currentPoint = newPoint;
        PlayerController.Instance.currentPoint.isFree = false;
        PlayerController.Instance.currentPoint.canDrop = false;
    }
}
