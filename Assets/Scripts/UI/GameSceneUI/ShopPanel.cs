using System.Collections.Generic;
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
        private List<GameObject> _items = new();

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
            List<ShopItemData> shopList = GameManager.Instance.shopManager.RefreshShopItems();

            foreach (ShopItemData shopItemData in shopList)
            {
                GameObject item = Instantiate(itemPrefab, container);
                
                _items.Add(item);
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