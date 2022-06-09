using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData {
    public Coordinate player;
    public List<Coordinate> obstacles;
    public List<Coordinate> destroyables;
    public List<Coordinate> defects;

    public List<EnemyData> enemies; 
    public List<RespawnData> respawns;

    public List<GoalData> goals;
}


[System.Serializable]
public class Coordinate {
    public int x;
    public int y;

    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
    }
}


public enum GoalType {CollectGreen, CollectBlue, CollectRed}

[System.Serializable]
public class GoalData {
    public GoalType gType;
    public int value;
    public Sprite icon;

    public GoalData(GoalType gType, int value, Sprite icon) {
        this.gType = gType;
        this.value = value;
        this.icon = icon;
    }
}


public enum EnemyType {Worm, Skeleton, Zombie, Agent, Spider, Defect}

[System.Serializable]
public class EnemyData {
    public EnemyType eType;
    public Coordinate point;

    public EnemyData(EnemyType eType, Coordinate point) {
        this.eType = eType;
        this.point = point;
    }
}


public enum RespawnType {Simple, AfterGoalsComplete, AfterDeath}

[System.Serializable]
public class RespawnData {
    public RespawnType rType;
    public EnemyType eType;
    public int frequency;

    public RespawnData(RespawnType rType, EnemyType eType, int frequency) {
        this.rType = rType;
        this.eType = eType;
        this.frequency = frequency;
    }
}
