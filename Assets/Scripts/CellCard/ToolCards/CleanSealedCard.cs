using DG.Tweening;
using GameProcessor;
using SkillManager;
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
                PlayHelperAndClear();
            });
        }

        /// <summary>
        /// 只对选中的行播放动画，动画结束立刻清该行
        /// </summary>
        private void PlayHelperAndClear()
        {
            GridManager grid = GameManager.Instance.gridManager;
            List<int> rows = GetRandomSealedRows(grid);

            if (rows.Count == 0)
            {
                base.StartSkill();
                return;
            }

            int index = 0;

            void PlayNextRow()
            {
                if (index >= rows.Count)
                {
                    base.StartSkill();
                    return;
                }

                int row = rows[index];

                SkillAnimationManager.Instance.StartHelperSkill(row, () =>
                {
                    grid.ClearSealedRows(new List<int> { row });

                    index++;
                    PlayNextRow();
                });
            }

            PlayNextRow();
        }

        /// <summary>
        /// 随机获取包含溟痕的行
        /// </summary>
        private List<int> GetRandomSealedRows(GridManager grid)
        {
            List<int> availableRows = new List<int>();

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

            List<int> selected = new List<int>();
            int count = Mathf.Min(clearRowCount, availableRows.Count);

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, availableRows.Count);
                selected.Add(availableRows[index]);
                availableRows.RemoveAt(index);
            }

            return selected;
        }
    }
}
