using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player
{
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private PlayerBase player;

        [SerializeField] private float mouseSensitivityX;
        [SerializeField] private float mouseSensitivityY;


        private float _rotationX;
        private float _rotationY;

        private Vector3 _defaultCameraRotation;
        private Vector3 _defaultPlayerRotation;

        private void Start()
        {
            player.OnInit += SetStartView;
            _defaultCameraRotation = transform.localEulerAngles;
            _defaultPlayerRotation = player.transform.localEulerAngles;
        }

        private void OnDestroy()
        {
            player.OnInit -= SetStartView;
        }

        private void SetStartView()
        {
            transform.localEulerAngles = _defaultCameraRotation;
            player.transform.localEulerAngles = _defaultPlayerRotation;
        }

        private void Update()
        {
            if (player.IsAlive() && player.IsInGame())
            {
                TrackMouse();
            }
        }

        private void TrackMouse()
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityX * Time.deltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivityY * Time.deltaTime;

            _rotationY += mouseX;
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

            transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
            player.transform.rotation = Quaternion.Euler(0f, _rotationY, 0f);
        }
    }
}
