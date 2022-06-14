using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

    public List<MovementPoint> Points = new List<MovementPoint>();
    public List<MovementPoint> ActivatedPoints = new List<MovementPoint>();

    public List<Point_x4> Points_x4 = new List<Point_x4>();

    public static MovementManager Instance { get; private set; }
    
    void Awake() {
        Instance = this;

        GameObject[] pointsGameObjects = GameObject.FindGameObjectsWithTag("MovementPoint");

        foreach(GameObject point in pointsGameObjects)
            Points.Add(point.GetComponent<MovementPoint>());


        GameObject[] points_x4_GameObjects = GameObject.FindGameObjectsWithTag("Point_x4");

        foreach(GameObject point in points_x4_GameObjects)
            Points_x4.Add(point.GetComponent<Point_x4>());
    }


    public void ClearMovementTrack() {
        ActivatedPoints.Clear();
    }

    public void SetMovementTrack(List<MovementPoint> points) {
        ClearMovementTrack();

        foreach(var point in points)
            ActivatedPoints.Add(point);
    }
}
