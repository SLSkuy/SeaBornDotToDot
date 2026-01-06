using System;
using DG.Tweening;
using UIFramework.UIAnimation;
using UnityEngine;

namespace UI.Animation
{
    /// <summary>
    /// 面板从屏幕底部弹出（带力量感）
    /// </summary>
    public class PanelPopInAni : AnimComponent
    {
        public enum PopDir
        {
            up,
            down,
            left,
            right
        }
        
        [Header("弹出动画")]
        [SerializeField] private PopDir currentDir = PopDir.up;
        [SerializeField] private float moveDuration = 0.4f;
        [SerializeField] private float scaleDuration = 0.25f;
        [SerializeField] private float overshootScale = 1.05f;

        private Tween _tween;

        public override void Animate(Transform target, Action callback)
        {
            // 如果上一个动画还在，强制结束
            _tween?.Kill(true);

            Vector3 originPos = target.localPosition;
            Vector3 endPos = originPos;

            switch (currentDir)
            {
                case PopDir.up:
                    endPos += Vector3.up * Screen.height;
                    break;
                case PopDir.down:
                    endPos += Vector3.down * Screen.height;
                    break;
                case PopDir.left:
                    endPos += Vector3.left * Screen.width;
                    break;
                case PopDir.right:
                    endPos += Vector3.right * Screen.width;
                    break;
            }

            Sequence seq = DOTween.Sequence();
            
            seq.Append(
                target.DOLocalMove(endPos, moveDuration)
                    .SetEase(Ease.OutBack)
            );
            
            seq.Join(
                target.DOScale(overshootScale, scaleDuration)
                    .SetEase(Ease.OutQuad)
            );
            
            seq.Append(
                target.DOScale(1f, 0.15f)
                    .SetEase(Ease.OutQuad)
            );

            seq.OnComplete(() =>
            {
                _tween = null;
                callback?.Invoke();
            });

            _tween = seq;
        }
    }
}