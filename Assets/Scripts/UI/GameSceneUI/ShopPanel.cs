using System.Collections.Generic;
using CellCard;
using EventProcess;
using GameProcessor;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class ExitShop : ASignal { }

    public class ShopPanel : PanelController
    {
        [Header("交互按钮")] 
        public Button bargainButton;
        public Button exitButton;

        [Header("商品属性")] 
        public GameObject itemPrefab;
        public Transform container;
        private readonly List<GameObject> _items = new();
        private List<ShopItemData> _currentShopItems;

        private void Start()
        {
            exitButton.onClick.AddListener(UI_ExitButton);

            GameManager.Instance.OnShopTime += OnRefreshShop;
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveAllListeners();
            
            GameManager.Instance.OnShopTime -= OnRefreshShop;
        }

        private void OnRefreshShop()
        {
            ClearItems();

            _currentShopItems = GameManager.Instance.shopManager.RefreshShopItems();

            for (int i = 0; i < _currentShopItems.Count; i++)
            {
                GameObject item = Instantiate(itemPrefab, container);

                ShopItem shopItem = item.GetComponent<ShopItem>();
                shopItem.id = i;

                int index = i;
                shopItem.buy.onClick.AddListener(() => TryBuyItem(index));

                _items.Add(item);
            }
        }
        
        private void ClearItems()
        {
            foreach (GameObject item in _items)
                if (item) Destroy(item);

            _items.Clear();
        }
        
        private void TryBuyItem(int id)
        {
            if (id < 0 || id >= _currentShopItems.Count)
                return;

            ShopItemData data = _currentShopItems[id];

            if (GameManager.Instance.shopManager.TryBuyItem(data, out ToolCard card))
            {
                GameManager.Instance.specialCardManager.AddCard(card);

                // 购买成功，移除UI
                Destroy(_items[id]);
                _items[id] = null;
            }
        }

        private void OnDisable()
        {
            foreach (GameObject item in _items)
            {
                Destroy(item);
            }
            
            _items.Clear();
        }

        private void UI_ExitButton()
        {
            Signals.Get<ExitShop>().Dispatch();
        }
    }
}