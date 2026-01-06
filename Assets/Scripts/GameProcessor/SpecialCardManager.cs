using System;
using System.Collections.Generic;
using Cell;
using UnityEngine;

namespace GameProcessor
{
    public class SpecialCardManager : MonoBehaviour
    {
        [Header("特殊卡牌")] 
        public List<Card> specialCards;

        #region 事件

        public event Action OnPreCardProcessFinished;
        public event Action OnPostCardProcessFinished;

        #endregion
        
        private void Start()
        {
            GameManager.Instance.OnPreSpecialCard += ProcessPreCard;
            GameManager.Instance.OnPostSpecialCard += ProcessPostCard;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnPreSpecialCard -= ProcessPreCard;
            GameManager.Instance.OnPostSpecialCard -= ProcessPostCard;
        }

        private void ProcessPreCard()
        {
            // TODO: 处理先机卡牌效果
            
            OnPreCardProcessFinished?.Invoke();
        }

        private void ProcessPostCard()
        {
            // TODO：处理后手卡片效果
            
            OnPostCardProcessFinished?.Invoke();
        }
    }
}