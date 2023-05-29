using Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.NPC
{
    public class WavesHolder : MonoBehaviour
    {
        [SerializeField] private PlayerBase player;
        [SerializeField] private List<Wave> waves;

        public Action<bool> OnCurrentWaveEnd;

        private Wave _currentWave;

        public PlayerBase GetPlayer()
        {
            return player;
        }

        public int GetWavesCount()
        {
            return waves.Count;
        }

        public List<NPCBase> GetCitezens(bool isAlive = false)
        {

            if (_currentWave == null)
            {
                return null;
            }

            List<NPCBase> citizens = _currentWave.GetNPCs().Where(n => n.GetNPCType() == NPC_Type.Citizen).ToList();

            if (isAlive)
            {
                citizens = citizens.Where(c => c.IsAlive()).ToList();
            }

            return citizens;
        }

        public NPCBase GetNPC(GameObject npc)
        {
            return _currentWave.GetNPC(npc);
        }

        public List<NPCBase> GetEnemies(NPCBase self)
        {
            if (_currentWave == null)
            {
                return null;
            }

            List<NPCBase> enemies = _currentWave.GetNPCs().Where(n => n.GetNPCType() != NPC_Type.Citizen && n != self).ToList();
            return enemies;
        }

        public NPCBase GetRandomNPC(NPC_Type type, bool isAlive = false)
        {
            var list = _currentWave.GetTypeNPCs(type, isAlive);
            NPCBase randomNPC = _currentWave.GetRandomNPC(list);
            return randomNPC;
        }

        public void StartWave(int waveId)
        {
            if (_currentWave != null)
            {
                _currentWave.OnWaveEnd -= EndCurrentWave;
                _currentWave = null;
            }
            _currentWave = waves.Find(w => w.GetId() == waveId);
            _currentWave.StartWave();
            _currentWave.OnWaveEnd += EndCurrentWave;
        }

        public void EnemiesWin()
        {
            var aliveEnemies = _currentWave.GetTypeNPCs(NPC_Type.Enemy, true);
            aliveEnemies.ForEach(e => e.Victory());
        }

        private void EndCurrentWave(bool state)
        {
            StartCoroutine(EndWaveCor(state));
        }

        private IEnumerator EndWaveCor(bool state)
        {
            yield return new WaitForSeconds(5f);
            WaveEnd(state);
        }

        private void WaveEnd(bool state)
        {
            EndCurrentWave();
            OnCurrentWaveEnd?.Invoke(state);
        }

        public void EndCurrentWave()
        {
            _currentWave.EndWave();
        }

        private void OnDestroy()
        {
            if (_currentWave != null)
            {
                _currentWave.OnWaveEnd -= EndCurrentWave;
            }
        }
    }
}
