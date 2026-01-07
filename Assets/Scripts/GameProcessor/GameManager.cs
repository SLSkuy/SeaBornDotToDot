using System;
using System.Collections;
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
        public float roundTime = 30f;   // 每回合秒数
        private float _currentTime;
        private int _lastSecond;
        private bool _isTiming;
        
        [Header("组件引用")]
        public GridManager gridManager;
        public SpecialCardManager specialCardManager;
        public MissionManager missionManager;
        public ShopManager shopManager;
        
        public int CurrentRound => _currentRound;
        private int _currentRound;
        
        private bool _isGameOver;

        #region 事件

        public event Action OnPreSpecialCard;
        public event Action OnPostSpecialCard;

        public event Action<string> OnRoundChange;
        
        public event Action OnShopTime;

        public event Action OnLockDot;
        public event Action OnUnlockDot;
        
        public event Action<int> OnRoundUpdate;
        public event Action<int> OnTimeUpdate;
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
            
            // 初始化计时器
            _currentRound = roundCount;
            _isTiming = true;
            _currentTime = roundTime;
            _lastSecond = Mathf.CeilToInt(_currentTime);
            
            // 任务管理器回调
            missionManager.OnGameOver += GameOver;
            missionManager.OnMissionCompleted += shopManager.GetMoney;
            
            // 网格事件回调
            gridManager.OnMatch += OnCellMatch;
            gridManager.OnBomb += OnCellBomb;
            gridManager.OnGridClear += OnGridClear;

            // 特殊卡牌处理回调
            specialCardManager.OnPreCardProcessFinished += PreCardProcessFinished;
            specialCardManager.OnPostCardProcessFinished += PostCardProcessFinished;
            
            // 数据更新
            OnScoreUpdate?.Invoke(score);
            OnRoundUpdate?.Invoke(_currentRound);
            OnTimeUpdate?.Invoke(_lastSecond);

            AudioManager.Instance.PlayMainBGM();
            
            Signals.Get<ExitShop>().AddListener(OnShoppingFinished);
        }

        private void OnDestroy()
        {
            missionManager.OnGameOver -= GameOver;
            missionManager.OnMissionCompleted -= shopManager.GetMoney;
            
            gridManager.OnMatch -= OnCellMatch;
            gridManager.OnBomb -= OnCellBomb;
            gridManager.OnGridClear -= OnGridClear;
            
            specialCardManager.OnPreCardProcessFinished -= PreCardProcessFinished;
            specialCardManager.OnPostCardProcessFinished -= PostCardProcessFinished;
            
            Signals.Get<ExitShop>().RemoveListener(OnShoppingFinished);
        }
        
        private void Update()
        {
            if (_isGameOver || !_isTiming) return;

            _currentTime -= Time.deltaTime;
            if (_currentTime < 0)
                _currentTime = 0;

            int currentSecond = Mathf.CeilToInt(_currentTime);

            if (currentSecond != _lastSecond)
            {
                _lastSecond = currentSecond;
                OnTimeUpdate?.Invoke(currentSecond);
            }

            if (_currentTime <= 0)
            {
                EndRoundByTime();
            }
        }

        public void UI_ExitGame()
        {
            SceneLoader.LoadScene("MainScene");
        }

        #region 游戏事件
        
        private void EndRoundByTime()
        {
            _isTiming = false;

            // 禁用连连看
            OnLockDot?.Invoke();
            OnRoundChange?.Invoke("回合结束");

            // 进入后手卡牌处理
            CalculateSpecialCard();
        }

        public void GameOver()
        {
            Debug.Log("游戏结束");
            
            _isGameOver = true;
            
            OnLockDot?.Invoke();
        }
        
        public void AddRound(int i) => _currentRound += i;
        public void AddTime(int i)
        {
            _currentTime += i;
            OnTimeUpdate?.Invoke(Mathf.CeilToInt(_currentTime));
        }

        #endregion
        
        #region 网格事件处理

        private void OnGridClear()
        {
            Debug.Log("网格清空，刷新网格");
            
            OnLockDot?.Invoke();

            StartCoroutine(GenerateGridAnimation());
        }

        private IEnumerator GenerateGridAnimation()
        {
            yield return new WaitForSeconds(0.2f);
            
            gridManager.FillCells();
            
            OnUnlockDot?.Invoke();
        }
        
        private void OnCellMatch(int s, int pairs)
        {
            score += s;
            OnScoreUpdate?.Invoke(score);
        }
        
        private void OnCellBomb(int s, int pairs)
        {
            score += s;
            
            OnScoreUpdate?.Invoke(score);
        }

        #endregion

        #region 商店回调

        private void OnShopping()
        {
            if (_isGameOver) return;
            
            OnShopTime?.Invoke();
            AudioManager.Instance.PlayShopBGM();
        }

        private void OnShoppingFinished()
        {
            // 先机卡
            OnPreSpecialCard?.Invoke();
            AudioManager.Instance.PlayMainBGM();
            
            // 重置时间
            _currentTime = roundTime;
            OnTimeUpdate?.Invoke((int)_currentTime);
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
            // 开始计时
            _isTiming = true;
            OnUnlockDot?.Invoke();
            OnRoundChange?.Invoke("回合开始");
        }

        private void PostCardProcessFinished()
        {
            missionManager.OnRoundEndCheck();

            --_currentRound;
            OnRoundUpdate?.Invoke(_currentRound);

            if (_isGameOver) return;

            OnShopping();
        }


        #endregion
    }
}