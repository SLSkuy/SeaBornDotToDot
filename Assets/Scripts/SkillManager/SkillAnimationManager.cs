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
        private Vector3 _startPos;
        public float flyDuration = 0.3f;
        
        private void Awake()
        {
            Instance = this;
            
            _startPos = blade.position;
        }

        public void StartBladeSkill(Vector3 target, Action onComplete)
        {
            blade.gameObject.SetActive(true);
            blade.position = _startPos;
            
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
    }
}
