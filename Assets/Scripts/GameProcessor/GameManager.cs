using System;
using System.Collections.Generic;
using Cell;
using UnityEngine;

namespace GameProcessor
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("当前积分")] 
        public int score;

        [Header("回合数据")] 
        public int roundCount = 3;
        public int stepPerRound = 5;

        [Header("特殊卡牌")] 
        public List<Card> specialCards;
        
        [Header("网格设置")]
        public GridManager gridManager;
        
        private int _currentRound;
        private int _currentStep;

        #region 事件

        public event Action<bool> OnCalculate;
        public event Action<int> OnRoundUpdate;
        public event Action<int> OnStepUpdate;

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
            
            _currentRound = roundCount;
            _currentStep = stepPerRound;
            
            gridManager.OnMatch += OnCellMatch;
            
            OnRoundUpdate?.Invoke(_currentRound);
            OnStepUpdate?.Invoke(_currentStep);
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
                
                CalculateSpecialCard();
            }
            
            OnRoundUpdate?.Invoke(_currentRound);
            OnStepUpdate?.Invoke(_currentStep);
        }

        private void CalculateSpecialCard()
        {
            // TODO：处理小丑牌特殊逻辑
            OnCalculate?.Invoke(false);

            if (_currentRound <= 0)
            {
                Debug.Log("回合耗尽，游戏结束");
            }
        }
    }
}