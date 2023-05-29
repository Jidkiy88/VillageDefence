using Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ShopWindow : MonoBehaviour
    {
        [SerializeField] private List<ShopItem> items;
        [SerializeField] private PlayerBase player;
        [SerializeField] private WeaponsConfig weaponsConfig;
        [SerializeField] private Button closeButton;
        [SerializeField] private InteractableObject interactor;
        [SerializeField] private Wallet wallet;

        private void Awake()
        {
            interactor.OnInteract += Open;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void InitializeShop()
        {
            closeButton.onClick.AddListener(Close);
            items.ForEach(i => 
            {
                UpdateItemInfo(i);
                i.OnClick += ApplyItem;
            });
        }

        private void UpdateItemInfo(ShopItem item)
        {
            WeaponInfo weapon = weaponsConfig.weapons.Find(w => w.id == item.GetId());
            item.SetInfo(weapon.weaponName, weapon.weaponIcon, weapon.weaponCost, weapon.state);
        }

        private void ApplyItem(int id)
        {
            var weapon = weaponsConfig.weapons.Find(w => w.id == id);
            var item = items.Find(i => i.GetId() == id);

            if (weapon.state == ItemState.Locked)
            {
                if (wallet.GetValue() >= weapon.weaponCost)
                {
                    wallet.DecreaseValue(weapon.weaponCost);
                }
                else
                {
                    Debug.Log("No Money");
                    return;
                }
            }

            weaponsConfig.ApplyWeapon(id);
            items.ForEach(i => UpdateItemInfo(i));
        }

        private void Open()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.InGameState(false);
            gameObject.SetActive(true);
            InitializeShop();
        }

        private void Close()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.InGameState(true);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(Close);
            interactor.OnInteract -= Open;
            items.ForEach(i => i.OnClick -= ApplyItem);
        }
    }
}
