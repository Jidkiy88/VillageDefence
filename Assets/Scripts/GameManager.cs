using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using Scripts.Player;
using UnityEngine;
using Scripts.NPC;

namespace Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerBase player;
        [SerializeField] private DeathWindow deathWindow;
        [SerializeField] private Wallet wallet;
        [SerializeField] private Timer timer;
        [SerializeField] private WavesHolder wavesHolder;
        [SerializeField] private WeaponsHolder weaponsHolder;
        [SerializeField] private Light sun;
        [SerializeField] private Vector3 sunEndRotation;
        [SerializeField] private float sunCycleDuration;
        [SerializeField] private float sunEndIntensity;

        private Vector3 _sunStartRotation;
        private float _sunStartIntensity;
        private int _wavesCount;

        private bool _isPaused = false;
        private bool _isDay = true;

        private readonly string _savedWaveIdKey = "SAVEDWAVEIDKEY";


        private void Start()
        {
            timer.OnTimeOut += SwitchDayTime;
            player.OnDeath += OnPlayerDeath;
            _sunStartIntensity = sun.intensity;
            _sunStartRotation = sun.transform.rotation.eulerAngles;
            _wavesCount = wavesHolder.GetWavesCount();
            timer.StartCountDown();
        }

        private void OnPlayerDeath()
        {
            deathWindow.Open();
            wavesHolder.EnemiesWin();
        }

        public void RestartWave()
        {
            wavesHolder.EndCurrentWave();
            SetDayState(true);
            player.Initialize();
            Action callback = () => timer.StartCountDown();
            deathWindow.Close(callback);
        }

        public void ClearProgress()
        {
            DeleteSavedInfo();
            wallet.Initialize();
            RestartWave();
        }

        private void DeleteSavedInfo()
        {
            PlayerPrefs.DeleteKey(_savedWaveIdKey);
            PlayerPrefs.DeleteKey(wallet.GetWalletKey());
            weaponsHolder.ClearHolder();
            PlayerPrefs.DeleteAll();
        }

        private void SaveProgress(bool state)
        {
            wavesHolder.OnCurrentWaveEnd -= OnWaveEnd;

            if (state)
            {
                int oldWaveId = PlayerPrefs.GetInt(_savedWaveIdKey, 1);
                int nextWaveId = oldWaveId + 1;
                if (nextWaveId > _wavesCount)
                {
                    FinishGame();
                    return;
                }
                PlayerPrefs.SetInt(_savedWaveIdKey, oldWaveId + 1);
                PlayerPrefs.Save();
            }

            timer.StartCountDown();
        }

        private void FinishGame()
        {
            PlayerPrefs.DeleteKey(_savedWaveIdKey);
            wavesHolder.EndCurrentWave();
            SetDayState(true);
            player.Initialize();
            timer.StartCountDown();
        }

        private void LoadWave()
        {
            int waveId = PlayerPrefs.GetInt(_savedWaveIdKey, 1);
            wavesHolder.StartWave(waveId);
            wavesHolder.OnCurrentWaveEnd += OnWaveEnd;
        }

        private void OnWaveEnd(bool state)
        {
            if (state)
            {
                SaveProgress(state);
                SwitchDayTime();
            }
            else
            {
                player.LoseControl();
                OnPlayerDeath();
            }
        }

        private void SetDayState(bool state)
        {
            _isDay = state;
            Vector3 rotation = _isDay ? _sunStartRotation : sunEndRotation;
            float intensity = _isDay ? _sunStartIntensity : sunEndIntensity;
            sun.intensity = intensity;
            sun.transform.localEulerAngles = rotation;
        }

        private void SwitchDayTime()
        {
            Vector3 rotation = _isDay ? sunEndRotation : _sunStartRotation;
            float intensity = _isDay ? sunEndIntensity : _sunStartIntensity;

            Sequence seq = DOTween.Sequence();
            seq.Append(sun.DOIntensity(intensity, sunCycleDuration)).Join(sun.transform.DOLocalRotate(rotation, sunCycleDuration)).AppendCallback(() =>
            {
                if (_isDay)
                {
                    LoadWave();
                }

                _isDay = !_isDay;
            });
        }

        private void OnDestroy()
        {
            timer.OnTimeOut -= SwitchDayTime;
            player.OnDeath -= OnPlayerDeath;
        }
    }
}
