using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour {

    public List<MovementPoint> Points = new List<MovementPoint>();
    public List<MovementPoint> ActivatedPoints = new List<MovementPoint>();

    public static MovementManager Instance { get; private set; }
    
    void Start() {
        Instance = this;

        GameObject[] pointsGameObjects = GameObject.FindGameObjectsWithTag("MovementPoint");

        foreach(GameObject point in pointsGameObjects)
            Points.Add(point.GetComponent<MovementPoint>());
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
