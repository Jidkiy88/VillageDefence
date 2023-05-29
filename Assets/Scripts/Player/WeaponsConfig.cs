using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player
{
    [CreateAssetMenu(fileName = "WeaponsConfig", menuName = "Configs")]
    public class WeaponsConfig : ScriptableObject
    {
        public List<WeaponInfo> weapons;

        public Action OnApply;

        public void ApplyWeapon(int id)
        {
            WeaponInfo weapon = weapons.Find(w => w.id == id);
            weapons.ForEach(w =>
            {
                if (w.state == ItemState.Equipped)
                {
                    w.state = ItemState.Owned;
                }
            });

            if (weapon.state == ItemState.Locked || weapon.state == ItemState.Owned)
            {
                weapon.state = ItemState.Equipped;
            }

            OnApply?.Invoke();
        }
    }
}
