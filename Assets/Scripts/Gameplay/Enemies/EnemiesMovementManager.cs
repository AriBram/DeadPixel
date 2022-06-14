using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovementManager : MonoBehaviour {

    public List<Coordinate> busyPoints = new List<Coordinate>();

    private int nextInQueue;

    public static EnemiesMovementManager Instance { get; private set; }
    
    
    void Awake() {
        Instance = this;
        Init();
    }

    public void Init() {
        nextInQueue = 0;
    }


    public bool IsPointBusy(Coordinate point) {
        return busyPoints.Find(p => p.x == point.x && p.y == point.y) != null;
    }

    public void Reset() {
        busyPoints = new List<Coordinate>();
    }
    
    public void AddBusyPoint(Coordinate point) {
        busyPoints.Add(point);
    }


    public void SetQueue(List<Enemy> simpleEnemies, List<BigEnemy> bigEnemies) {
        List<EnemyBase> enemies = new List<EnemyBase>();
        foreach(var e in simpleEnemies)
            enemies.Add(e);
        foreach(var e in bigEnemies)
            enemies.Add(e);


        int queueSize = enemies.Count - 1;
        if(nextInQueue > queueSize)
            nextInQueue = 0;

        if(enemies.Count <= Field.Instance.maxEnemiesCanMove) {
            foreach(var e in enemies)
                e.SetCanMove(true);
        }
        else {
            foreach(var e in enemies)
                e.SetCanMove(false);

            enemies[nextInQueue].SetCanMove(true);
            MoveQueue(queueSize);
            enemies[nextInQueue].SetCanMove(true);
            MoveQueue(queueSize);
        }
    }

    private void MoveQueue(int qSize) {
        if( (nextInQueue + 1) > qSize )
            nextInQueue = 0;
        else
            nextInQueue++;
    }
}