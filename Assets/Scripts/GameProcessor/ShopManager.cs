using TMPro;
using UnityEngine;

namespace GameProcessor
{
    public class ShopManager : MonoBehaviour
    {
        [Header("货币")] 
        public int money;
        public TextMeshProUGUI moneyText;

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
    }
}