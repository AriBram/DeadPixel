using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour {

    public int startHp;
    public int startMaxHp;

    [HideInInspector] public int hp;
    [HideInInspector] public int maxHp;

    public int hpLimit;

    public class HealthChangedEvent : UnityEvent { }
    [HideInInspector] public HealthChangedEvent onHealthChanged = new HealthChangedEvent();


    void Start() {
        hp = startHp;
        maxHp = startMaxHp;
    }

    
    public void GetDamage(int damage) {
        hp = Mathf.Clamp(hp - damage, 0, maxHp);
        onHealthChanged.Invoke();
    }

    public void GetHeal(int heal) {
        hp = Mathf.Clamp(hp + heal, 0, maxHp);
        onHealthChanged.Invoke();
    }

    public void IncreaseMaxHP(int increaseValue) {
        maxHp = Mathf.Clamp(maxHp + increaseValue, 0, hpLimit);
        onHealthChanged.Invoke();
    }
}
