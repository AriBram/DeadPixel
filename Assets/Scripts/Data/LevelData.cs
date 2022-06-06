using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData {
    public Coordinate player;
    public List<Coordinate> obstacles;
    public List<Coordinate> destroyables;

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

    public GoalData(GoalType gType, int value) {
        this.gType = gType;
        this.value = value;
    }
}
