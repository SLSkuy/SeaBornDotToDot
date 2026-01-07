using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class AutoHorizontalLayout : MonoBehaviour
    {
        [Header("布局区域")]
        public RectTransform layoutArea;

        [Header("卡牌尺寸")]
        public float cardWidth = 160f;
        public float spacing = 20f;

        [Header("动画")]
        public float moveDuration = 0.35f;
        public Ease moveEase = Ease.OutCubic;

        private readonly List<RectTransform> _cards = new();

        /// <summary>
        /// 注册卡牌
        /// </summary>
        public void RegisterCard(RectTransform card)
        {
            if (_cards.Contains(card)) return;

            _cards.Add(card);
            card.SetParent(layoutArea, true);
            card.anchoredPosition = new Vector2(0, 0);
            RefreshLayout();
        }

        /// <summary>
        /// 移除卡牌
        /// </summary>
        public void UnregisterCard(RectTransform card)
        {
            if (_cards.Remove(card))
            {
                RefreshLayout();
            }
        }

        /// <summary>
        /// 刷新布局
        /// </summary>
        public void RefreshLayout()
        {
            if (_cards.Count == 0) return;

            float areaWidth = layoutArea.rect.width;
            int count = _cards.Count;

            // 计算总宽度
            float totalWidth = count * cardWidth + (count - 1) * spacing;

            // 如果超出区域 → 压缩间距
            float realSpacing = spacing;
            if (totalWidth > areaWidth && count > 1)
            {
                realSpacing = (areaWidth - count * cardWidth) / (count - 1);
            }

            float startX = -((count - 1) * (cardWidth + realSpacing)) / 2f;

            for (int i = 0; i < count; i++)
            {
                RectTransform card = _cards[i];
                float targetX = startX + i * (cardWidth + realSpacing);

                card.DOAnchorPosX(targetX, moveDuration)
                    .SetEase(moveEase);
            }
        }
    }
}