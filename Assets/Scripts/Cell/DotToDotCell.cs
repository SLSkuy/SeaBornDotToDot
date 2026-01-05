using UnityEngine;

namespace Cell
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DotToDotCell : MonoBehaviour
    {
        public int x;
        public int y;
        public CellType type;      // 图案类型
        public bool IsEmpty => type == CellType.None;  // 是否已消除
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

        public void HighLight()
        {
            background.color = isSelected ? Color.yellow : Color.white;
        }

        public void Set(CellType cellType, Sprite sprite)
        {
            type = cellType;
            img.sprite = sprite;
            
            img.enabled = true;
            background.enabled = true;
        }

        public void Clear()
        {
            type = CellType.None;
            
            img.enabled = false;
            background.enabled = false;
        }

        #endregion
    }
}
