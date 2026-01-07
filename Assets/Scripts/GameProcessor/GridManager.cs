using System;
using System.Collections.Generic;
using UnityEngine;
using CellCard;
using Random = UnityEngine.Random;

namespace GameProcessor
{
    public class GridManager : MonoBehaviour
    {
        [Header("网格设置")]
        public Grid grid;
        public Vector2Int gridSize;
        public Vector2 cellSize;
        public Vector2 cellGap;
        private Vector2Int _logicGridSize;

        [Header("元素设置")]
        public Card cellPrefab;
        public Sprite[] sprites;
        
        [Header("溟痕设置")]
        public Sprite sealedFloor;
        public Material sealedFloorMat;
        
        [Tooltip("每多少行动值扩张一次")]public int expendRate = 15;
        private int _lastExpend;
        private readonly HashSet<Vector2Int> _sealedCells = new();
        
        [Header("输入设置")]
        public Camera cam;
        public LayerMask cellLayer;

        public Card[,] Cells => _cells;
        private Card[,] _cells;

        private int _curMatchCount;
        private int _maxMatchCount;

        private bool _isLockByLogicProcess;
        
        // 组件引用
        private MatchManager _matchManager;

        #region 事件

        public event Action<int, int> OnMatch;  // score, pairs
        public event Action<int, int> OnBomb;   // score, pairs
        public event Action OnGridClear;

        #endregion

        private void Awake()
        {
            _matchManager = new MatchManager(this, GetComponent<LinkVisualizer>());
        }

        private void Start()
        {
            if (gridSize.x % 2 != 0 && gridSize.y % 2 != 0)
            {
                Debug.LogWarning("网格宽高需要偶数才能生成成对元素！");
                return;
            }
            
            // 初始化网格数据
            grid.cellSize = cellSize;
            grid.cellGap = cellGap;
            _isLockByLogicProcess = false;
            
            // 初始化逻辑数据，以让边缘元素能够相连
            _logicGridSize = new Vector2Int(gridSize.x + 2, gridSize.y + 2);
            _maxMatchCount = gridSize.x * gridSize.y;
            
            _matchManager.OnCellMatch += OnCellMatch;
            _matchManager.OnNoneAvailableMatches += ShuffleRemainingCells;  // 若没有可连接元素，刷新一次

            GameManager.Instance.OnLockDot += OnLock;
            GameManager.Instance.OnUnlockDot += OnUnlock;
            GameManager.Instance.OnTimeUpdate += SealedAction;
            
            GenerateGrid();
        }

        private void OnLock() => _isLockByLogicProcess = true;  // 购物时禁用连连看功能
        private void OnUnlock() => _isLockByLogicProcess = false;  // 处理先机卡牌后开启连连看功能
        
        private void OnDestroy()
        {
            _matchManager.OnCellMatch -= OnCellMatch;
            _matchManager.OnNoneAvailableMatches -= ShuffleRemainingCells;
            
            GameManager.Instance.OnLockDot -= OnLock;
            GameManager.Instance.OnUnlockDot -= OnUnlock;
            GameManager.Instance.OnTimeUpdate -= SealedAction;
        }

        private void Update()
        {
            if (!_isLockByLogicProcess && Input.GetMouseButtonDown(0))
            {
                DetectCellClick();
            }
        }
        
        private bool IsInsideGrid(int x, int y)
        {
            return x > 0 && x <= gridSize.x && y > 0 && y <= gridSize.y;
        }

        private void OnCellMatch(CardType cardType)
        {
            _curMatchCount += 2;
            
            OnMatch?.Invoke((int)cardType + 1, 1);
            
            if (_curMatchCount == _maxMatchCount)
            {
                Debug.Log("消除完毕");
                
                _curMatchCount = 0;
                OnGridClear?.Invoke();
            }
        }
        
        /// <summary>
        /// 溟痕逻辑
        /// </summary>
        private void SealedAction(int time)
        {
            _lastExpend++;

            if (_lastExpend % expendRate == 0)
            {
                SetSealedCellExpend();
            }
        }
        
        /// <summary>
        /// 选择检测
        /// </summary>
        private void DetectCellClick()
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 rayOrigin = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.zero,
                0f,
                cellLayer
            );

            if (!hit) return;

            Card cell = hit.collider.GetComponent<Card>();
            if (!cell || cell.IsEmpty || cell.isSealedFloor) return;

            _matchManager.OnCellClicked(cell);
        }

        /// <summary>
        /// 置中网格
        /// </summary>
        private void CenterGrid()
        {
            Vector3 gap = grid.cellGap;

            float boardWidth = _logicGridSize.x * cellSize.x;
            float boardHeight = _logicGridSize.y * cellSize.y;
            float gapWidth = _logicGridSize.x * gap.x;
            float gapHeight = _logicGridSize.y * gap.y;

            Vector3 offset = new Vector3(
                (boardWidth + gapWidth) / 2f,
                (boardHeight + gapHeight) / 2f,
                0f
            );

            grid.transform.localPosition = -offset;
        }

        /// <summary>
        /// 生成网格
        /// </summary>
        private void GenerateGrid()
        {
            CenterGrid();
            _cells = new Card[_logicGridSize.x, _logicGridSize.y];

            for (int x = 0; x < _logicGridSize.x; x++)
            {
                for (int y = 0; y < _logicGridSize.y; y++)
                {
                    Vector3 worldPos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    Card cell = Instantiate(cellPrefab, worldPos, Quaternion.identity, grid.transform);
                    cell.transform.localScale = new Vector3(grid.cellSize.x, grid.cellSize.y, 1f);

                    cell.x = x;
                    cell.y = y;

                    // 初始化设置元素为空
                    cell.Clear();

                    _cells[x, y] = cell;
                }
            }

            // 清空溟痕
            _sealedCells.Clear();
            FillCells(); // 只填中间
        }
        
        /// <summary>
        /// 填充元素进网格
        /// </summary>
        public void FillCells()
        {
            int total = gridSize.x * gridSize.y;
            int pairCount = total / 2;
            List<CardType> types = new List<CardType>();

            for (int i = 0; i < pairCount; i++)
            {
                CardType type = (CardType)Random.Range(0, sprites.Length);
                types.Add(type);
                types.Add(type);
            }

            // 洗牌
            for (int i = 0; i < types.Count; i++)
            {
                int rand = Random.Range(i, types.Count);
                (types[i], types[rand]) = (types[rand], types[i]);
            }

            int index = 0;

            // 跳过边界
            for (int x = 1; x <= gridSize.x; x++)
            {
                for (int y = 1; y <= gridSize.y; y++)
                {
                    CardType type = types[index++];
                    _cells[x, y].Set(type, sprites[(int)type]);
                }
            }
        }

        #region 特殊效果

        public void RecordSealedCell(int x, int y)
        {
            _sealedCells.Add(new Vector2Int(x, y));
        }

        private void SetSealedCellExpend()
        {
            List<Vector2Int> newSealed = new();

            foreach (var pos in _sealedCells)
            {
                TrySeal(pos.x + 1, pos.y, newSealed);
                TrySeal(pos.x - 1, pos.y, newSealed);
                TrySeal(pos.x, pos.y + 1, newSealed);
                TrySeal(pos.x, pos.y - 1, newSealed);
            }

            // 统一转化，避免同一轮连锁扩散
            foreach (var p in newSealed)
            {
                Card cell = _cells[p.x, p.y];
                if (!cell || cell.isSealedFloor) continue;
                
                cell.SetSealedFloor();

                _sealedCells.Add(p);
            }
        }
        
        private void TrySeal(int x, int y, List<Vector2Int> result)
        {
            if (!IsInsideGrid(x, y)) return;

            Card cell = _cells[x, y];
            if (!cell) return;

            if (cell.IsEmpty && !cell.isSealedFloor)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!result.Contains(pos) && !_sealedCells.Contains(pos))
                {
                    result.Add(pos);
                }
            }
        }
        
        public void ClearSealedRows(List<int> rows)
        {
            foreach (int row in rows)
            {
                for (int x = 1; x <= gridSize.x; x++)
                {
                    Card cell = _cells[x, row];
                    if (cell && cell.isSealedFloor)
                    {
                        cell.ClearSealedFloor();
                        _sealedCells.Remove(new Vector2Int(x, row));
                    }
                }
            }
        }
        
        /// <summary>
        /// 刷新替换当前未被消除元素位置
        /// </summary>
        public void ShuffleRemainingCells()
        {
            List<Card> remainingCells = new List<Card>();
            List<CardType> remainingTypes = new List<CardType>();

            foreach (var cell in _cells)
                if (!cell.IsEmpty && !cell.isSealedFloor)
                {
                    remainingCells.Add(cell);
                    remainingTypes.Add(cell.type);
                }

            if (remainingTypes.Count == 0) return;

            // 洗牌
            for (int i = 0; i < remainingTypes.Count; i++)
            {
                int rand = Random.Range(i, remainingTypes.Count);
                (remainingTypes[i], remainingTypes[rand]) = (remainingTypes[rand], remainingTypes[i]);
            }

            for (int i = 0; i < remainingCells.Count; i++)
            {
                remainingCells[i].Set(remainingTypes[i], sprites[(int)remainingTypes[i]]);
            }
        }

        /// <summary>
        /// 在指定位置进行爆炸消除
        /// </summary>
        public void BombAt(int centerX, int centerY, int radius)
        {
            HashSet<Card> removed = new HashSet<Card>();
            int score = 0;
            int pairs = 0;

            // 1. 范围爆炸
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (!IsInsideGrid(x, y)) continue;

                    Card cell = _cells[x, y];
                    if (!cell || cell.IsEmpty || cell.isSealedFloor) continue;

                    score += (int)cell.type;
                    ++pairs;
                    
                    // 计数销毁的元素个数，单个计算非配对
                    _curMatchCount++;
                    
                    removed.Add(cell);
                    cell.Clear();
                }
            }
            
            // 配对数量为炸毁数量的 1/3
            OnBomb?.Invoke(score / 3, pairs / 3);
            
            // 奇数修正
            FixOddCardTypes();
            
            Debug.Log(_curMatchCount + "/" + _maxMatchCount);
            
            if (_curMatchCount == _maxMatchCount)
            {
                Debug.Log("消除完毕");
                
                _curMatchCount = 0;
                OnGridClear?.Invoke();
            }
        }
        
        /// <summary>
        /// 修正爆炸后出现的奇数元素问题
        /// </summary>
        private void FixOddCardTypes()
        {
            Dictionary<CardType, List<Card>> typeMap = new Dictionary<CardType, List<Card>>();

            foreach (var cell in _cells)
            {
                if (!cell || cell.IsEmpty || cell.isSealedFloor) continue;

                if (!typeMap.TryGetValue(cell.type, out var list))
                {
                    list = new List<Card>();
                    typeMap[cell.type] = list;
                }

                list.Add(cell);
            }

            foreach (var pair in typeMap)
            {
                // 若某一类为奇数，随机销毁一个
                if (pair.Value.Count % 2 != 0)
                {
                    int rand = Random.Range(0, pair.Value.Count);
                    pair.Value[rand].Clear();
                    
                    // 计数销毁的元素个数，单个计算非配对
                    _curMatchCount++;
                }
            }
        }

        #endregion
    }
}
