using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private TextMeshProUGUI healthLabel;
        [SerializeField] private Image healthFiller;
        [SerializeField] private string healthLabelTextTemplate;

        private Camera _camera;
        private float _maxHPValue;

        private void Awake()
        {
            health.OnHealthValueChange += UpdateLabel;
        }

        private void Start()
        {
            _camera = Camera.main;
            _maxHPValue = health.GetMaxHealthInfo();
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            float value = health.GetHealthInfo();
            float percent =  value / _maxHPValue;

            if (healthLabel != null)
            {
                healthLabel.text = string.Format(healthLabelTextTemplate, value);
            }

            Sequence seq = DOTween.Sequence();
            seq.Append(healthFiller.DOFillAmount(percent, 0.5f)).AppendCallback(() => 
            {
                gameObject.SetActive(percent > 0);
            });
        }

        private void LateUpdate()
        {
            if (healthLabel == null)
            {
                transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
                _camera.transform.rotation * Vector3.up);
            }
        }

        private void OnDestroy()
        {
            health.OnHealthValueChange -= UpdateLabel;
        }
    }
}
