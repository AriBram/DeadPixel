using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovementManager : MonoBehaviour {

    public List<Coordinate> busyPoints = new List<Coordinate>();

    public static EnemiesMovementManager Instance { get; private set; }
    
    
    void Awake() {
        Instance = this;
    }


    public bool IsPointBusy(Coordinate point) {
        //Debug.Log(busyPoints.Contains(point));
        Debug.Log(busyPoints.Count);
        //return busyPoints.Find(p => p.x == point.x && p.y == point.y) != null;
        return busyPoints.Find(p => p.x == point.x && p.y == point.y) != null;
    }

    public void Reset() {
        busyPoints = new List<Coordinate>();
    }
    
    public void AddBusyPoint(Coordinate point) {
        busyPoints.Add(point);
    }
}