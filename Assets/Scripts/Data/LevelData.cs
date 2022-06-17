using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData {
    public Coordinate player;
    public List<Coordinate> obstacles;
    public List<DestroyableData> destroyables;
    public List<Coordinate> defects;

    public List<EnemyData> enemies; 
    public List<EnemyData> bigEnemies;
    public List<EnemyData> animatedEnemies; 
    public List<RespawnData> respawns;
    public List<RespawnLimitData> respawnLimits;

    public List<GoalData> goals;

    public MovesLimitData movesLimitData;

    public List<QBitType> availableColors;

    public List<DebuffData> debuffs;
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


public enum GoalType {CollectGreen, CollectBlue, CollectRed, KillWorms, KillSkeletons, KillZombies, KillAgents, KillSpiders, DestroySkeletonSpawners}

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


public enum DestroyableType {Simple, SkeletonSpawner}

[System.Serializable]
public class DestroyableData {
    public DestroyableType dType;
    public Coordinate point;

    public DestroyableData(DestroyableType dType, Coordinate point) {
        this.dType = dType;
        this.point = point;
    }
}


public enum RespawnType {Simple, AfterGoalsComplete, AfterDeath}

[System.Serializable]
public class RespawnData {
    public RespawnType rType;
    public EnemyType eType;
    public int frequency;
    public bool isAnimated;

    public RespawnData(RespawnType rType, EnemyType eType, int frequency, bool isAnimated) {
        this.rType = rType;
        this.eType = eType;
        this.frequency = frequency;
        this.isAnimated = isAnimated;
    }
}

[System.Serializable]
public class RespawnLimitData {
    public EnemyType eType;
    public int value;

    public RespawnLimitData(EnemyType eType, int value) {
        this.eType = eType;
        this.value = value;
    }
}


[System.Serializable]
public class MovesLimitData {
    public bool isMovesLimited;
    public int movesCount;

    public MovesLimitData(bool isMovesLimited, int movesCount) {
        this.isMovesLimited = isMovesLimited;
        this.movesCount = movesCount;
    }
}


[System.Serializable]
public class DebuffData {
    public DebuffType debuff;
    public Coordinate point_x4;

    public DebuffData(DebuffType debuff, Coordinate point_x4) {
        this.debuff = debuff;
        this.point_x4 = point_x4;
    }
}
