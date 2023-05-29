using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player
{
    [Serializable]
    public class WeaponInfo
    {
        public int id;
        public string weaponName;
        public Sprite weaponIcon;
        public int weaponCost;
        public float damage;
        [Range(0, 100)] public int criticalDamageChance;
        public float criticalDamagePercent;
        public ItemState state;
    }
}
public enum ItemState
{
    Locked,
    Owned,
    Equipped
}