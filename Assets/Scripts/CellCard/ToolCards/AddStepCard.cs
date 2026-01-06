using DG.Tweening;
using GameProcessor;
using UnityEngine;

namespace CellCard.ToolCards
{
    public class AddStepCard : ToolCard
    {
        [SerializeField] private int addStepCount = 3;

        public override void StartSkill()
        {
            PlayPopAnimation();
        }

        private void PlayPopAnimation()
        {
            Transform t = transform;
            Vector3 originScale = t.localScale;

            Sequence seq = DOTween.Sequence();

            seq.Append(t.DOScale(originScale * 1.2f, 0.15f));
            seq.Append(t.DOScale(originScale, 0.15f));

            seq.OnComplete(() =>
            {
                GameManager.Instance.AddStep(addStepCount);
                base.StartSkill();
            });
        }
    }
}