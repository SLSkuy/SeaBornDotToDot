using GameProcessor;
using TMPro;
using UIFramework.Panel;
using UnityEngine;

namespace UI.GameSceneUI
{
    public class ScorePanel : PanelController
    {
        [Header("积分属性")]
        public TextMeshProUGUI scoreText;
        
        [Header("回合属性")]
        public TextMeshProUGUI roundText;
        public TextMeshProUGUI stepText;

        #region 周期函数

        private void Start()
        {
            GameManager.Instance.OnScoreUpdate += UpdateScore;
            GameManager.Instance.OnRoundUpdate += RoundUpdate;
            GameManager.Instance.OnStepUpdate += StepUpdate;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnScoreUpdate -= UpdateScore;
            GameManager.Instance.OnRoundUpdate -= RoundUpdate;
            GameManager.Instance.OnStepUpdate -= StepUpdate;
        }

        #endregion
        
        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }

        private void RoundUpdate(int round)
        {
            roundText.text = round.ToString();
        }

        private void StepUpdate(int step)
        {
            stepText.text = step.ToString();
        }
    }
}