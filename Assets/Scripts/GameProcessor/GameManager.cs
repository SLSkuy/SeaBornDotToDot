using System;
using System.Collections.Generic;
using Cell;
using TMPro;
using UnityEngine;

namespace GameProcessor
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("当前积分")] 
        public int score;
        public TextMeshProUGUI scoreText;

        [Header("回合数据")] 
        public int roundCount = 3;
        public int stepPerRound = 5;
        public TextMeshProUGUI stepText;

        [Header("小丑牌")] 
        public List<DotToDotCell> jokers;
        
        [Header("网格设置")]
        public GridManager gridManager;
        
        private int _currentRound;
        private int _currentStep;

        #region 事件

        public event Action<bool> OnCalculate;
        public event Action OnRoundEmpty;

        #endregion
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            score = 0;
            scoreText.text = "Score: " + 0;
            
            _currentRound = roundCount;
            _currentStep = stepPerRound;
            stepText.text = "Step: " + _currentStep;
            
            gridManager.OnMatch += OnCellMatch;
        }

        private void OnDestroy()
        {
            gridManager.OnMatch -= OnCellMatch;
        }

        private void OnCellMatch(int s)
        {
            // 基础分数计算
            score += s;
            --_currentStep;
            
            // 回合结束处理
            if (_currentStep <= 0)
            {
                OnCalculate?.Invoke(true);
                
                --_currentRound;
                _currentStep = stepPerRound;
                
                CalculateJoker();
            }
            
            // 更新显示逻辑
            scoreText.text = "Score: " + score;
            stepText.text = "Step: " + _currentStep;
        }

        private void CalculateJoker()
        {
            // TODO：处理小丑牌特殊逻辑
            OnCalculate?.Invoke(false);

            if (_currentRound <= 0)
            {
                Debug.Log("回合耗尽，游戏结束");
                OnRoundEmpty?.Invoke();
            }
        }
    }
}