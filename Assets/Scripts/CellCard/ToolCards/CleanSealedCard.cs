using DG.Tweening;
using GameProcessor;
using UnityEngine;
using System.Collections.Generic;

namespace CellCard.ToolCards
{
    public class CleanSealedCard : ToolCard
    {
        [SerializeField] private int clearRowCount = 3;

        public override void StartSkill()
        {
            PlayCleanAnimation();
        }

        private void PlayCleanAnimation()
        {
            Transform t = transform;
            Vector3 originScale = t.localScale;

            Sequence seq = DOTween.Sequence();
            
            seq.Append(t.DOScale(originScale * 1.1f, 0.2f));
            seq.Append(t.DOScale(originScale, 0.15f));

            seq.OnComplete(() =>
            {
                ClearRandomSealedRows();
                base.StartSkill();
            });
        }

        private void ClearRandomSealedRows()
        {
            GridManager grid = GameManager.Instance.gridManager;

            List<int> availableRows = new List<int>();

            // 收集包含溟痕的行
            for (int y = 1; y <= grid.gridSize.y; y++)
            {
                for (int x = 1; x <= grid.gridSize.x; x++)
                {
                    Card cell = grid.Cells[x, y];
                    if (cell && cell.isSealedFloor)
                    {
                        availableRows.Add(y);
                        break;
                    }
                }
            }

            if (availableRows.Count == 0) return;

            // 随机选行
            List<int> selected = new List<int>();
            int count = Mathf.Min(clearRowCount, availableRows.Count);

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, availableRows.Count);
                selected.Add(availableRows[index]);
                availableRows.RemoveAt(index);
            }

            grid.ClearSealedRows(selected);
        }
    }
}