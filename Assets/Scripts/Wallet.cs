using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Wallet : MonoBehaviour
    {
        private int coinsValue;

        private readonly string _walletKey = "WalletKey";

        public Action OnValueChange;

        private void Awake()
        {
            Initialize();
        }

        public string GetWalletKey()
        {
            return _walletKey;
        }

        public void Initialize()
        {
            coinsValue = PlayerPrefs.HasKey(_walletKey) ? PlayerPrefs.GetInt(_walletKey) : 0;
            OnValueChange?.Invoke();
            Save();
        }

        public void IncreaseValue(int value)
        {
            coinsValue += value;
            OnValueChange?.Invoke();
            Save();
        }

        public void DecreaseValue(int value)
        {
            coinsValue -= value;
            OnValueChange?.Invoke();
            Save();
        }

        private void Save()
        {
            PlayerPrefs.SetInt(_walletKey, coinsValue);
            PlayerPrefs.Save();
        }

        public int GetValue()
        {
            return coinsValue;
        }
    }
}
