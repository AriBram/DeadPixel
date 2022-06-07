using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalsBar : MonoBehaviour {

    public Transform goalsContainer;
    public GameObject goalPrefab;

    public List<GameObject> goalsLinks;
    public List<Goal> goals;

    
    void Awake() {
        Field.Instance.onFieldInit.AddListener(Init);
        Field.Instance.goals.onGoalsRefresh.AddListener(Refresh);
    }


    public void Init() {
        Clear();

        foreach(var g in Field.Instance.goals.activeGoals) {
            var item = Instantiate(goalPrefab, goalsContainer);
            goalsLinks.Add(item);
            Goal goal = item.GetComponent<Goal>();
            goal.Init(g);
            goals.Add(goal);
        }
    }


    public void Refresh() {
        foreach(var g in goals)
            g.Refresh();
    }


    void Clear() {
        foreach(var g in goalsLinks)
            Destroy(g);

        goalsLinks.Clear();
        goals.Clear();
    }
}
