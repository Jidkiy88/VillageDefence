using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Scripts.NPC
{
    public class Citizen : NPCBase
    {
        [SerializeField] private List<Transform> waypoints;

        private Coroutine _patrol;
        private Coroutine _enemyCheck;

        private float _agentDefaultSpeed;
        private bool _isScared = false;
        private bool _reverseWay = false;


        protected override void Initialize()
        {
            base.Initialize();
            PatrolWay();
            CheckEnemies();
            _agentDefaultSpeed = agent.speed;
        }

        private void CheckEnemies()
        {
            if (_enemyCheck != null)
            {
                StopCoroutine(_enemyCheck);
                _enemyCheck = null;
            }
            _enemyCheck = StartCoroutine(EnemyChecker());
        }

        private void PatrolWay()
        {
            if (_patrol != null)
            {
                StopCoroutine(_patrol);
                _patrol = null;
            }
            _patrol = StartCoroutine(Patrol());
        }

        private void StopPatrol()
        {
            if (_patrol != null)
            {
                StopCoroutine(_patrol);
                _patrol = null;
            }
        }

        private bool CheckForEnemies()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    var enemy = wavesHolder.GetNPC(collider.gameObject);
                    if (enemy.IsAlive())
                    {
                        _isScared = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private IEnumerator EnemyChecker()
        {
            while (_isAlive)
            {
                if (_canMove)
                {
                    if (CheckForEnemies())
                    {
                        if (_isScared)
                        {
                            animator.Scare(true);
                            agent.speed = 1.5f;
                        }
                        StopPatrol();
                        RunFromEnemies();
                    }
                    else
                    {
                        if (_isScared)
                        {
                            PatrolWay();
                            animator.Scare(false);
                            _isScared = false;
                            FreezeState(false);
                            agent.speed = _agentDefaultSpeed;
                        }
                    }
                }
                yield return new WaitForSeconds(0.4f);
            }
        }

        private Transform GetClosestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance);
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = collider.transform;
                    }
                }
            }
            return closestEnemy;
        }

        private void RunFromEnemies()
        {
            Vector3 direction = transform.position - GetClosestEnemy().position;
            direction.y = 0f;
            Vector3 position = transform.position + direction.normalized * viewDistance;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 4f, NavMesh.AllAreas))
            {
                WalkToPoint(hit.position);
            }
        }

        private IEnumerator Patrol()
        {
            while (_isAlive)
            {
                Transform movePoint = waypoints[Random.Range(0, waypoints.Count)];
                WalkToPoint(movePoint.position);
                yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            }
        }
    }
}
