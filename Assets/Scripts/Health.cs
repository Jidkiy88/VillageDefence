using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealthPoints;
        [SerializeField] private float currentHealthPoint;

        public Action OnHealthValueChange;
        public Action OnHit;
        public Action OnDeath;
        public Action OnRevive;

        private bool _isAlive = true;
        private bool _isWounded = false;

        private void Awake()
        {
            RefreshHP();
        }

        public void RefreshHP()
        {
            currentHealthPoint = maxHealthPoints;
            _isAlive = true;
            OnHealthValueChange?.Invoke();
        }

        public void GetDamage(float damagePoints)
        {
            if (!_isAlive)
            {
                return;
            }

            currentHealthPoint -= damagePoints;

            if (IsAlive())
            {
                OnHit?.Invoke();
            }

            CheckHealth();
        }

        public float GetHealthInfo()
        {
            return currentHealthPoint;
        }

        public float GetMaxHealthInfo()
        {
            return maxHealthPoints;
        }

        public bool IsAlive()
        {
            return currentHealthPoint > 0;
        }

        public void GetHeal(float healPoints)
        {
            if (!_isAlive)
            {
                return;
            }

            currentHealthPoint += healPoints;
            CheckHealth();
        }

        public void Revive()
        {
            _isAlive = true;
            currentHealthPoint = maxHealthPoints;
            OnHealthValueChange?.Invoke();
            OnRevive?.Invoke();
        }

        public bool IsWounded()
        {
            return _isWounded;
        }

        private void CheckHealth()
        {
            currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0f, maxHealthPoints);
            OnHealthValueChange?.Invoke();

            _isWounded = maxHealthPoints > currentHealthPoint;

            if (currentHealthPoint == 0f)
            {
                _isAlive = false;
                OnDeath?.Invoke();
                return;
            }
        }
    }
}
