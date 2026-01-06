using DG.Tweening;
using GameProcessor;
using UnityEngine;

namespace CellCard.ToolCards
{
    public class ShuffleCard : ToolCard
    {
        private void Awake()
        {
            toolType = ToolCardType.Shuffle;
            timing = ToolCardTiming.Post;
        }

        public override void StartSkill()
        {
            Debug.Log("使用【洗牌卡】");

            // 先执行效果（也可以放到动画结束后，看设计）
            GameManager.Instance.gridManager.ShuffleRemainingCells();

            // 播放卡牌晃动动画
            PlayShakeAnimation();
        }

        private void PlayShakeAnimation()
        {
            Transform t = transform;

            // 记录初始状态，防止累积误差
            Vector3 originPos = t.localPosition;
            Quaternion originRot = t.localRotation;

            Sequence seq = DOTween.Sequence();

            seq.Append(
                t.DOShakePosition(
                    duration: 0.4f,
                    strength: new Vector3(20f, 0f, 0f),
                    vibrato: 20,
                    randomness: 90f,
                    snapping: false,
                    fadeOut: true
                )
            );

            seq.Join(
                t.DOShakeRotation(
                    duration: 0.4f,
                    strength: new Vector3(0f, 0f, 15f),
                    vibrato: 20,
                    randomness: 90f,
                    fadeOut: true
                )
            );

            seq.OnComplete(() =>
            {
                // 还原状态（重要）
                t.localPosition = originPos;
                t.localRotation = originRot;

                // 通知技能结束
                base.StartSkill();
            });
        }
    }
}