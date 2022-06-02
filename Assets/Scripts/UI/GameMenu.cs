using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameMenu : MenuBase {

    public Button goBtn;
    public Button initFieldBtn;


    void Start() {
        goBtn.onClick.AddListener(OnBtnGoClick);
        initFieldBtn.onClick.AddListener(OnBtnInitClick);
    }

    void Update() {
        goBtn.gameObject.SetActive(GameplayController.Instance.IsPrepare && MovementManager.Instance.ActivatedPoints.Count > 0);
    }

    public override void Show() {
        gameObject.SetActive(true);
        //Field.Instance.Init();
    }

    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnBackPressed() {}


    public void OnBtnGoClick() {
        Player.Instance.ActivateMove();
    }

    public void OnBtnInitClick() {
        Field.Instance.Init();
    }
}
