using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene(1);
        }
    }
}

