using EventProcess;
using UIFramework.Panel;
using UIFramework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class ExitShop : ASignal { }

    public class ShopWindow : PanelController
    {
        [Header("交互按钮")] 
        public Button bargainButton;
        public Button exitButton;

        private void Start()
        {
            exitButton.onClick.AddListener(UI_ExitButton);
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveAllListeners();
        }

        private void UI_ExitButton()
        {
            Signals.Get<ExitShop>().Dispatch();
        }
    }
}