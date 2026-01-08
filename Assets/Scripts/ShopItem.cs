using CellCard;
using UI.GameSceneUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerEnterHandler
{
    [Header("商品属性")]
    public int id;
    public Text title;
    
    [Header("组件获取")]
    public Image icon;
    public Button buy;
    public Text priceText;
    public string description;
    
    private ShopPanel _panel;

    public void Init(int i, int price, ToolCard card, ShopPanel panel)
    {
        id = i;
        _panel = panel;
        title.text = card.cardName;
        description = card.description;
        priceText.text = price.ToString();
        icon.sprite = card.icon;
        buy.onClick.AddListener(OnBuy);
    }

    private void OnBuy()
    {
        _panel.TryBuyItem(id);
    }
    
    /// <summary>
    /// 鼠标移入
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _panel.ShowDescription(description);
    }
}
