using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Memory : MonoBehaviour {

    public int startVaue;
    public int startMaxValue;

    [HideInInspector] public int value;
    [HideInInspector] public int maxValue;

    public int limit;

    public class MemoryChangedEvent : UnityEvent { }
    [HideInInspector] public MemoryChangedEvent onMemoryChanged = new MemoryChangedEvent();


    void Awake() {
        value = startVaue;
        maxValue = startMaxValue;
    }

    
    public void Use(int count) {
        value = Mathf.Clamp(value - count, 0, maxValue);
        onMemoryChanged.Invoke();
    }

    public void Get(int count) {
        value = Mathf.Clamp(value + count, 0, maxValue);
        onMemoryChanged.Invoke();
    }

    public void IncreaseMaxValue(int increaseValue) {
        maxValue = Mathf.Clamp(maxValue + increaseValue, 0, limit);
        onMemoryChanged.Invoke();
    }
}

