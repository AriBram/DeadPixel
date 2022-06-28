using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrabStuff;

namespace DeadPixel
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField]
        private Button startBtn;

        void Start()
        {
            startBtn.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            Loader.Load(1); //Loading Gameplay scene 
        }
    }
}

