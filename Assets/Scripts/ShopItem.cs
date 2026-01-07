using UI.GameSceneUI;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("商品属性")]
    public int id;
    public Text title;
    
    [Header("组件获取")]
    public Image icon;
    public Button buy;
    public Text priceText;
    
    private ShopPanel _panel;

    public void Init(int i, int price, string t, ShopPanel panel, Sprite sprite = null)
    {
        id = i;
        _panel = panel;
        title.text = t;
        priceText.text = price.ToString();
        icon.sprite = sprite;
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
