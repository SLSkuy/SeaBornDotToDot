using System;
using TMPro;
using UnityEngine;

namespace GameProcessor
{
    /// <summary>
    /// 回合任务管理器
    /// 决定每回合需要消除的对数，并处理奖励与失败逻辑
    /// </summary>
    public class MissionManager : MonoBehaviour
    {
        [Header("任务基础配置")]
        [Tooltip("第1回合需要消除的对数")]
        public int basePairs = 3;
        public TextMeshProUGUI missionText;

        [Tooltip("每回合递增的消除对数")]
        public int increasePerRound = 1;

        [Header("奖励配置")]
        public int rewardGold = 10;
        public int goldPerExtraStep = 1;
        public int rewardExtraRound = 1;

        private int _currentTargetPairs;
        private int _currentCompletedPairs;
        private int _lastFailedPairs;

        private int _currentRound;

        private GameManager _game;

        #region 事件
        
        public event Action<int, int> OnMissionProgress;
        public event Action<int> OnMissionCompleted;
        public event Action OnGameOver;

        #endregion

        private void Start()
        {
            _game = GameManager.Instance;

            _game.gridManager.OnMatch += OnMatch;
            _game.OnRoundUpdate += OnRoundStart;
            _game.OnPostSpecialCard += OnRoundEndCheck;

            InitMission();
        }

        private void OnDestroy()
        {
            _game.gridManager.OnMatch -= OnMatch;
            _game.OnRoundUpdate -= OnRoundStart;
            _game.OnPostSpecialCard -= OnRoundEndCheck;
        }

        private void UpdateMissionText()
        {
            missionText.text = _currentCompletedPairs + " / " + _currentTargetPairs;
        }

        #region 任务流程

        /// <summary>
        /// 初始化或进入新回合任务
        /// </summary>
        private void InitMission()
        {
            ++_currentRound;
            
            _currentTargetPairs = basePairs + _currentRound * increasePerRound + _lastFailedPairs;
            _currentCompletedPairs = 0;

            OnMissionProgress?.Invoke(_currentCompletedPairs, _currentTargetPairs);
            
            UpdateMissionText();
        }

        /// <summary>
        /// 每次成功消除一对
        /// </summary>
        private void OnMatch(int score)
        {
            _currentCompletedPairs++;
            OnMissionProgress?.Invoke(_currentCompletedPairs, _currentTargetPairs);
            
            UpdateMissionText();
        }

        /// <summary>
        /// 新回合开始（回合数变化时）
        /// </summary>
        private void OnRoundStart(int roundLeft)
        {
            if (roundLeft < 0) return;

            InitMission();
        }

        /// <summary>
        /// 回合结束后检查任务状态
        /// </summary>
        private void OnRoundEndCheck()
        {
            if (_currentCompletedPairs >= _currentTargetPairs)
            {
                HandleMissionSuccess();
            }
            else
            {
                HandleMissionFail();
            }
        }

        #endregion

        #region 结果处理

        private void HandleMissionSuccess()
        {
            Debug.Log("【任务完成】");

            int reward = 0;

            // 基础奖励
            reward += rewardGold;

            // 消除多余海嗣转换为金币
            int extraGold = (_currentCompletedPairs-_currentTargetPairs) * goldPerExtraStep;
            reward += extraGold;

            // 奖励回合
            _game.AddRound(rewardExtraRound);

            OnMissionCompleted?.Invoke(reward);
        }

        private void HandleMissionFail()
        {
            Debug.Log("【任务未完成，进度延续】");

            // 如果回合耗尽 -> 游戏结束
            if (_game.CurrentRound <= 0)
            {
                Debug.Log("任务失败，游戏结束");
                OnGameOver?.Invoke();
            }
            // 否则：不清进度，继续累加
            
            _lastFailedPairs = _currentTargetPairs - _currentCompletedPairs;
        }

        #endregion
    }
}
