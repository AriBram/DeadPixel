using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Field : MonoBehaviour {
    
    public Transform qBitsContainer;
    public GameObject qBitPrefab;

    public Transform obstacleContainer;
    public GameObject obstaclePrefab;

    public Transform destroyableContainer;
    public GameObject destroyablePrefab;

    public List<GameObject> qBitsLinks = new List<GameObject>();
    public List<QBit> qBits = new List<QBit>();
    public List<GameObject> obstaclesLinks = new List<GameObject>();
    public List<GameObject> destroyablesLinks = new List<GameObject>();
    public List<Destroyable> destroyables = new List<Destroyable>();

    public int size_X;
    public int size_Y;

    public static Field Instance { get; private set; }

    
    void Awake() {
        Instance = this;
    }

    void Start() {
        //Init();
    }

    void Update() {
        
    }


    public void Init() {
        DestroyField();
        LevelData level = GameData.Instance.GetCurrentLevel();
        SpawnLevelElements(level);
        FillFreePoints();
    }



    public void DestroyField() {
        foreach(GameObject qBit in qBitsLinks)
            Destroy(qBit);

        qBitsLinks.Clear();
        qBits.Clear();
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
                        QBit qBitToDrop = qBits.Find(q => q.x == newPoint.x && q.y == newPoint.y);
                        Transform dropPoint = point.gameObject.GetComponent<Transform>();
                        qBitToDrop.gameObject.transform.position = new Vector3(dropPoint.position.x, dropPoint.position.y, dropPoint.position.z);
                        qBitToDrop.Init(qBitToDrop.data, point);
                        newPoint.Reset();
                        point.isFree = false;
                        point.data = PointData.QBit;
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
                QBitData qBitData = GameData.Instance.GetRandomQBit();
                qBit.Init(qBitData, spawnPoint);
                qBits.Add(qBit);
            }
        }
    }



    public void SpawnLevelElements(LevelData level) {
        SpawnPlayer(level.player);
        SpawnObstacles(level.obstacles);
        SpawnDestroyables(level.destroyables);
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

    public void SpawnDestroyables(List<Coordinate> obstacles) {
        foreach(var destroyable in obstacles) {
            MovementPoint spawnPoint = MovementManager.Instance.Points.Find(p => p.x == destroyable.x && p.y == destroyable.y);
            Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();
            var item = Instantiate(destroyablePrefab, destroyableContainer);
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
}
