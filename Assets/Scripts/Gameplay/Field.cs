using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using TMPro;


public class Field : MonoBehaviour {
    
    public Transform qBitsContainer;
    public GameObject qBitPrefab;

    public Transform obstacleContainer;
    public GameObject obstaclePrefab;

    public Transform destroyableContainer;
    public GameObject destroyablePrefab_Simple;
    public GameObject destroyablePrefab_SkeletonSpawner;

    public Transform defectContainer;
    public GameObject defectPrefab;

    public Transform enemiesContainer;
    public GameObject enemyPrefab_Worm;
    public GameObject enemyPrefab_Skeleton;
    public GameObject enemyPrefab_Zombie;
    public GameObject enemyPrefab_Agent;
    public int enemiesCounter;
    public int maxEnemiesCanMove;
    public Dictionary<EnemyType, int> deathsCounter;

    public List<GameObject> qBitsLinks = new List<GameObject>();
    public List<QBit> qBits = new List<QBit>();
    public List<GameObject> obstaclesLinks = new List<GameObject>();
    public List<GameObject> destroyablesLinks = new List<GameObject>();
    public List<Destroyable> destroyables = new List<Destroyable>();
    public List<GameObject> defectsLinks = new List<GameObject>();
    public List<Defect> defectsItems = new List<Defect>();
    public List<GameObject> enemiesLinks = new List<GameObject>();
    public List<Enemy> enemiesItems = new List<Enemy>();

    public GoalsController goals;
    public bool isGoalsComplete;
    public Coordinate exitFromLevelPoint;
    public GameObject goalsCompleteUI;

    public bool isMovesLimited;
    public int movesCount;
    public GameObject movesCounter;
    public TMP_Text movesCountCaption;

    public List<QBitType> availableColors;

    public int size_X;
    public int size_Y;

    public static Field Instance { get; private set; }

    public class FieldInitEvent : UnityEvent { }
    [HideInInspector] public FieldInitEvent onFieldInit = new FieldInitEvent();

    
    void Awake() {
        Instance = this;

        goals.onGoalsComplete.AddListener(SetGoalsCompleteState);
    }

    void Start() {}

    void Update() {
        if(GameplayController.Instance.IsEnemyMove) {
            //Debug.Log("counter: " + enemiesCounter.ToString() + "; items: " + enemiesItems.Count.ToString());
            if(enemiesCounter == Mathf.Clamp(enemiesItems.Count, 0, maxEnemiesCanMove))
                Refill();
        }
    }


    public void Init() {
        DestroyField();
        LevelData level = GameData.Instance.GetCurrentLevel();
        SpawnLevelElements(level);

        isGoalsComplete = false;
        goalsCompleteUI.SetActive(false);

        EnemiesRespawnManager.Instance.Init(level.respawns);
        deathsCounter = new Dictionary<EnemyType, int>();
        var allEnemyTypes = Enum.GetValues(typeof(EnemyType));
        foreach(EnemyType eType in allEnemyTypes)
            deathsCounter[eType] = 0;

        Player.Instance.Init();

        isMovesLimited = level.movesLimitData.isMovesLimited;
        movesCounter.SetActive(isMovesLimited);
        if(isMovesLimited) {
            movesCount = level.movesLimitData.movesCount;
            movesCountCaption.text = movesCount.ToString();
        }

        availableColors = new List<QBitType>();
        foreach(var c in level.availableColors)
            availableColors.Add(c);

        FillFreePoints();
        
        onFieldInit.Invoke();
    }



    public void DestroyField() {
        foreach(GameObject qBit in qBitsLinks)
            Destroy(qBit);

        foreach(GameObject obs in obstaclesLinks)
            Destroy(obs);

        foreach(GameObject des in destroyablesLinks)
            Destroy(des);

        foreach(GameObject def in defectsLinks)
            Destroy(def);

        foreach(GameObject en in enemiesLinks)
            Destroy(en);

        qBitsLinks.Clear();
        obstaclesLinks.Clear();
        destroyablesLinks.Clear();
        defectsLinks.Clear();
        enemiesLinks.Clear();

        qBits.Clear();
        destroyables.Clear();
        defectsItems.Clear();
        enemiesItems.Clear();

        foreach(var point in MovementManager.Instance.Points)
            point.Reset();
    }



    public void DropTiles() {
        for (int x = 0; x < size_X; x++) {
            for (int y = 0; y < size_Y; y++) {
                MovementPoint point = MovementManager.Instance.Points.Find(p => p.x == x && p.y == y);
                if(!point.isFree)
                    continue;

                for(int i = 1; y + i < size_Y; i++) {
                    MovementPoint newPoint = MovementManager.Instance.Points.Find(p => p.x == x && p.y == y + i);
                    if (!newPoint.isFree && newPoint.canDrop) {
                        Transform dropPoint = point.gameObject.GetComponent<Transform>();
                        if(newPoint.isQbit) {
                            QBit qBitToDrop = qBits.Find(q => q.x == newPoint.x && q.y == newPoint.y);
                            
                            qBitToDrop.gameObject.transform.position = new Vector3(dropPoint.position.x, dropPoint.position.y, dropPoint.position.z);
                            qBitToDrop.Init(qBitToDrop.data, point);
                            point.data = PointData.QBit;
                        }
                        else if(newPoint.isDefect) {
                            Defect defectToDrop = defectsItems.Find(d => d.x == newPoint.x && d.y == newPoint.y);
                            defectToDrop.gameObject.transform.position = new Vector3(dropPoint.position.x, dropPoint.position.y, dropPoint.position.z);
                            defectToDrop.Init(point);  
                            point.data = PointData.Defect;
                        }
                        point.isFree = false;
                        newPoint.Reset();
                        break;
                    }
                }
            }
        }
    }


    public void FillFreePoints() {
        foreach(var spawnPoint in MovementManager.Instance.Points) {
            if(spawnPoint.isFree) {
                Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();
                var item = Instantiate(qBitPrefab, qBitsContainer);
                item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);
                qBitsLinks.Add(item);
                spawnPoint.isFree = false;
                spawnPoint.data = PointData.QBit;
                QBit qBit = item.GetComponent<QBit>();
                QBitData qBitData = GetRandomColor();
                qBit.Init(qBitData, spawnPoint);
                qBits.Add(qBit);
            }
        }

        DestroyDuplicates(); //bug
    }

    public QBitData GetRandomColor() {
        QBitType qType = availableColors[Random.Range(0, availableColors.Count)];
        QBitData qBitData = GameData.Instance.GetQBitDataByType(qType);
        return qBitData;
    }

    private void DestroyDuplicates() {
        foreach(var point in MovementManager.Instance.Points) {
            List<QBit> qBitsByPoint = qBits.FindAll(q => q.x == point.x && q.y == point.y);
            if(qBitsByPoint.Count > 1) {
                qBits.Remove(qBitsByPoint[0]);
                Destroy(qBitsByPoint[0].gameObject);
                Debug.Log("duplicate destroyed");
            }
        }
    }



    public void SpawnLevelElements(LevelData level) {
        SpawnPlayer(level.player);
        SpawnObstacles(level.obstacles);
        SpawnDestroyables(level.destroyables);
        SpawnDefects(level.defects);
        SpawnEnemies(level.enemies);
        goals.Init(level.goals);
    }

    public void SpawnPlayer(Coordinate pos) {
        Player.Instance.SetSpawnPosition(pos);
    }

    public void SpawnObstacles(List<Coordinate> obstacles) {
        foreach(var obstacle in obstacles) {
            MovementPoint spawnPoint = MovementManager.Instance.Points.Find(p => p.x == obstacle.x && p.y == obstacle.y);
            Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();
            var item = Instantiate(obstaclePrefab, obstacleContainer);
            item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);
            obstaclesLinks.Add(item);
            spawnPoint.isFree = false;
            spawnPoint.canDrop = false;
            spawnPoint.data = PointData.Obstacle;
        }
    }

    public void SpawnDestroyables(List<DestroyableData> data) {
        foreach(var destroyable in data) {
            MovementPoint spawnPoint = MovementManager.Instance.Points.Find(p => p.x == destroyable.point.x && p.y == destroyable.point.y);
            Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();

            GameObject item = new GameObject();
            Destroy(item);

            switch(destroyable.dType) {
                case DestroyableType.Simple:
                    item = Instantiate(destroyablePrefab_Simple, destroyableContainer);
                    break;
                case DestroyableType.SkeletonSpawner:
                    item = Instantiate(destroyablePrefab_SkeletonSpawner, destroyableContainer);
                    break;
            }

            item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);
            destroyablesLinks.Add(item);
            spawnPoint.isFree = false;
            spawnPoint.canDrop = false;
            spawnPoint.data = PointData.Destroyable;

            Destroyable d = item.GetComponent<Destroyable>();
            d.Init(spawnPoint);
            destroyables.Add(d);
        }
    }

    public void SpawnDefects(List<Coordinate> defects) {
        foreach(var defect in defects) {
            MovementPoint spawnPoint = MovementManager.Instance.Points.Find(p => p.x == defect.x && p.y == defect.y);
            Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();
            var item = Instantiate(defectPrefab, defectContainer);
            item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);
            defectsLinks.Add(item);
            spawnPoint.isFree = false;
            spawnPoint.canDrop = true;
            spawnPoint.data = PointData.Defect;

            Defect d = item.GetComponent<Defect>();
            d.Init(spawnPoint);
            defectsItems.Add(d);
        }
    }

    public void SpawnEnemies(List<EnemyData> enemies) {
        foreach(var e in enemies) {
            MovementPoint spawnPoint = MovementManager.Instance.Points.Find(p => p.x == e.point.x && p.y == e.point.y);
            Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();

            GameObject item = new GameObject();
            Destroy(item);

            switch(e.eType) {
                case EnemyType.Worm:
                    item = Instantiate(enemyPrefab_Worm, enemiesContainer);
                    break;
                case EnemyType.Skeleton:
                    item = Instantiate(enemyPrefab_Skeleton, enemiesContainer);
                    break;
                case EnemyType.Zombie:
                    item = Instantiate(enemyPrefab_Zombie, enemiesContainer);
                    break;
                case EnemyType.Agent:
                    item = Instantiate(enemyPrefab_Agent, enemiesContainer);
                    break;
            }

            item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);
            enemiesLinks.Add(item);
            spawnPoint.isFree = false;
            spawnPoint.canDrop = false;
            spawnPoint.data = PointData.Enemy;

            Enemy enemy = item.GetComponent<Enemy>();
            enemy.Init(e.eType, spawnPoint);
            enemy.onMoveEnd.AddListener(UpdateEnemiesCounter);
            enemiesItems.Add(enemy);
        }
    }



    public void SetGoalsCompleteState() {
        isGoalsComplete = true;
        goalsCompleteUI.SetActive(true);
    }

    public bool CheckIfLevelComplete(Coordinate playerPoint) {
        if(isGoalsComplete) {
            if(playerPoint.x == exitFromLevelPoint.x && playerPoint.y == exitFromLevelPoint.y) {
                MoveToNextLevel();
                return true;
            }
        }
        return false;
    }
    
    public void MoveToNextLevel() {
        UserData.Instance.currentLevel++;
        GameplayController.Instance.SetPrepareState();
        Init();
    }


    public void CheckDefectsForAttackPlayer() {
        foreach(var d in defectsItems)
            d.FindAndAttackPlayer();
    }


    public void ActivateEnemyMove() {
        enemiesCounter = 0;
        GameplayController.Instance.SetEnemyMoveState();
        EnemiesMovementManager.Instance.Reset();
        EnemiesMovementManager.Instance.SetQueue(enemiesItems);
        foreach(var e in enemiesItems)
            e.ActivateMove();
    }

    public void UpdateEnemiesCounter() {
        enemiesCounter++;
    }





    public void Refill() {
        DropTiles();
        FillFreePoints();
        goals.Refresh();
        GameplayController.Instance.SetPrepareState();
        EnemiesRespawnManager.Instance.MakeRespawnIteration();
        ApplyDeathCounterToRespawnManager();
        ActivateSkeletonSpawners();

        if(isMovesLimited) {
            movesCount -= 1;
            movesCountCaption.text = movesCount.ToString();
        }
    }



    public void SpawnEnemy(EnemyType eType) {
            List<MovementPoint> availablePoints = MovementManager.Instance.Points.FindAll(p => p.data == PointData.QBit || p.data == PointData.None);

            if(availablePoints.Count == 0)
                return;

            MovementPoint spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();
            
            if(spawnPoint.isQbit) {
                QBit qBitToDestroy = qBits.Find(q => q.x == spawnPoint.x && q.y == spawnPoint.y);
                qBitToDestroy.DestroyQbit();
            }

            GameObject item = new GameObject();
            Destroy(item);

            switch(eType) {
                case EnemyType.Worm:
                    item = Instantiate(enemyPrefab_Worm, enemiesContainer);
                    break;
                case EnemyType.Skeleton:
                    item = Instantiate(enemyPrefab_Skeleton, enemiesContainer);
                    break;
                case EnemyType.Zombie:
                    item = Instantiate(enemyPrefab_Zombie, enemiesContainer);
                    break;
                case EnemyType.Agent:
                    item = Instantiate(enemyPrefab_Agent, enemiesContainer);
                    break;
                case EnemyType.Defect:
                    item = Instantiate(defectPrefab, defectContainer);
                    break;
            }

            item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);

            if(eType != EnemyType.Defect) {
                enemiesLinks.Add(item);
                spawnPoint.isFree = false;
                spawnPoint.canDrop = false;
                spawnPoint.data = PointData.Enemy;

                Enemy enemy = item.GetComponent<Enemy>();
                enemy.Init(eType, spawnPoint);
                enemy.onMoveEnd.AddListener(UpdateEnemiesCounter);
                enemiesItems.Add(enemy);
            }
            else {
                defectsLinks.Add(item);
                spawnPoint.isFree = false;
                spawnPoint.canDrop = true;
                spawnPoint.data = PointData.Defect;

                Defect d = item.GetComponent<Defect>();
                d.Init(spawnPoint);
                defectsItems.Add(d);
            }
    }


    public void ApplyDeathCounterToRespawnManager() {
        foreach(var dc in deathsCounter)
            EnemiesRespawnManager.Instance.deathsCounter[dc.Key] += dc.Value;

        deathsCounter = new Dictionary<EnemyType, int>();
        var allEnemyTypes = Enum.GetValues(typeof(EnemyType));
        foreach(EnemyType eType in allEnemyTypes)
            deathsCounter[eType] = 0;
    }


    public void ActivateSkeletonSpawners() {
        foreach(var d in destroyables) {
            if(d.dType == DestroyableType.SkeletonSpawner)
                d.Use();
        }
    }
}
