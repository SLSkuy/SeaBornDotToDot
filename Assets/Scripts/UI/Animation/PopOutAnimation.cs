using System;
using DG.Tweening;
using UIFramework.UIAnimation;
using UnityEngine;

namespace UI.Animation
{
    /// <summary>
    /// 面板向下收回动画
    /// </summary>
    public class PanelPopOutAni : AnimComponent
    {
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private PanelPopInAni.PopDir currentDir;

        private Tween _tween;

        public override void Animate(Transform target, Action callback)
        {
            _tween?.Kill(true);

            Vector3 endPos = target.localPosition;
            
            switch (currentDir)
            {
                case PanelPopInAni.PopDir.up:
                    endPos += Vector3.up * Screen.height;
                    break;
                case PanelPopInAni.PopDir.down:
                    endPos += Vector3.down * Screen.height;
                    break;
                case PanelPopInAni.PopDir.left:
                    endPos += Vector3.left * Screen.width;
                    break;
                case PanelPopInAni.PopDir.right:
                    endPos += Vector3.right * Screen.width;
                    break;
            }

            _tween = target
                .DOLocalMove(endPos, duration)
                .OnComplete(() =>
                {
                    _tween = null;
                    callback?.Invoke();
                });
        }
    }
}