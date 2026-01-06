using System;
using System.Collections.Generic;
using Cell;
using EventProcess;
using UI.GameSceneUI;
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

        public event Action OnShopTime;
        public event Action OnShopFinished;
        public event Action<int> OnRoundUpdate;
        public event Action<int> OnStepUpdate;
        public event Action<int> OnScoreUpdate;

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
            gridManager.OnGridClear += OnGridClear;
            
            OnScoreUpdate?.Invoke(score);
            OnRoundUpdate?.Invoke(_currentRound);
            OnStepUpdate?.Invoke(_currentStep);
            
            Signals.Get<ExitShop>().AddListener(OnShoppingFinished);
        }

        private void OnDestroy()
        {
            gridManager.OnMatch -= OnCellMatch;
            gridManager.OnGridClear -= OnGridClear;
            
            Signals.Get<ExitShop>().RemoveListener(OnShoppingFinished);
        }

        private void OnGridClear()
        {
            // TODO: 处理网格清空事件
        }

        private void OnCellMatch(int s)
        {
            // 基础分数计算
            score += s;
            --_currentStep;
            
            // 回合结束处理
            if (_currentStep <= 0)
            {
                --_currentRound;
                _currentStep = stepPerRound;
                
                CalculateSpecialCard();
            }
            
            OnScoreUpdate?.Invoke(score);
            OnRoundUpdate?.Invoke(_currentRound);
            OnStepUpdate?.Invoke(_currentStep);
        }

        private void CalculateSpecialCard()
        {
            // TODO：处理卡牌特殊逻辑

            if (_currentRound <= 0)
            {
                Debug.Log("回合耗尽，游戏结束");
            }
            
            OnShopTime?.Invoke();
        }

        private void OnShoppingFinished()
        {
            OnShopFinished?.Invoke();
        }
    }
}