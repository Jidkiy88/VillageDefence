using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Player
{
    public class WeaponsHolder : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weapons;
        [SerializeField] private WeaponsConfig weaponsConfig;

        private Weapon _currentWeapon;

        public Action OnWeaponSwitch;

        private void Start()
        {
            InitializeHolder();
        }

        public void ClearHolder()
        {
            var list = weaponsConfig.weapons;
            weapons.ForEach(w => w.gameObject.SetActive(false));
            list.ForEach(w => w.state = ItemState.Locked);
            list[0].state = ItemState.Equipped;
            InitializeHolder();
            OnWeaponSwitch?.Invoke();
        }

        private void InitializeHolder()
        {
            if (weapons.Any())
            {
                WeaponInfo currentWeapon = weaponsConfig.weapons.Find(w => w.state == ItemState.Equipped);
                _currentWeapon = weapons.Find(w => w.GetId() == currentWeapon.id);
                weapons.ForEach(w =>
                {
                    WeaponInfo weapon = weaponsConfig.weapons.Find(i => i.id == w.GetId());
                    float damage = weapon.damage;
                    int critChance = weapon.criticalDamageChance;
                    float critPercente = weapon.criticalDamagePercent;
                    w.InitializeWeapon(damage, critChance, critPercente);
                });
                _currentWeapon.SelectWeapon();
                weaponsConfig.OnApply += UpdateWeapon;
            }
        }

        private void UpdateWeapon()
        {
            var selectedWeapon = weaponsConfig.weapons.Find(w => w.state == ItemState.Equipped);
            SelectWeapon(selectedWeapon.id);
        }

        public void SelectWeapon(int id)
        {
            var selectedWeapon = weapons.Find(w => w.GetId() == id);
            weapons.ForEach(w => w.gameObject.SetActive(false));
            _currentWeapon = selectedWeapon;
            _currentWeapon.SelectWeapon();
            OnWeaponSwitch?.Invoke();
        }

        public Weapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }

        public void WeaponAttack()
        {
            if (_currentWeapon == null)
            {
                return;
            }

            _currentWeapon.Attack();
        }

        public void WeaponBlock()
        {
            if (_currentWeapon == null)
            {
                return;
            }

            _currentWeapon.Block();
        }
    }
}
