using UI.GameSceneUI;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("商品属性")]
    public int id;
    
    [Header("组件获取")]
    public Image icon;
    public Button buy;
    public Text priceText;
    
    private ShopPanel _panel;

    public void Init(int i, int price, ShopPanel panel, Sprite sprite = null)
    {
        id = i;
        _panel = panel;
        priceText.text = price.ToString();
        buy.onClick.AddListener(OnBuy);

        if (!sprite)
        {
            icon.sprite = sprite;
        }
    }

    private void OnBuy()
    {
        _panel.TryBuyItem(id);
    }
}
