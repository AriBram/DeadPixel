using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeadPixel
{
    public class UI_Logic : MonoBehaviour
    {
        public GameMenu gameMenu;
        public SkillPopup skillPopup;
        public List<Image> colorItems;

        public static UI_Logic Instance { get; private set; }

        void Start()
        {
            Instance = this;
            //Player.Instance.onAwake.AddListener(StartUI);
            gameMenu.Show();
        }

        public void ChangeUIColor(QBitType qType)
        {
            QBitData qbit = GameData.Instance.qBits.Find(q => q.qType == qType);
            if (qbit == null)
                return;

            Color newColor = qbit.color;
            foreach (var uiItem in colorItems)
                uiItem.color = newColor;
        }
    }

    public abstract class MenuBase : MonoBehaviour
    {
        public abstract void Show();
        public abstract void Hide();
        public abstract void OnBackPressed();
    }
}

