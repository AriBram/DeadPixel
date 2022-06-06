using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthBar : MonoBehaviour {
    
    public List<GameObject> healthPoints;
    public List<GameObject> shields;


    void Start() {
        Player.Instance.onAwake.AddListener(Refresh);
        Player.Instance.health.onHealthChanged.AddListener(Refresh);
    }

    
    public void Refresh() {
        Clear();

        int playerHp = Player.Instance.health.hp;
        int shieldsCount = Player.Instance.health.shields.value;

        for(int i = 0; i < playerHp; i++)
            healthPoints[i].SetActive(true);

        for(int i = 0; i < shieldsCount; i++)
            shields[i].SetActive(true);
    }


    void Clear() {
        foreach(var hp in healthPoints)
            hp.SetActive(false);

        foreach(var sh in shields)
            sh.SetActive(false);
    }
}
