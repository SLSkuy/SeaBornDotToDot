using GameProcessor;
using UIFramework.Panel;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class GameOverPanel : PanelController
    {
        public Button exitButton;

        private void Start()
        {
            exitButton.onClick.AddListener(()=>GameManager.Instance.UI_ExitGame());
        }
    }
}
