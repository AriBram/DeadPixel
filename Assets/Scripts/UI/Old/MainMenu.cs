using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MenuBase{
    public Button startBtn;


    void Start() {
        startBtn.onClick.AddListener(StartGame);
    }

    public override void Show() {
        gameObject.SetActive(true);
    }

    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnBackPressed() {}

    private void StartGame() {
        UI.Instance.ShowGame();
    }
}
