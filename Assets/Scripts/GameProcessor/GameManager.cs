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
        
        [Header("组件引用")]
        public GridManager gridManager;
        public SpecialCardManager specialCardManager;
        public MissionManager missionManager;
        public ShopManager shopManager;
        
        public int CurrentRound {get => _currentRound;}
        private int _currentRound;
        private int _currentStep;
        
        private bool _isGameOver;

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
            
            // 任务管理器回调
            missionManager.OnGameOver += GameOver;
            missionManager.OnMissionCompleted += shopManager.GetMoney;
            
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
            missionManager.OnGameOver -= GameOver;
            missionManager.OnMissionCompleted -= shopManager.GetMoney;
            
            gridManager.OnMatch -= OnCellMatch;
            gridManager.OnGridClear -= OnGridClear;
            
            specialCardManager.OnPreCardProcessFinished -= PreCardProcessFinished;
            specialCardManager.OnPostCardProcessFinished -= PostCardProcessFinished;
            
            Signals.Get<ExitShop>().RemoveListener(OnShoppingFinished);
        }

        #region 游戏事件

        public void GameOver()
        {
            Debug.Log("游戏结束");
            
            _isGameOver = true;
            
            OnLockDot?.Invoke();
        }
        
        public void AddRound(int i) => _currentRound += i;

        #endregion
        
        #region 网格事件处理

        private void OnGridClear()
        {
            Debug.Log("网格清空，刷新网格");
            
            OnLockDot?.Invoke();
        }
        
        private void OnCellMatch(int s)
        {
            // 基础分数计算
            score += s;
            --_currentStep;
            
            // 回合结束处理
            if (_currentStep <= 0)
            {
                // 禁用连连看
                OnLockDot?.Invoke();
                
                // 处理卡牌效果
                CalculateSpecialCard();
            }
            
            // 更新分数
            OnStepUpdate?.Invoke(_currentStep);
            OnScoreUpdate?.Invoke(score);
        }

        #endregion

        #region 商店回调

        private void OnShopping()
        {
            if (_isGameOver) return;
            
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
        }

        private void PreCardProcessFinished()
        {
            OnUnlockDot?.Invoke();
        }

        private void PostCardProcessFinished()
        {
            // 回合检测
            missionManager.OnRoundEndCheck();
            
            // 更新回合
            _currentStep = stepPerRound;
            --_currentRound;
            
            // 更新回合数等信息
            OnRoundUpdate?.Invoke(_currentRound);
            OnStepUpdate?.Invoke(_currentStep);

            if (_isGameOver) return;
            
            // 后手卡片处理完毕后开启商店
            OnShopping();
        }

        #endregion
    }
}