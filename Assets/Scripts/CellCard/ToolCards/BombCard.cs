using DG.Tweening;
using GameProcessor;
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
                t.DOShakePosition(
                    0.3f,
                    strength: new Vector3(25f, 25f, 0f),
                    vibrato: 30
                )
            );

            seq.OnComplete(() =>
            {
                t.localPosition = originPos;

                // 这里你可以根据选中格子或中心点炸
                int x = Random.Range(0,GameManager.Instance.gridManager.gridSize.x);
                int y = Random.Range(0,GameManager.Instance.gridManager.gridSize.y);
                
                GameManager.Instance.gridManager.BombAt(x, y, bombRadius);

                base.StartSkill();
            });
        }
    }
}