using System.Collections;
using GameProcessor;
using UnityEngine;

namespace CellCard
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Card : MonoBehaviour
    {
        private static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        public int x;
        public int y;
        public CardType type;      // 图案类型
        public bool IsEmpty => type == CardType.None;  // 是否已消除
        public bool isSelected;
        public bool isSealedFloor;

        [Header("图片渲染")]
        [SerializeField]private SpriteRenderer img;
        [SerializeField]private SpriteRenderer background;
        
        #region 成员方法

        public void OnSelect(bool select)
        {
            isSelected = select;
            HighLight();
        }

        private void HighLight()
        {
            background.color = isSelected ? Color.yellow : Color.white;
        }

        public void Set(CardType cardType, Sprite sprite)
        {
            type = cardType;
            img.sprite = sprite;
            
            img.enabled = enabled;
            background.enabled = true;
        }

        public void SetSealedFloor()
        {
            // 切换为溟痕
            type = CardType.SealedFloor;
            isSealedFloor = true;

            img.sprite = GameManager.Instance.gridManager.sealedFloor;
            
            Material matInstance = new Material(GameManager.Instance.gridManager.sealedFloorMat);
            img.material = matInstance;
                
            // 启用溟痕快
            StartCoroutine(DissolveIn(img.material));
                
            // 记录溟痕块
            GameManager.Instance.gridManager.RecordSealedCell(x,y);

            img.enabled = true;
            background.enabled = false;
        }

        public void Clear()
        {
            if (type == CardType.ShenMingYinHenZhe)
            {
                SetSealedFloor();
                return;
            }

            if (type == CardType.SealedFloor)
            {
                // 溟痕无法常规清除
                return;
            }
            
            type = CardType.None;
            
            img.enabled = false;
            background.enabled = false;
        }
        
        IEnumerator DissolveIn(Material mat)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                mat.SetFloat(NoiseStrength, t);
                yield return null;
            }
        }


        #endregion
    }
}
