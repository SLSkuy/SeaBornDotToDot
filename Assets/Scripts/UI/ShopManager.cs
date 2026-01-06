using System.Collections.Generic;
using CellCard;
using TMPro;
using UnityEngine;

namespace GameProcessor
{
    [System.Serializable]
    public class ShopItemData
    {
        public ToolCard cardPrefab;
        public int price;
    }
    
    public class ShopManager : MonoBehaviour
    {
        [Header("货币")] 
        public int money;
        public TextMeshProUGUI moneyText;
        
        [Header("卡牌池")]
        public List<ShopItemData> toolCards;
        
        [Header("商店设置")]
        public int minRefreshCount = 3;
        public int maxRefreshCount = 10;

        private readonly List<ShopItemData> _currentItems = new();

        private void Update()
        {
            UpdateMoneyText();
        }

        public void GetMoney(int n)
        {
            money += n;
            
            UpdateMoneyText();
        }

        private void UpdateMoneyText()
        {
            moneyText.text = money.ToString();
        }
        
        public List<ShopItemData> RefreshShopItems()
        {
            _currentItems.Clear();

            int count = Random.Range(minRefreshCount, maxRefreshCount);

            for (int i = 0; i < count; i++)
            {
                _currentItems.Add(toolCards[Random.Range(0, toolCards.Count)]);
            }

            return _currentItems;
        }
    }
}