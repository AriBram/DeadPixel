using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameMenu : MenuBase {

    public TMP_Text goCounter;

    public Button goBtn;

    /*Debug features*/
    public Button goNextArenaBtn;


    void Start() {
        goBtn.onClick.AddListener(OnBtnGoClick);
        goNextArenaBtn.onClick.AddListener(OnBtnGoNextArenaClick);

        MovementManager.Instance.onMovementTrackChanged.AddListener(Refresh);

        goBtn.gameObject.SetActive(true);
    }


    public void Refresh() {
        bool isGoBtnActive = GameplayController.Instance.IsPrepare && MovementManager.Instance.ActivatedPoints.Count > 1 ? true : false;
        if(isGoBtnActive)
            goCounter.text = MovementManager.Instance.GetCombinationPower().ToString();
        else
            goCounter.text = "";
    }

    public override void Show() {
        gameObject.SetActive(true);
        Field.Instance.Init();
    }

    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnBackPressed() {}


    public void OnBtnGoClick() {
        if(GameplayController.Instance.IsPrepare && MovementManager.Instance.ActivatedPoints.Count > 1)
            Player.Instance.ActivateMove();
    }

    public void OnBtnGoNextArenaClick() {
        Field.Instance.MoveToNextLevel();
    }
}
