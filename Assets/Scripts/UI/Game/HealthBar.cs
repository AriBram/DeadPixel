using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthBar : MonoBehaviour {
    
    public List<GameObject> healthPoints;
    public List<GameObject> shields;

    public TMP_Text hpCaption;
    public Image icon;

    public Sprite heartIcon;
    public Sprite shieldIcon;


    void Start() {
        Player.Instance.onAwake.AddListener(Refresh);
        Player.Instance.health.onHealthChanged.AddListener(Refresh);
    }

    
    public void Refresh() {
        Clear();

        int playerHp = Player.Instance.health.hp;
        int shieldsCount = Player.Instance.health.shields.value;

        icon.sprite = shieldsCount > 0 ? shieldIcon : heartIcon;

        for(int i = 0; i < playerHp; i++)
            healthPoints[i].SetActive(true);

        for(int i = 0; i < shieldsCount; i++)
            shields[i].SetActive(true);

        hpCaption.text = playerHp.ToString();
    }


    void Clear() {
        foreach(var hp in healthPoints)
            hp.SetActive(false);

        foreach(var sh in shields)
            sh.SetActive(false);
    }
}
