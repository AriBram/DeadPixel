using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MemoryBar : MonoBehaviour {
    
    public List<GameObject> activePoints;
    public List<GameObject> passivePoints;

    public TMP_Text caption;


    void Start() {
        Player.Instance.onAwake.AddListener(Refresh);
        Player.Instance.memory.onMemoryChanged.AddListener(Refresh);
    }

    
    public void Refresh() {
        Clear();

        int val = Player.Instance.memory.value;
        for(int i = 0; i < val; i++)
            activePoints[i].SetActive(true);

        int maxVal = Player.Instance.memory.maxValue;
        for(int i = 0; i < maxVal; i++)
            passivePoints[i].SetActive(true);

        caption.text = val.ToString();
    }


    void Clear() {
        foreach(var p in activePoints)
            p.SetActive(false);

        foreach(var p in passivePoints)
            p.SetActive(false);
    }
}

