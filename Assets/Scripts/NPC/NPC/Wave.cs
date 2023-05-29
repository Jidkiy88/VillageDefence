using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.NPC
{
    public class Wave : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private List<NPCBase> npcs;

        private List<NPCBase> _enemies;
        private List<NPCBase> _citizens;

        public Action<bool> OnWaveEnd;

        private void Awake()
        {
            _enemies = GetTypeNPCs(NPC_Type.Enemy, true);
            _citizens = GetTypeNPCs(NPC_Type.Citizen, true);
        }

        private void Start()
        {
            _enemies.ForEach(e => e.OnDeath += CheckWaveWin);
            _citizens.ForEach(c => c.OnDeath += CheckWaveLose);
        }

        private void CheckWaveLose()
        {
            var aliveCitizens = _citizens.Where(c => c.IsAlive()).ToList().Count;
            if (aliveCitizens <= 0)
            {
                OnWaveEnd?.Invoke(false);
                _citizens.ForEach(c => c.OnDeath -= CheckWaveLose);
            }
        }

        private void CheckWaveWin()
        {
            var aliveEnemiesCount = _enemies.Where(e => e.IsAlive()).ToList().Count;
            if (aliveEnemiesCount <= 0)
            {
                OnWaveEnd?.Invoke(true);
                _enemies.ForEach(e => e.OnDeath -= CheckWaveWin);
            }
        }

        public int GetId()
        {
            return id;
        }

        public List<NPCBase> GetNPCs()
        {
            return npcs;
        }

        public List<NPCBase> GetTypeNPCs(NPC_Type type, bool isAlive = false)
        {
            if (isAlive)
            {
                List<NPCBase> list = npcs.Where(n => n.GetNPCType() == type && n.IsAlive()).ToList();
                return list;
            }
            else
            {
                List<NPCBase> list = npcs.Where(n => n.GetNPCType() == type).ToList();
                return list;
            }
        }

        public NPCBase GetNPC(GameObject target)
        {
            var npc = npcs.Find(n => n.gameObject == target);
            return npc;
        }

        public NPCBase GetRandomNPC(List<NPCBase> list)
        {
            if (!list.Any())
            {
                return null;
            }
            int randomIndex = Random.Range(0, list.Count);
            NPCBase randomNPC = list[randomIndex];
            return randomNPC;
        }

        public void StartWave()
        {
            gameObject.SetActive(true);
            npcs.ForEach(n => n.Spawn());
        }

        public void EndWave()
        {
            npcs.ForEach(n => n.Despawn());
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _enemies.ForEach(e => e.OnDeath -= CheckWaveWin);
            _citizens.ForEach(c => c.OnDeath -= CheckWaveLose);
        }
    }
}