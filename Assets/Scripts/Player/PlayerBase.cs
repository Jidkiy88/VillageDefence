using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerBase : MonoBehaviour
    {
        [SerializeField] private PlayerControl playerControl;
        [SerializeField] private Health health;
        [SerializeField] private Wallet wallet;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float blockPercent;

        public Action OnDeath;
        public Action OnInit;

        private Weapon _currentWeapon;
        private Vector3 _defaultPosition;
        private bool _inGame = true;

        private void Awake()
        {
            health.OnDeath += Death;
            _defaultPosition = transform.position;
            Initialize();
        }

        public void Initialize()
        {
            rigidbody.isKinematic = false;
            health.RefreshHP();
            transform.position = _defaultPosition;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            controller.enabled = true;
            InGameState(true);
            OnInit?.Invoke();
        }

        private void OnBusy(bool state)
        {
            playerControl.SetSpeed(state);
        }

        public void SetWeapon(Weapon weapon)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.OnBusyStateChange -= OnBusy;
            }

            _currentWeapon = weapon;
            _currentWeapon.OnBusyStateChange += OnBusy;
        }

        private void OnDestroy()
        {
            health.OnDeath -= Death;

            if (_currentWeapon != null)
            {
                _currentWeapon.OnBusyStateChange -= OnBusy;
            }
        }

        public void LoseControl()
        {
            rigidbody.isKinematic = true;
            controller.enabled = false;
            InGameState(false);
        }

        private void Death()
        {
            LoseControl();
            OnDeath?.Invoke();
        }

        public void GetBounty(int value)
        {
            wallet.IncreaseValue(value);
        }

        public bool IsAlive()
        {
            return health.IsAlive();
        }

        public void InGameState(bool state)
        {
            _inGame = state;
            controller.enabled = state;
        }

        public bool IsInGame()
        {
            return _inGame;
        }

        public void GetDamage(float value)
        {
            float damage = _currentWeapon.IsBlocked() ? value - (value * (blockPercent / 100)) : value;
            health.GetDamage(damage);
        }

        public void GetHeal(float value)
        {
            health.GetHeal(value);
        }
    }
}
