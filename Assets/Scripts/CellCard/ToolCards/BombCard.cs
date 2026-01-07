using DG.Tweening;
using GameProcessor;
using SkillManager;
using UnityEngine;

namespace CellCard.ToolCards
{
    public class BombCard : ToolCard
    {
        [SerializeField] private int bombRadius = 3;

        public override void StartSkill()
        {
            PlayBombAnimation();
        }

        private void PlayBombAnimation()
        {
            Transform t = transform;
            Vector3 originPos = t.localPosition;

            Sequence seq = DOTween.Sequence();

            seq.Append(
                t.DOShakePosition(0.3f, new Vector3(25f, 25f, 0f), 30)
            );

            seq.OnComplete(() =>
            {
                t.localPosition = originPos;

                // 随机目标格子位置
                int x = Random.Range(1, GameManager.Instance.gridManager.gridSize.x);
                int y = Random.Range(1, GameManager.Instance.gridManager.gridSize.y);
                Vector3 targetPos = GameManager.Instance.gridManager.GetCellWorldPosition(x, y);
                
                SkillAnimationManager.Instance.StartBladeSkill(targetPos, () =>
                {
                    GameManager.Instance.gridManager.BombAt(x, y, bombRadius);
                    base.StartSkill();
                });
            });
        }

    }
}