using System;
using DG.Tweening;
using UnityEngine;

namespace SkillManager
{
    public class SkillAnimationManager : MonoBehaviour
    {
        public static SkillAnimationManager Instance;
        
        public Camera animCamera;
        
        [Header("Man !")] 
        public Transform blade;
        private Vector3 _manStartPos;
        public float flyDuration = 0.3f;

        [Header("小帮手")] 
        public Transform helper;
        private Vector3 _helperStartPos;
        public float moveDuration = 0.3f;

        private void Awake()
        {
            Instance = this;
            
            _manStartPos = blade.position;
            _helperStartPos = helper.position;
        }

        public void StartBladeSkill(Vector3 target, Action onComplete)
        {
            blade.gameObject.SetActive(true);
            blade.position = _manStartPos;
            
            // 震动效果
            Sequence seq = DOTween.Sequence();
            seq.Append(blade.DOMove(target, flyDuration).SetEase(Ease.OutCubic));
            seq.AppendCallback(() =>
            {
                animCamera.transform.DOShakePosition(0.5f, 0.7f);
                blade.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        public void StartHelperSkill(int row, Action onComplete)
        {
            helper.gameObject.SetActive(true);
            helper.position = _helperStartPos;

            // 元素大小与间隔大小
            float gap = 0.65f + 0.1f;
            helper.position += new Vector3(0, (row-1) * gap, 0);
            
            Vector3 target = helper.position + new Vector3(16, 0, 0);
            
            Sequence seq = DOTween.Sequence();
            seq.Append(helper.DOMove(target, moveDuration).SetEase(Ease.OutCubic));
            seq.AppendCallback(() =>
            {
                helper.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
    }
}
