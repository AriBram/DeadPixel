using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shields : MonoBehaviour {

    public int startShields;

    [HideInInspector] public int value;

    public int shieldsLimit;


    void Awake() {
        value = startShields;
    }

    
    public int Use(int damage) {
        int damageRemain = Mathf.Clamp(damage - value, 0, damage);
        value = Mathf.Clamp(value - damage, 0, shieldsLimit);
        return damageRemain;
    }

    public void GetShields(int shieldsCount) {
        value = Mathf.Clamp(value + shieldsCount, 0, shieldsLimit);
    }
}
