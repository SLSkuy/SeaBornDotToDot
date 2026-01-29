using GameProcessor;
using UIFramework.Panel;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class GameOverPanel : PanelController
    {
        public Button exitButton;
        public Text scoreText;

        private void Start()
        {
            exitButton.onClick.AddListener(()=>GameManager.Instance.UI_ExitGame());
        }

        private void OnEnable()
        {
            if(GameManager.Instance != null)
                scoreText.text = "最终分数：" + GameManager.Instance.score;
        }
    }
}
