using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private TextMeshProUGUI weaponLabel;
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponInfoLabel;
        [SerializeField] private Button purchaseButton;

        public Action<int> OnClick;
        private int _price;
        private ItemState _state;

        private void Start()
        {
            purchaseButton.onClick.AddListener(OnButtonClick);    
        }

        public void SetInfo(string name, Sprite sprite, int price, ItemState state)
        {
            weaponLabel.text = name;
            weaponImage.sprite = sprite;
            _price = price;
            _state = state;

            UpdateItemView();
        }

        public void UpdateItemView()
        {
            switch (_state)
            {
                case ItemState.Locked:
                    {
                        purchaseButton.enabled = true;
                        weaponInfoLabel.text = _price.ToString();
                        break;
                    }
                case ItemState.Owned:
                    {
                        purchaseButton.enabled = true;
                        weaponInfoLabel.text = "Select";
                        break;
                    }
                case ItemState.Equipped:
                    {
                        purchaseButton.enabled = false;
                        weaponInfoLabel.text = "Equipped";
                        break;
                    }
            }
        }

        public void OnButtonClick()
        {
            OnClick?.Invoke(id);
        }

        public int GetId()
        {
            return id;
        }

        private void OnDestroy()
        {
            purchaseButton.onClick.RemoveListener(OnButtonClick);
        }
    }
}