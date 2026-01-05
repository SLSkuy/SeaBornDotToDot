using System.Collections.Generic;
using Cell;
using UnityEngine;

namespace GameProcessor
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

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

        private DotToDotCell[,] _cells;

        private DotToDotCell _firstSelected;
        private DotToDotCell _secondSelected;

        #region 生命周期

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // 锁定帧率
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            if (width % 2 != 0 && height % 2 != 0)
            {
                Debug.Log("给定的数据无法生成偶数的元素");
                return;
            }
            GenerateGrid();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DetectCellClick();
            }
        }

        #endregion

        #region 网格生成

        void CenterGrid()
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

        void GenerateGrid()
        {
            CenterGrid();

            _cells = new DotToDotCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    Vector3 worldPos = grid.GetCellCenterWorld(cellPos);

                    DotToDotCell cell = Instantiate(
                        cellPrefab,
                        worldPos,
                        Quaternion.identity,
                        grid.transform
                    );

                    cell.x = x;
                    cell.y = y;

                    _cells[x, y] = cell;
                }
            }

            FillCells();
        }

        void FillCells()
        {
            int total = width * height;
            int pairCount = total / 2;

            List<CellType> types = new List<CellType>();
            int typeCount = sprites.Length;

            for (int i = 0; i < pairCount; i++)
            {
                CellType type = (CellType)Random.Range(0, typeCount);
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

        #endregion

        #region 点击检测

        void DetectCellClick()
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
            if (cell == null || cell.IsEmpty) return;

            OnCellClicked(cell);
        }

        private void OnCellClicked(DotToDotCell cell)
        {
            if (_firstSelected == null)
            {
                _firstSelected = cell;
                cell.OnSelect(true);
                return;
            }

            if (_firstSelected == cell)
            {
                _firstSelected.OnSelect(false);
                _firstSelected = null;
                return;
            }

            _secondSelected = cell;
            _secondSelected.OnSelect(true);

            TryEliminate();

            _firstSelected.OnSelect(false);
            _secondSelected.OnSelect(false);

            _firstSelected = null;
            _secondSelected = null;
        }

        void TryEliminate()
        {
            if (_firstSelected.type != _secondSelected.type)
                return;

            if (!CanLink(_firstSelected, _secondSelected))
                return;

            _firstSelected.Clear();
            _secondSelected.Clear();
        }
        
        #endregion

        #region 连线检测

        private bool CanLink(DotToDotCell a, DotToDotCell b)
        {
            if (CheckStraight(a, b))
                return true;

            if (CheckOneTurn(a, b))
                return true;

            if (CheckTwoTurn(a, b))
                return true;

            return false;
        }
        
        private bool CheckStraight(DotToDotCell a, DotToDotCell b)
        {
            if (a.x == b.x)
            {
                int minY = Mathf.Min(a.y, b.y) + 1;
                int maxY = Mathf.Max(a.y, b.y);

                for (int y = minY; y < maxY; y++)
                {
                    if (!_cells[a.x, y].IsEmpty)
                        return false;
                }
                return true;
            }

            if (a.y == b.y)
            {
                int minX = Mathf.Min(a.x, b.x) + 1;
                int maxX = Mathf.Max(a.x, b.x);

                for (int x = minX; x < maxX; x++)
                {
                    if (!_cells[x, a.y].IsEmpty)
                        return false;
                }
                return true;
            }

            return false;
        }
        
        bool CheckOneTurn(DotToDotCell a, DotToDotCell b)
        {
            // 拐点1
            if (IsEmptyOrTarget(a.x, b.y))
            {
                if (CheckStraight(a, _cells[a.x, b.y]) &&
                    CheckStraight(_cells[a.x, b.y], b))
                    return true;
            }

            // 拐点2
            if (IsEmptyOrTarget(b.x, a.y))
            {
                if (CheckStraight(a, _cells[b.x, a.y]) &&
                    CheckStraight(_cells[b.x, a.y], b))
                    return true;
            }

            return false;
        }
        
        bool IsEmptyOrTarget(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            return _cells[x, y].IsEmpty;
        }

        bool CheckTwoTurn(DotToDotCell a, DotToDotCell b)
        {
            // 向上
            for (int y = a.y + 1; y < height; y++)
            {
                if (!_cells[a.x, y].IsEmpty)
                    break;

                if (CheckOneTurn(_cells[a.x, y], b))
                    return true;
            }

            // 向下
            for (int y = a.y - 1; y >= 0; y--)
            {
                if (!_cells[a.x, y].IsEmpty)
                    break;

                if (CheckOneTurn(_cells[a.x, y], b))
                    return true;
            }

            // 向右
            for (int x = a.x + 1; x < width; x++)
            {
                if (!_cells[x, a.y].IsEmpty)
                    break;

                if (CheckOneTurn(_cells[x, a.y], b))
                    return true;
            }

            // 向左
            for (int x = a.x - 1; x >= 0; x--)
            {
                if (!_cells[x, a.y].IsEmpty)
                    break;

                if (CheckOneTurn(_cells[x, a.y], b))
                    return true;
            }

            return false;
        }

        #endregion
        
        #region 元素刷新功能

        /// <summary>
        /// 刷新未被消除的元素
        /// </summary>
        public void ShuffleRemainingCells()
        {
            // 1. 收集未被消除的元素
            List<CellType> remainingTypes = new List<CellType>();
            List<DotToDotCell> remainingCells = new List<DotToDotCell>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = _cells[x, y];
                    if (!cell.IsEmpty)
                    {
                        remainingTypes.Add(cell.type);
                        remainingCells.Add(cell);
                    }
                }
            }

            if (remainingTypes.Count == 0)
                return; // 全部消除，无需刷新

            // 2. 打乱元素类型
            for (int i = 0; i < remainingTypes.Count; i++)
            {
                int rand = Random.Range(i, remainingTypes.Count);
                (remainingTypes[i], remainingTypes[rand]) = (remainingTypes[rand], remainingTypes[i]);
            }

            // 3. 重新赋值
            for (int i = 0; i < remainingCells.Count; i++)
            {
                DotToDotCell cell = remainingCells[i];
                CellType type = remainingTypes[i];
                cell.Set(type, sprites[(int)type]);
            }
        }

        #endregion

        #region 可消除检测

        /// <summary>
        /// 检测当前网格中是否存在可消除的元素
        /// </summary>
        /// <returns>存在可消除元素返回true，否则false</returns>
        public bool HasAvailableMatches()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DotToDotCell a = _cells[x, y];
                    if (a.IsEmpty) continue;

                    // 只检查右边和下边，避免重复
                    for (int i = x; i < width; i++)
                    {
                        for (int j = y; j < height; j++)
                        {
                            if (i == x && j == y) continue;

                            DotToDotCell b = _cells[i, j];
                            if (b.IsEmpty) continue;

                            if (a.type != b.type) continue;

                            if (CanLink(a, b))
                                return true; // 找到可消除的一对
                        }
                    }
                }
            }
            return false; // 没有可消除的元素
        }

        #endregion

    }
}
