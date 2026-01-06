using System;
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
        
        [Header("网格设置")]
        public GridManager gridManager;
        public SpecialCardManager specialCardManager;
        
        private int _currentRound;
        private int _currentStep;

        #region 事件

        public event Action OnPreSpecialCard;
        public event Action OnPostSpecialCard;
        
        public event Action OnShopTime;

        public event Action OnLockDot;
        public event Action OnUnlockDot;
        
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
            
            // 网格事件回调
            gridManager.OnMatch += OnCellMatch;
            gridManager.OnGridClear += OnGridClear;

            // 特殊卡牌处理回调
            specialCardManager.OnPreCardProcessFinished += PreCardProcessFinished;
            specialCardManager.OnPostCardProcessFinished += PostCardProcessFinished;
            
            // 数据更新
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
        
        #region 网格事件处理

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

        #endregion

        #region 商店回调

        private void OnShopping()
        {
            // 禁用连连看
            OnLockDot?.Invoke();
            OnShopTime?.Invoke();
        }

        private void OnShoppingFinished()
        {
            // 处理先机特殊卡片效果
            OnPreSpecialCard?.Invoke();
        }

        #endregion

        #region 卡牌处理回调
        
        private void CalculateSpecialCard()
        {
            // 处理后手特殊卡片效果
            OnPostSpecialCard?.Invoke();

            if (_currentRound <= 0)
            {
                Debug.Log("回合耗尽，游戏结束");
            }
        }

        private void PreCardProcessFinished()
        {
            OnUnlockDot?.Invoke();
        }

        private void PostCardProcessFinished()
        {
            // 后手卡片处理完毕后开启商店
            OnShopping();
        }

        #endregion
    }
}