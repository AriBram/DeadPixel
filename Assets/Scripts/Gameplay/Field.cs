using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Field : MonoBehaviour {
    
    public Transform qBitsContainer;
    public GameObject qBitPrefab;

    public List<GameObject> qBitsLinks = new List<GameObject>();

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
        SpawnField();
    }


    public void SpawnField() {
        DestroyField();

        foreach(var spawnPoint in MovementManager.Instance.Points) {
            if(spawnPoint.isFree) {
                Transform spawnTransform = spawnPoint.gameObject.GetComponent<Transform>();
                var item = Instantiate(qBitPrefab, qBitsContainer);
                item.transform.position = new Vector3(spawnTransform.position.x, spawnTransform.position.y, item.transform.position.z);
                qBitsLinks.Add(item);
                spawnPoint.isFree = false;
                QBit qBit = item.GetComponent<QBit>();
                QBitData qBitData = GameData.Instance.GetRandomQBit();
                qBit.Init(qBitData, spawnPoint);
            }
        }
    }

    public void DestroyField() {
        foreach(GameObject qBit in qBitsLinks)
            Destroy(qBit);

        qBitsLinks.Clear();
    }
}
