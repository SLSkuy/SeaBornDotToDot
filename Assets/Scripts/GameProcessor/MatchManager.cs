using System;
using System.Collections.Generic;
using UnityEngine;
using Cell;

namespace GameProcessor
{
    /// <summary>
    /// 连线匹配处理类
    /// </summary>
    public class MatchManager
    {
        private readonly GridManager _gridManager;
        private readonly LinkVisualizer _linkVisualizer;
        
        private DotToDotCell _firstSelected;
        private DotToDotCell _secondSelected;

        #region 事件
        
        public event Action OnNoneAvailableMatches;
        public event Action OnCellMatch;

        #endregion

        public MatchManager(GridManager gridManager, LinkVisualizer linkVisualizer)
        {
            _gridManager = gridManager;
            _linkVisualizer = linkVisualizer;
        }
        
        public void OnCellClicked(DotToDotCell cell)
        {
            if (!_firstSelected)
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

            // 判断是否能够进行连线
            if (!HasAvailableMatches())
            {
                // 没有可匹配的元素
                OnNoneAvailableMatches?.Invoke();
            }
        }

        private void TryEliminate()
        {
            if (_firstSelected.type != _secondSelected.type) return;

            List<DotToDotCell> path = FindLinkPath(_firstSelected, _secondSelected);
            if (path == null) return;

            // 绘制折线
            _linkVisualizer?.DrawPath(path);

            OnCellMatch?.Invoke();
            _firstSelected.Clear();
            _secondSelected.Clear();
        }

        #region 连线检测

        private List<DotToDotCell> FindLinkPath(DotToDotCell a, DotToDotCell b)
        {
            if (CheckStraight(a, b, out var straightPath)) return straightPath;
            if (CheckOneTurn(a, b, out var oneTurnPath)) return oneTurnPath;
            if (CheckTwoTurn(a, b, out var twoTurnPath)) return twoTurnPath;
            return null;
        }

        private bool CheckStraight(DotToDotCell a, DotToDotCell b, out List<DotToDotCell> path)
        {
            path = new List<DotToDotCell>();
            var cells = _gridManager.Cells;

            if (a.x == b.x)
            {
                int minY = Mathf.Min(a.y, b.y);
                int maxY = Mathf.Max(a.y, b.y);
                for (int y = minY + 1; y < maxY; y++)
                    if (!cells[a.x, y].IsEmpty) return false;

                path.Add(a);
                path.Add(b);
                return true;
            }

            if (a.y == b.y)
            {
                int minX = Mathf.Min(a.x, b.x);
                int maxX = Mathf.Max(a.x, b.x);
                for (int x = minX + 1; x < maxX; x++)
                    if (!cells[x, a.y].IsEmpty) return false;

                path.Add(a);
                path.Add(b);
                return true;
            }

            return false;
        }

        private bool CheckOneTurn(DotToDotCell a, DotToDotCell b, out List<DotToDotCell> path)
        {
            path = new List<DotToDotCell>();
            var cells = _gridManager.Cells;

            // 拐点1
            if (IsEmptyOrTarget(a.x, b.y) &&
                CheckStraight(a, cells[a.x, b.y], out _) &&
                CheckStraight(cells[a.x, b.y], b, out _))
            {
                path.Add(a);
                path.Add(cells[a.x, b.y]);
                path.Add(b);
                return true;
            }

            // 拐点2
            if (IsEmptyOrTarget(b.x, a.y) &&
                CheckStraight(a, cells[b.x, a.y], out _) &&
                CheckStraight(cells[b.x, a.y], b, out _))
            {
                path.Add(a);
                path.Add(cells[b.x, a.y]);
                path.Add(b);
                return true;
            }

            return false;
        }

        private bool CheckTwoTurn(DotToDotCell a, DotToDotCell b, out List<DotToDotCell> path)
        {
            path = new List<DotToDotCell>();
            var cells = _gridManager.Cells;

            // 向四个方向延伸
            for (int y = a.y + 1; y < _gridManager.Cells.GetLength(1); y++)
            {
                if (!cells[a.x, y].IsEmpty) break;
                if (CheckOneTurn(cells[a.x, y], b, out var onePath))
                {
                    path.Add(a);
                    path.AddRange(onePath);
                    return true;
                }
            }

            for (int y = a.y - 1; y >= 0; y--)
            {
                if (!cells[a.x, y].IsEmpty) break;
                if (CheckOneTurn(cells[a.x, y], b, out var onePath))
                {
                    path.Add(a);
                    path.AddRange(onePath);
                    return true;
                }
            }

            for (int x = a.x + 1; x < _gridManager.Cells.GetLength(0); x++)
            {
                if (!cells[x, a.y].IsEmpty) break;
                if (CheckOneTurn(cells[x, a.y], b, out var onePath))
                {
                    path.Add(a);
                    path.AddRange(onePath);
                    return true;
                }
            }

            for (int x = a.x - 1; x >= 0; x--)
            {
                if (!cells[x, a.y].IsEmpty) break;
                if (CheckOneTurn(cells[x, a.y], b, out var onePath))
                {
                    path.Add(a);
                    path.AddRange(onePath);
                    return true;
                }
            }

            return false;
        }

        private bool IsEmptyOrTarget(int x, int y)
        {
            if (x < 0 || x >= _gridManager.Cells.GetLength(0) || y < 0 || y >= _gridManager.Cells.GetLength(1)) return false;
            return _gridManager.Cells[x, y].IsEmpty;
        }

        private bool HasAvailableMatches()
        {
            var cells = _gridManager.Cells;
            int width = cells.GetLength(0);
            int height = cells.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DotToDotCell a = cells[x, y];
                    if (a.IsEmpty) continue;

                    for (int i = x; i < width; i++)
                    {
                        for (int j = y; j < height; j++)
                        {
                            if (i == x && j == y) continue;
                            DotToDotCell b = cells[i, j];
                            if (b.IsEmpty || a.type != b.type) continue;
                            if (FindLinkPath(a, b) != null) return true;
                        }
                    }
                }
            }
            
            return false;
        }

        #endregion
    }
}
