using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameData : MonoBehaviour {

    public List<QBitData> qBits;

    public List<LevelData> Levels;
    
    public static GameData Instance { get; private set; }


    void Awake() {
        Instance = this;
    }


    public QBitData GetRandomQBit() {
        return qBits[Random.Range(0, qBits.Count)];
    }

    public QBitData GetQBitDataByType(QBitType qType) {
        return qBits.Find(q => q.qType == qType);
    }

    public LevelData GetCurrentLevel() {
        int currentLevelIndex = Mathf.Clamp(UserData.Instance.currentLevel, 0, Levels.Count - 1);
        return Levels[currentLevelIndex];
    }
}
