using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class WalletUI : MonoBehaviour
    {
        [SerializeField] private Wallet wallet;
        [SerializeField] private TextMeshProUGUI valueLabel;

        private void Awake()
        {
            wallet.OnValueChange += UpdateLabel;
        }

        private void Start()
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            int value = wallet.GetValue();
            valueLabel.text = value.ToString();
        }

        private void OnDestroy()
        {
            wallet.OnValueChange -= UpdateLabel;
        }
    }
}
