using DG.Tweening;
using Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts.NPC
{
    public class NPCBase : MonoBehaviour
    {
        [SerializeField] private NPC_Type npcType;
        [SerializeField] private Health health;
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private Collider collider;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected CharactersAnimator animator;
        [SerializeField] protected ParticlesHolder particlesHolder;
        [SerializeField] protected WavesHolder wavesHolder;
        [SerializeField] private float damage;
        [SerializeField] protected float attackDistance;
        [SerializeField] protected int bounty;
        [SerializeField] protected float viewDistance;
        [SerializeField] private bool isArmed;

        public Action OnDeath;
        public Action OnRevive;
        public Action OnFindTarget;


        protected Coroutine _followCoroutine;
        protected Coroutine _checkPlayerCoroutine;
        private Coroutine _lookAtTargetCoroutine;
        private Coroutine _walkToPointCoroutine;

        protected bool _isAlive = true;
        protected bool _isBusy = false;
        protected bool _canMove = true;
        protected bool _attacksPlayer = false;

        protected NPCBase _currentTarget;
        private NPCBase _lastTarget;
        protected PlayerBase _player;
        private float _agentDefaultRadius;
        private Vector3 _defaultPosition;
        private Vector3 _defaultRotation;

        private void Awake()
        {
            _defaultPosition = transform.position;
            _defaultRotation = transform.localEulerAngles;
            _agentDefaultRadius = agent.radius;
            gameObject.SetActive(false);
        }

        protected virtual void Awaken()
        {

        }

        public void Spawn()
        {
            Initialize();
            Awaken();
        }

        public virtual void Despawn()
        {
            animator.Idle();
            health.OnDeath -= Death;
            health.OnHit -= OnHit;
        }

        public void Victory()
        {
            FreezeState(true);
            StopAllCoroutines();
            animator.Victory();
        }

        protected void AggressiveToPlayer()
        {
            if (_player != null && IsAlive() && _player.IsAlive())
            {
                StopAggressiveToPlayer();
                _checkPlayerCoroutine = StartCoroutine(CheckPlayer());
            }
        }

        protected IEnumerator CheckPlayer()
        {
            while (IsAlive() && _player.IsAlive())
            {
                if (IsTargetClose(_player.transform) && !_attacksPlayer && !_isBusy)
                {
                    AttackPlayer();
                }
                else if (!IsTargetClose(_player.transform, 2) && _attacksPlayer)
                {
                    _attacksPlayer = false;
                    UpdateTarget();
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        private bool IsTargetClose(Transform target, float multiplier = 1)
        {
            return Vector3.Distance(transform.position, target.position) <= viewDistance * multiplier;
        }

        private void AttackPlayer()
        {
            if (_followCoroutine != null)
            {
                StopCoroutine(_followCoroutine);
                StopNPC();
                _followCoroutine = null;
            }
            _currentTarget = null;
            _attacksPlayer = true;
            _followCoroutine = StartCoroutine(FollowCoroutine(_player.transform));
        }

        protected void StopAggressiveToPlayer()
        {
            if (_checkPlayerCoroutine != null)
            {
                StopCoroutine(_checkPlayerCoroutine);
                _checkPlayerCoroutine = null;
            }
        }

        protected virtual void Initialize()
        {
            agent.enabled = true;
            agent.radius = _agentDefaultRadius;
            transform.position = _defaultPosition;
            transform.localEulerAngles = _defaultRotation;
            collider.enabled = true;
            rigidbody.isKinematic = false;
            _isAlive = true;
            _isBusy = false;
            _attacksPlayer = false;
            _canMove = true;
            _player = wavesHolder.GetPlayer();
            health.OnDeath += Death;
            health.OnHit += OnHit;
            health.RefreshHP();
            StopNPC();
            gameObject.SetActive(true);
            animator.SetArmed(isArmed);
        }

        public virtual void OnCastEnd()
        {

        }

        protected virtual void OnDestroy()
        {
            health.OnDeath -= Death;
            health.OnHit -= OnHit;
        }

        protected void StopFollow()
        {
            if (_followCoroutine == null)
            {
                return;
            }

            StopCoroutine(_followCoroutine);
            _followCoroutine = null;
        }

        protected IEnumerator FollowCoroutine(Transform target)
        {
            while (Vector3.Distance(transform.position, target.position) > attackDistance)
            {
                if (_isAlive && agent.enabled && agent.isOnNavMesh && !_isBusy)
                {
                    WalkToPoint(target.position);
                }
                yield return new WaitForEndOfFrame();
            }
            TryToHit();
        }

        protected void WalkToPoint(Vector3 pos)
        {
            StopWalkToPointCor();
            _walkToPointCoroutine = StartCoroutine(WalkToPointCor(pos));
        }

        private void StopWalkToPointCor()
        {
            if (_walkToPointCoroutine == null)
            {
                return;
            }

            StopCoroutine(_walkToPointCoroutine);
            _walkToPointCoroutine = null;
        }

        private IEnumerator WalkToPointCor(Vector3 pos)
        {
            yield return new WaitUntil(() => _canMove);
            animator.Walk();
            agent.Resume();
            agent.SetDestination(pos);
        }

        public NPC_Type GetNPCType()
        {
            return npcType;
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        public bool IsWounded()
        {
            return health.IsWounded();
        }

        public void GetDamage(float value)
        {
            health.GetDamage(value);
            if (particlesHolder != null)
            {
                particlesHolder.OnHit();
            }
        }

        public void GetHeal(float value)
        {
            health.GetHeal(value);
            if (particlesHolder != null)
            {
                particlesHolder.OnHeal();
            }
        }

        protected virtual void Death()
        {
            FreezeState(true);
            animator.Death();
            StopFollow();
            StopLookTarget();
            rigidbody.isKinematic = true;
            collider.enabled = false;
            agent.radius = 0f;
            _isAlive = false;
            OnDeath?.Invoke();
        }

        private void OnHit()
        {
            _attacksPlayer = false;
            if (_isAlive && IsTargetClose(_player.transform, 2))
            {
                AttackPlayer();
            }
            else
            {
                UpdateTarget();
            }
            animator.Hit();
        }

        public void OnHitAnimationStart()
        {
            FreezeState(true);
        }

        public void OnHitAnimationEnd()
        {
            FreezeState(false);
            _isBusy = false;
        }

        public void OnAttackAnimationStart()
        {
            FreezeState(true);
            if (_currentTarget != null && !_attacksPlayer)
            {
                _currentTarget.FreezeState(true);
            }
        }

        public void OnAttackHit()
        {
            if (!_attacksPlayer && _currentTarget == null)
            {
                return;
            }

            Transform target = _attacksPlayer ? _player.transform : _currentTarget.transform;
            var distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackDistance + 0.5f)
            {
                if (_attacksPlayer)
                {
                    _player.GetDamage(damage);
                }
                else
                {
                    _currentTarget.GetDamage(damage);
                    if (_currentTarget == null || !_currentTarget.IsAlive())
                    {
                        UpdateTarget();
                    }
                }
            }
        }

        public void OnAttackAnimationEnd()
        {
            FreezeState(false);
            StopLookTarget();
            AttackEnd();
            if (_currentTarget != null && !_attacksPlayer)
            {
                _currentTarget.FreezeState(false);
            }
        }

        protected void FreezeState(bool state)
        {
            if (!_isAlive)
            {
                return;
            }

            if (state)
            {
                StopNPC();
            }

            _canMove = !state;
        }

        protected void TryToHit()
        {
            StopNPC();
            if (!_attacksPlayer)
            {
                if (_currentTarget == null)
                {
                    return;
                }
                _currentTarget.FreezeState(true);
                _currentTarget.FaceTo(transform);
            }
            Transform target = _attacksPlayer ? _player.transform : _currentTarget.transform;
            LookTarget(target);
            animator.Attack();
        }

        protected void SaveLastTarget()
        {
            _lastTarget = _currentTarget;
        }

        protected void AttackEnd()
        {
            _attacksPlayer = false;
            if (_isAlive && IsTargetClose(_player.transform, 2))
            {
                AttackPlayer();
            }
            else
            {
                UpdateTarget();
            }
        }

        protected void AggressiveBehaviour()
        {
            if (_currentTarget == null)
            {
                return;
            }

            if (_followCoroutine != null)
            {
                _followCoroutine = null;
            }

            _followCoroutine = StartCoroutine(FollowCoroutine(_currentTarget.transform));
        }

        protected void LookTarget(Transform target)
        {
            StopLookTarget();

            _isBusy = true;
            _lookAtTargetCoroutine = StartCoroutine(LookAtTarget(target));
        }

        public void StopLookTarget()
        {
            if (_lookAtTargetCoroutine != null)
            {
                StopCoroutine(_lookAtTargetCoroutine);
                _lookAtTargetCoroutine = null;
            }

            _isBusy = false;
        }

        private IEnumerator LookAtTarget(Transform target)
        {
            while (_isBusy)
            {
                var pos = new Vector3(target.position.x, transform.position.y, target.position.z);
                transform.LookAt(pos);
                yield return new WaitForEndOfFrame();
            }
        }

        protected void UpdateTarget()
        {
            if (_currentTarget == null)
            {
                _currentTarget = wavesHolder.GetRandomNPC(NPC_Type.Citizen, true);
                if (_currentTarget == null)
                {
                    wavesHolder.EnemiesWin();
                    return;
                }
                _currentTarget.OnDeath += SaveLastTarget;
                _currentTarget.OnDeath += UpdateTarget;
            }
            if (_lastTarget != null)
            {
                _lastTarget.OnDeath -= UpdateTarget;
                _lastTarget.OnDeath -= SaveLastTarget;
                _currentTarget = wavesHolder.GetRandomNPC(NPC_Type.Citizen, true);
                if (_currentTarget == null)
                {
                    wavesHolder.EnemiesWin();
                    return;
                }
                _currentTarget.OnDeath += SaveLastTarget;
                _currentTarget.OnDeath += UpdateTarget;
                OnFindTarget?.Invoke();
                return;
            }
            if (!_currentTarget.IsAlive())
            {
                _currentTarget.OnDeath -= UpdateTarget;
                _currentTarget.OnDeath -= SaveLastTarget;
                _currentTarget = wavesHolder.GetRandomNPC(NPC_Type.Citizen, true);
                if (_currentTarget == null)
                {
                    wavesHolder.EnemiesWin();
                    return;
                }
                _currentTarget.OnDeath += SaveLastTarget;
                _currentTarget.OnDeath += UpdateTarget;
            }
            OnFindTarget?.Invoke();
        }

        public void FaceTo(Transform target)
        {
            var pos = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.DOLookAt(pos, 0.5f);
        }

        protected void RewardByDeath()
        {
            _player.GetBounty(bounty);
        }

        protected void StopNPC()
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.Stop();
            }

            animator.Idle();
        }

        public virtual void Revive()
        {
            animator.Revive();
            FreezeState(false);
            rigidbody.isKinematic = false;
            collider.enabled = true;
            agent.radius = _agentDefaultRadius;
            health.Revive();
            _isAlive = true;
            Awaken();
            OnRevive?.Invoke();
        }
    }
}

public enum NPC_Type
{
    Citizen,
    Enemy
}
