using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class MovementManager : MonoBehaviour {

    public List<MovementPoint> Points = new List<MovementPoint>();
    public List<MovementPoint> ActivatedPoints = new List<MovementPoint>();

    public List<Point_x4> Points_x4 = new List<Point_x4>();

    public static MovementManager Instance { get; private set; }

    public class MovementTrackChangedEvent : UnityEvent { }
    [HideInInspector] public MovementTrackChangedEvent onMovementTrackChanged = new MovementTrackChangedEvent();

    
    void Awake() {
        Instance = this;

        GameObject[] pointsGameObjects = GameObject.FindGameObjectsWithTag("MovementPoint");

        foreach(GameObject p in pointsGameObjects) {
            MovementPoint point = p.GetComponent<MovementPoint>();
            if(!point.isEscapeFromLevel)
                Points.Add(point);
        }

        GameObject[] points_x4_GameObjects = GameObject.FindGameObjectsWithTag("Point_x4");

        foreach(GameObject point in points_x4_GameObjects)
            Points_x4.Add(point.GetComponent<Point_x4>());
    }


    public void ClearMovementTrack() {
        ActivatedPoints.Clear();
        onMovementTrackChanged.Invoke();
    }

    public void SetMovementTrack(List<MovementPoint> points) {
        ClearMovementTrack();

        foreach(var point in points)
            ActivatedPoints.Add(point);

        onMovementTrackChanged.Invoke();
    }

    public int GetCombinationPower() {
        int power = 0;
        foreach(var p in ActivatedPoints) {
            if(p.data == PointData.None || p.isQbit || p.isQuant) //bug
                power++;
        }
        return power;
    }
}
