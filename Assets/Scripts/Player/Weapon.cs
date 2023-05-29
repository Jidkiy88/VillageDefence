using Scripts.NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private Animator animator;
        [SerializeField] private CollisionDetector detector;

        private float _damage;
        private int _criticalDamageChance;
        private int _attackType;
        private float _criticalDamagePercente;

        [Header("Visual Parameters")]
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 startRotation;

        public Action OnAttack;
        public Action OnBlock;
        public Action<bool> OnBusyStateChange;

        private bool _isBusy = false;
        private bool _isBlocked = false;

        private void Start()
        {
            detector.OnTargetHit += OnTargetHit;
            PlaceWeapon();
        }

        public void InitializeWeapon(float dmg, int critChance, float critPercente)
        {
            _damage = dmg;
            _criticalDamageChance = critChance;
            _criticalDamagePercente = critPercente;
        }

        public bool IsBlocked()
        {
            return _isBlocked;
        }

        public int GetId()
        {
            return id;
        }

        private void PlaceWeapon()
        {
            transform.localPosition = startPosition;
            transform.localEulerAngles = startRotation;
        }

        public void SelectWeapon()
        {
            PlaceWeapon();

            gameObject.SetActive(true);
        }

        private void OnTargetHit(NPCBase target)
        {
            float damageToDeal = CalculateDamage();
            target.GetDamage(damageToDeal);
        }

        private float CalculateDamage()
        {
            float DealDamage = 0;
            int luckyNumber = Random.Range(0, 101);

            if (luckyNumber <= _criticalDamageChance)
            {
                DealDamage = (_damage * 0.01f) * _criticalDamagePercente;
            }
            else
            {
                DealDamage = _damage;
            }

            return DealDamage;
        }

        public void Attack()
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            OnBusyStateChange?.Invoke(_isBusy);
            OnAttack?.Invoke();
            animator.SetInteger("AttackType", GetAttackTypeId());
            animator.SetTrigger("Attack");
        }

        private int GetAttackTypeId()
        {
            int value = _attackType + 1;
            if (value >= 3)
            {
                value = 0;
            }

            _attackType = value;
            return _attackType;
        }

        public void OnAttackAnimationHitStart()
        {
            detector.ActivateDetector();
        }

        public void OnAttackAnimationHitEnd()
        {
            detector.DeactivateDetector();
        }

        public void OnAnimationEnd()
        {
            _isBusy = false;
            OnBusyStateChange?.Invoke(_isBusy);
        }

        public void Block()
        {
            if (_isBusy)
            {
                return;
            }
            _isBusy = true;
            OnBusyStateChange?.Invoke(_isBusy);
            OnBlock?.Invoke();
            animator.SetTrigger("Block");
        }

        public void OnBlockAnimationStart()
        {
            _isBlocked = true;
        }

        public void OnBlockAnimationEnd()
        {
            _isBlocked = false;
        }
    }
}
