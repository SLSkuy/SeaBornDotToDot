using UnityEngine;

namespace CellCard
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Card : MonoBehaviour
    {
        public int x;
        public int y;
        public CardType type;      // 图案类型
        public bool IsEmpty => type == CardType.None;  // 是否已消除
        public bool isSelected;

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

        public void Clear()
        {
            type = CardType.None;
            
            img.enabled = false;
            background.enabled = false;
        }

        #endregion
    }
}
