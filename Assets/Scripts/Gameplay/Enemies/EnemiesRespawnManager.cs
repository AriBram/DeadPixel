using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemiesRespawnManager : MonoBehaviour {

    public List<RespawnData> data;

    public int moveCounter;

    public Dictionary<EnemyType, int> deathsCounter;

    public static EnemiesRespawnManager Instance { get; private set; }


    void Awake() {
        Instance = this;
    }




    public void Init(List<RespawnData> data) {
        this.data = data;
        moveCounter = 0;

        deathsCounter = new Dictionary<EnemyType, int>();
        var allEnemyTypes = Enum.GetValues(typeof(EnemyType));
        foreach(EnemyType eType in allEnemyTypes)
            deathsCounter[eType] = 0;
    }


    public void MakeRespawnIteration() {
        moveCounter++;

        foreach(var respawn in data) {
            switch(respawn.rType) {
                case RespawnType.Simple:
                    if(moveCounter % respawn.frequency == 0)
                        Spawn(respawn.eType);
                    break;
                case RespawnType.AfterGoalsComplete:
                    if(Field.Instance.isGoalsComplete && moveCounter % respawn.frequency == 0)
                        Spawn(respawn.eType);
                    break;
                case RespawnType.AfterDeath:
                    if(deathsCounter[respawn.eType] > 0) {
                        Spawn(respawn.eType);
                        deathsCounter[respawn.eType] = Mathf.Clamp(deathsCounter[respawn.eType] - 1, 0, deathsCounter[respawn.eType]);
                    }
                    break;
            }
        }
    }

    private void Spawn(EnemyType eType) {
        Field.Instance.SpawnEnemy(eType);
    }
}
