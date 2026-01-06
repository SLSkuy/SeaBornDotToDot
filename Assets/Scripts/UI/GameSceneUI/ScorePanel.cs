using System;
using EventProcess;
using TMPro;
using UIFramework.Panel;
using UnityEngine;

namespace UI.GameSceneUI
{
    public class ScorePanelScoreUpdate : ASignal<int> { }

    public class ScorePanel : PanelController
    {
        [Header("积分")]
        public TextMeshProUGUI scoreText;

        private void Start()
        {
            Signals.Get<ScorePanelScoreUpdate>().AddListener(UpdateScore);
        }

        private void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}