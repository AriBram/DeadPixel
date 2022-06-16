using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Spine.Unity;


public class Player : MonoBehaviour {

    public Health health;

    private Transform target;
    private Rigidbody2D rb;

    public float moveSpeed;

    public List<MovementPoint> activatedPoints = new List<MovementPoint>();
    private int activeTargetIndex;
    private int maxPointIndex;

    public DragInput input;

    public Dictionary<QBitType, int> qBitsCollected;
    public Dictionary<EnemyType, int> enemiesKilled;
    public int skeletonSpawnersDestroyed;

    public int comboCheckPointIndex;

    //public Image root;
    public SkeletonGraphic root;
    public QBitType colorType;
    public QBitType lastMoveType;

    public static Player Instance { get; private set; }

    public class AwakeEvent : UnityEvent { }
    [HideInInspector] public AwakeEvent onAwake = new AwakeEvent();
    
    
    void Awake() {
        Instance = this;
    }
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<DragInput>();
        health = GetComponent<Health>();

        activeTargetIndex = 0;
        maxPointIndex = 0;
        comboCheckPointIndex = 0;

        foreach(var point in MovementManager.Instance.Points)
            point.onPointReach.AddListener(SwitchTargetToNextPoint);

        Init();

        InitializeAnimation();
        SetSkeletonColor(QBitType.GREEN);

        onAwake.Invoke();
    }


    void FixedUpdate() {
        if(GameplayController.Instance.IsMove)
            Move();
    }


    public void Init() {
        enemiesKilled = new Dictionary<EnemyType, int>();
        var allEnemyTypes = Enum.GetValues(typeof(EnemyType));
        foreach(EnemyType eType in allEnemyTypes)
            enemiesKilled[eType] = 0;

        qBitsCollected = new Dictionary<QBitType, int>();
        var allQBitTypes = Enum.GetValues(typeof(QBitType));
        foreach(QBitType qType in allQBitTypes)
            qBitsCollected[qType] = 0;

        skeletonSpawnersDestroyed = 0;

        SetPlayerStartOrientation();
    }


    public void ActivateMove() {
        activatedPoints.Clear();

        foreach(var point in MovementManager.Instance.ActivatedPoints)
            activatedPoints.Add(point);

        if(activatedPoints.Count < 1)
            return;

        activeTargetIndex = 0;
        comboCheckPointIndex = 1;
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

        MovementDirection mDir = input.GetMovemenetDirection(activatedPoints[activeTargetIndex + 1], activatedPoints[activeTargetIndex]);

        if(activeTargetIndex + 1 == maxPointIndex && activatedPoints[activeTargetIndex + 1].isDestroyable) {
            StartCoroutine(FightWithDestroyableEnd(mDir));
            return;
        }
        else if(activatedPoints[activeTargetIndex + 1].isDestroyable) {
            StartCoroutine(FightWithDestroyableContinuous(mDir));
            return;
        }

        else if(activeTargetIndex + 1 == maxPointIndex && (activatedPoints[activeTargetIndex + 1].isEnemy || activatedPoints[activeTargetIndex + 1].isBigEnemy)) {
            StartCoroutine(FightWithEnemyEnd(mDir));
            return;
        }
        else if(activatedPoints[activeTargetIndex + 1].isEnemy || activatedPoints[activeTargetIndex + 1].isBigEnemy) {
            StartCoroutine(FightWithEnemyContinuous(mDir));
            return;
        }

        activeTargetIndex++;
        target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();

        SetMoveAnimation(mDir);
    }

    public IEnumerator FightWithDestroyableContinuous(MovementDirection mDir) {
        bool isLetal = AttackDestroyable(CountAttackPower(activeTargetIndex + 1), activatedPoints[activeTargetIndex + 1], mDir);
        float timing = isLetal ? 0.533f : 0.367f;
        yield return new WaitForSeconds(timing);

        activeTargetIndex += 2;
        target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();
        MovementDirection newDir = input.GetMovemenetDirection(activatedPoints[activeTargetIndex], activatedPoints[activeTargetIndex - 1]);
        SetMoveAnimation(newDir);
    }

    public IEnumerator FightWithDestroyableEnd(MovementDirection mDir) {
        bool isLetal = AttackDestroyable(CountAttackPower(activeTargetIndex + 1), activatedPoints[activeTargetIndex + 1], mDir);
        float timing = isLetal ? 0.533f : 0.367f;
        yield return new WaitForSeconds(timing);

        if(activatedPoints[activeTargetIndex + 1].isDestroyable)
            EndMove();
        else {
            activeTargetIndex++;
            target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();
            SetMoveAnimation(mDir);
        }
    }

    public IEnumerator FightWithEnemyContinuous(MovementDirection mDir) {
        bool isLetal = AttackEnemy(CountAttackPower(activeTargetIndex + 1), activatedPoints[activeTargetIndex + 1], mDir);
        float timing = isLetal ? 0.533f : 0.367f;
        yield return new WaitForSeconds(timing);

        activeTargetIndex += 2;
        target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();
        MovementDirection newDir = input.GetMovemenetDirection(activatedPoints[activeTargetIndex], activatedPoints[activeTargetIndex - 1]);
        SetMoveAnimation(newDir);
    }

    public IEnumerator FightWithEnemyEnd(MovementDirection mDir) {
        bool isLetal = AttackEnemy(CountAttackPower(activeTargetIndex + 1), activatedPoints[activeTargetIndex + 1], mDir);
        float timing = isLetal ? 0.533f : 0.367f;
        yield return new WaitForSeconds(timing);

        if(activatedPoints[activeTargetIndex + 1].isEnemy || activatedPoints[activeTargetIndex + 1].isBigEnemy) {
            EndMove();
        }
        else {
            activeTargetIndex++;
            target = activatedPoints[activeTargetIndex].gameObject.GetComponent<Transform>();
            SetMoveAnimation(mDir);
        }
    }


    public int CountAttackPower(int targetIndex) {
        int attackPower = 0;
        for(int i = comboCheckPointIndex; i < targetIndex; i++) {
            //Debug.Log(i.ToString() + ": " + activatedPoints[i].data.ToString());
            //if(activatedPoints[i].isQbit)
            if(activatedPoints[i].data == PointData.None || activatedPoints[i].isQbit) //bug
                attackPower++;
        }
        comboCheckPointIndex = targetIndex + 1;
        return attackPower;
    }


    public bool AttackDestroyable(int power, MovementPoint point, MovementDirection mDir) {
        Debug.Log("power: " + power.ToString() + "; point x: " + point.x.ToString() + " y: " + point.y.ToString());
        Destroyable destroyableToAttack = Field.Instance.destroyables.Find(d => d.x == point.x && d.y == point.y);
        destroyableToAttack.GetDamageByPlayer(power);
        bool isLetal = power >= destroyableToAttack.healthPoints ? true : false;
        SetAttackAnimation(mDir, isLetal);
        return isLetal;
    }

    public bool AttackEnemy(int power, MovementPoint point, MovementDirection mDir) {
        Debug.Log("power: " + power.ToString() + "; point x: " + point.x.ToString() + " y: " + point.y.ToString());
        Enemy enemyToAttack = Field.Instance.enemiesItems.Find(e => e.currentPoint.x == point.x && e.currentPoint.y == point.y);
        bool isLetal = false;
        if(enemyToAttack != null) {
            isLetal = power >= enemyToAttack.healthPoints ? true : false;
            SetAttackAnimation(mDir, isLetal);
            enemyToAttack.GetDamageByPlayer(power, colorType);
        }

        for(int i = 0; i < Field.Instance.bigEnemiesItems.Count; i++) {
            foreach(var p in Field.Instance.bigEnemiesItems[i].currentPoint.points) {
                if(p.x == point.x && p.y == point.y) {
                    isLetal = power >= Field.Instance.bigEnemiesItems[i].healthPoints ? true : false;
                    SetAttackAnimation(mDir, isLetal);
                    Field.Instance.bigEnemiesItems[i].GetDamageByPlayer(power, colorType);
                }
            }
        }

        return isLetal;
    }


    public void PickQBit(QBitData data) {
        qBitsCollected[data.qType]++;
        SetSkeletonColor(data.qType);
    }


    public void EndMove() {
        SetOneShotAnimation("idle");

        PlayerController.Instance.currentPoint.Reset();
        PlayerController.Instance.currentPoint = activatedPoints[activeTargetIndex];
        PlayerController.Instance.currentPoint.isFree = false;
        PlayerController.Instance.currentPoint.canDrop = false;
        PlayerController.Instance.currentPoint.data = PointData.Player;

        if(colorType == lastMoveType && activatedPoints.Count >= 3)
            health.GetHeal(0, 1);
        lastMoveType = colorType;

        activeTargetIndex = 0;
        input.ClearMovementTrack();
        activatedPoints.Clear();

        bool isLevelComplete = Field.Instance.CheckIfLevelComplete(new Coordinate(PlayerController.Instance.currentPoint.x, PlayerController.Instance.currentPoint.y));
        if(isLevelComplete)
            return;

        Field.Instance.CheckDefectsForAttackPlayer();
        Field.Instance.ActivateEnemyMove();
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
        PlayerController.Instance.currentPoint.data = PointData.Player;
    }






    public void InitializeAnimation() {
        root.Initialize(true);
        root.AnimationState.SetAnimation(0, "idle", false);
    }

    public void SetSkeletonColor(QBitType qType) {
        switch(qType) {
            case QBitType.GREEN:
                root.Skeleton.SetSkin("green");
                break;
            case QBitType.BLUE:
                root.Skeleton.SetSkin("blue");
                break;
            case QBitType.RED:
                root.Skeleton.SetSkin("red");
                break;
            case QBitType.PINK:
                root.Skeleton.SetSkin("pink");
                break;
        }

        root.Skeleton.SetSlotsToSetupPose();
        root.LateUpdate();
    }

    public void SetOneShotAnimation(string animName) {
        root.AnimationState.SetAnimation(0, animName, false);
        root.AnimationState.AddAnimation(0, "idle", false, 0);
    }

    public void SetLoopAnimation(string animName) {
        root.AnimationState.SetAnimation(0, animName, true);
    }

    public void SetMoveAnimation(MovementDirection mDir) {
        switch(mDir) {
            case MovementDirection.Up:
                SetLoopAnimation("move_down_up");
                break;
            case MovementDirection.Down:
                SetLoopAnimation("move_up_down");
                break;
            case MovementDirection.Right:
                FlipHorizontalToRight();
                SetLoopAnimation("move_horizontal");
                break;
            case MovementDirection.Left:
                FlipHorizontalToLeft();
                SetLoopAnimation("move_horizontal");
                break;
            case MovementDirection.Up_Right:
                FlipHorizontalToRight();
                SetLoopAnimation("move_diagonal_down_up");
                break;
            case MovementDirection.Up_Left:
                FlipHorizontalToLeft();
                SetLoopAnimation("move_diagonal_down_up");
                break;
            case MovementDirection.Down_Right:
                FlipHorizontalToRight();
                SetLoopAnimation("move_diagonal_up_down");
                break;
            case MovementDirection.Down_Left:
                FlipHorizontalToLeft();
                SetLoopAnimation("move_diagonal_up_down");
                break;
        }
    }

    public void SetAttackAnimation(MovementDirection mDir, bool isLetal) {
        switch(mDir) {
            case MovementDirection.Up:
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_vertical_down_up");
                break;
            case MovementDirection.Down:
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_vertical_up_down");
                break;
            case MovementDirection.Right:
                FlipHorizontalToRight();
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_horizontal");
                break;
            case MovementDirection.Left:
                FlipHorizontalToLeft();
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_horizontal");
                break;
            case MovementDirection.Up_Right:
                FlipHorizontalToRight();
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_horizontal");
                break;
            case MovementDirection.Up_Left:
                FlipHorizontalToLeft();
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_horizontal");
                break;
            case MovementDirection.Down_Right:
                FlipHorizontalToRight();
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_horizontal");
                break;
            case MovementDirection.Down_Left:
                FlipHorizontalToLeft();
                if(isLetal)
                    SetOneShotAnimation("attack_horizontal_letal");
                else
                    SetOneShotAnimation("attack_horizontal");
                break;
        }
    }


    public void FlipHorizontalToLeft() {
        root.Skeleton.ScaleX = 1;
    }

    public void FlipHorizontalToRight() {
        root.Skeleton.ScaleX = -1;
    }


    public void SetPlayerStartOrientation() {
        List<EnemyBase> enemies = new List<EnemyBase>();
        List<Enemy> simpleEnemies = new List<Enemy>(Field.Instance.enemiesItems);
        List<BigEnemy> bigEnemies = new List<BigEnemy>(Field.Instance.bigEnemiesItems);
        foreach(var e in simpleEnemies)
            enemies.Add(e);
        foreach(var e in bigEnemies)
            enemies.Add(e);

        if(enemies.Count == 0) {
            if(PlayerController.Instance.currentPoint.x < 3)
                FlipHorizontalToLeft();
            else if(PlayerController.Instance.currentPoint.x > 3)
                FlipHorizontalToRight();
        }
        else {
            MovementPoint closestEnemy = FindClosestEnemy(simpleEnemies, bigEnemies);
            MovementDirection newDir = input.GetMovemenetDirection(PlayerController.Instance.currentPoint, closestEnemy);
            switch(newDir) {
                case MovementDirection.Right: case MovementDirection.Up_Right: case MovementDirection.Down_Right:
                    FlipHorizontalToLeft();
                    break;
                case MovementDirection.Left: case MovementDirection.Up_Left: case MovementDirection.Down_Left:
                    FlipHorizontalToRight();
                    break;
            }
        }
    }

    public MovementPoint FindClosestEnemy(List<Enemy> simpleEnemies, List<BigEnemy> bigEnemies) {
        Coordinate closestEnemy = new Coordinate(simpleEnemies[0].currentPoint.x, simpleEnemies[0].currentPoint.y);
        Coordinate playerC = new Coordinate(PlayerController.Instance.currentPoint.x, PlayerController.Instance.currentPoint.y);
        int shortestDistance = CountDistanceBetweenPoints(playerC, closestEnemy);

        foreach(var e in simpleEnemies) {
            Coordinate c = new Coordinate(e.currentPoint.x, e.currentPoint.y);
            int newDistance = CountDistanceBetweenPoints(playerC, c);
            if(newDistance < shortestDistance) {
                closestEnemy = c;
                shortestDistance = newDistance;
            }
        }

        foreach(var e in bigEnemies) {
            foreach(var p in e.currentPoint.points) {
                Coordinate c = new Coordinate(p.x, p.y);
                int newDistance = CountDistanceBetweenPoints(playerC, c);
                if(newDistance < shortestDistance) {
                    closestEnemy = c;
                    shortestDistance = newDistance;
                }
            }
        }

        MovementPoint enemyPoint = MovementManager.Instance.Points.Find(p => p.x == closestEnemy.x && p.y == closestEnemy.y);
        
        return enemyPoint;
    }

    public int CountDistanceBetweenPoints(Coordinate point1, Coordinate point2) {
        int distance = 0;
        distance += Mathf.Abs(point2.x - point1.x) + Mathf.Abs(point2.y - point1.y);
        return distance;
    }
}
