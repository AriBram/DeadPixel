using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI : MonoBehaviour {

    public MainMenu mainMenu;
    public GameMenu gameMenu;

    public SkillPopup skillPopup;

    public List<Image> colorableUIitems;

    Stack<MenuBase> menuStack = new Stack<MenuBase>();

    public static UI Instance { get; private set; }


    void Start() {
        Instance = this;

        Player.Instance.onAwake.AddListener(StartUI);

        //Restart();
    }

    void Update() {}



    public void ShowGame() {
        HideAll();
        PushMenu(gameMenu);
    }

    public void ShowMainMenu() {
        HideAll();
        PushMenu(mainMenu);
    }


    public void ShowSkillPopup(SkillType sType) {
        skillPopup.Init(sType);
        PushMenu(skillPopup);
    }


    public void PushMenu(MenuBase menu) {
        menuStack.Push(menu);
        menu.Show();
    }

    public void PopMenu() {
        if (menuStack.Count > 0)
            menuStack.Pop().Hide();
        else
            print("There is nothing to close");
    }

    void HideAll() {
        while (menuStack.Count > 0)
            PopMenu();
    }

    public bool IsGameMenu() {
        if(menuStack.Peek() == gameMenu)
            return true;
        return false;
    }

    public bool IsMainMenu() {
        if(menuStack.Peek() == mainMenu)
            return true;
        return false;
    }


    public void Restart() {
        HideAll();
        PushMenu(mainMenu);
    }


    public void StartUI() {
        gameMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        Restart();
    }



    public void ChangeUIColor(QBitType qType) {
        Color newColor = GameData.Instance.qBits.Find(q => q.qType == qType).color;
        foreach(var uiItem in colorableUIitems)
            uiItem.color = newColor;
    }
}

public abstract class MenuBase : MonoBehaviour {
    public abstract void Show();
    public abstract void Hide();
    public abstract void OnBackPressed();
}

