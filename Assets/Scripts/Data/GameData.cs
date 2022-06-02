using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameData : MonoBehaviour {

    public List<QBitData> qBits;
    
    public static GameData Instance { get; private set; }


    void Start() {
        Instance = this;
    }


    public QBitData GetRandomQBit() {
        return qBits[Random.Range(0, qBits.Count)];
    }
}
