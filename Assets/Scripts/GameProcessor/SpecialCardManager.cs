using System;
using System.Collections;
using System.Collections.Generic;
using CellCard;
using UnityEngine;

namespace GameProcessor
{
    public class SpecialCardManager : MonoBehaviour
    {
        [Header("特殊卡牌")]
        public List<ToolCard> specialCards;

        [Header("处理过程属性")] 
        public float processStep = 0.5f;
        public float windUpTime = 1f;
        public float windDownTime = 2f;

        public event Action OnPreCardProcessFinished;
        public event Action OnPostCardProcessFinished;

        private Queue<ToolCard> _processingQueue;

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
            StartCoroutine(ProcessCards(ToolCardTiming.Pre, OnPreCardProcessFinished));
        }

        private void ProcessPostCard()
        {
            StartCoroutine(ProcessCards(ToolCardTiming.Post, OnPostCardProcessFinished));
        }

        private IEnumerator ProcessCards(ToolCardTiming timing, Action finishedCallback)
        {
            _processingQueue = new Queue<ToolCard>();

            foreach (var card in specialCards)
                if (card.timing == timing)
                    _processingQueue.Enqueue(card);

            if (_processingQueue.Count == 0)
            {
                finishedCallback?.Invoke();
                yield break;
            }
            
            // 停顿一段时间再进行卡牌的逻辑处理
            yield return new WaitForSeconds(windUpTime);

            while (_processingQueue.Count > 0)
            {
                ToolCard card = _processingQueue.Dequeue();

                bool done = false;
                card.OnSkillFinished += () => done = true;

                card.StartSkill();

                yield return new WaitUntil(() => done);
                
                // 每张卡处理间隔
                yield return new WaitForSeconds(processStep);
            }

            // 停顿一段事件再进入下一步处理
            yield return new WaitForSeconds(windDownTime);
            
            finishedCallback?.Invoke();
        }
    }
}
