using System;
using System.Collections.Generic;
using UnityEngine;
using Cell;
using Random = UnityEngine.Random;

namespace GameProcessor
{
    public class GridManager : MonoBehaviour
    {
        [Header("网格设置")]
        public Grid grid;
        public int width = 8;
        public int height = 8;

        [Header("元素设置")]
        public DotToDotCell cellPrefab;
        public Sprite[] sprites;
        
        [Header("输入设置")]
        public Camera cam;
        public LayerMask cellLayer;

        public DotToDotCell[,] Cells => _cells;
        private DotToDotCell[,] _cells;

        private int _curMatchCount;
        private int _maxMatchCount;
        
        // 组件引用
        private MatchManager _matchManager;

        #region 事件

        public event Action OnGameOver;

        #endregion

        private void Awake()
        {
            _matchManager = new MatchManager(this, GetComponent<LinkVisualizer>());
        }

        private void Start()
        {
            if (width % 2 != 0 && height % 2 != 0)
            {
                Debug.LogWarning("网格宽高需要偶数才能生成成对元素！");
                return;
            }

            _maxMatchCount = width * height / 2;
            _matchManager.OnCellMatch += OnCellMatch;
            
            GenerateGrid();
        }

        private void OnDestroy()
        {
            _matchManager.OnCellMatch -= OnCellMatch;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectCellClick();
            }
        }

        private void OnCellMatch()
        {
            ++_curMatchCount;
            if (_curMatchCount == _maxMatchCount)
            {
                Debug.Log("消除完毕，游戏结束");
                OnGameOver?.Invoke();
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

            DotToDotCell cell = hit.collider.GetComponent<DotToDotCell>();
            if (!cell || cell.IsEmpty) return;

            _matchManager.OnCellClicked(cell);
        }

        /// <summary>
        /// 置中网格
        /// </summary>
        private void CenterGrid()
        {
            Vector3 cellSize = grid.cellSize;
            Vector3 cellGap = grid.cellGap;

            float boardWidth = width * cellSize.x;
            float boardHeight = height * cellSize.y;
            float gapWidth = width * cellGap.x;
            float gapHeight = height * cellGap.y;

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
            _cells = new DotToDotCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPos = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    DotToDotCell cell = Instantiate(cellPrefab, worldPos, Quaternion.identity, grid.transform);
                    cell.x = x;
                    cell.y = y;
                    _cells[x, y] = cell;
                }
            }

            FillCells();
        }

        /// <summary>
        /// 填充元素进网格
        /// </summary>
        private void FillCells()
        {
            int total = width * height;
            int pairCount = total / 2;
            List<CellType> types = new List<CellType>();

            for (int i = 0; i < pairCount; i++)
            {
                CellType type = (CellType)Random.Range(0, sprites.Length);
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
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CellType type = types[index++];
                    _cells[x, y].Set(type, sprites[(int)type]);
                }
            }
        }
        
        /// <summary>
        /// 刷新替换当前未被消除元素位置
        /// </summary>
        public void ShuffleRemainingCells()
        {
            List<DotToDotCell> remainingCells = new List<DotToDotCell>();
            List<CellType> remainingTypes = new List<CellType>();

            foreach (var cell in _cells)
                if (!cell.IsEmpty)
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
    }
}
