using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private PlayerBase player;
        [SerializeField] private Transform orientation;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float speed;
        [SerializeField] private float busySpeed;
        [SerializeField] [Range(0f, 0.5f)] private float moveSmoothTime;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpHeight;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Collider collider;
        [SerializeField] private float interactDistance;
        [SerializeField] private LayerMask interactableLayer;

        [SerializeField] private WeaponsHolder weaponsHolder;

        private Vector2 _currentDirection;
        private Vector2 _currentDirectionVelocity;

        private float _velocityY;
        private float _horizontalInput;
        private float _verticalInput;
        private float _defaultSpeed;
        private bool _isGrounded;
        private bool _canJump = true;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _defaultSpeed = speed;
            weaponsHolder.OnWeaponSwitch += SetPlayersWeapon;
            SetPlayersWeapon();
        }

        private void Update()
        {
            if (player.IsAlive() && player.IsInGame())
            {
                GroundChecker();
                ReadInput();
                PlayerMovement();
            }
        }

        public void SetSpeed(bool state)
        {
            speed = state ? busySpeed : _defaultSpeed;
        }

        private void SetPlayersWeapon()
        {
            player.SetWeapon(weaponsHolder.GetCurrentWeapon());
        }

        private void GroundChecker()
        {
            bool oldState = _isGrounded;
            _isGrounded = Physics.Raycast(collider.bounds.center, Vector3.down, collider.bounds.size.y * 0.5f + 0.2f, groundLayer);
            if (!_isGrounded && controller.velocity.y < -1f)
            {
                _velocityY = -8f;
            }
        }

        private void ReadInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(KeyCode.Space) && _isGrounded && _canJump)
            {
                StartCoroutine(JumpCoroutine());
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                weaponsHolder.WeaponAttack();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                weaponsHolder.WeaponBlock();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryToInteract();
            }
        }

        private void TryToInteract()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, transform.forward, out hit, interactDistance, interactableLayer))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        private IEnumerator JumpCoroutine()
        {
            _canJump = false;
            _velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
            yield return new WaitForSeconds(jumpCooldown);
            _canJump = true;
        }

        private void PlayerMovement()
        {
            Vector2 targetDir = new Vector2(_horizontalInput, _verticalInput);
            targetDir.Normalize();
            _currentDirection = Vector2.SmoothDamp(_currentDirection, targetDir, ref _currentDirectionVelocity, moveSmoothTime);
            _velocityY += gravity * 2f * Time.deltaTime;
            Vector3 velocity = (transform.forward * _currentDirection.y + transform.right * _currentDirection.x) * speed + Vector3.up * _velocityY;
            controller.Move(velocity * Time.deltaTime);
        }

        private void OnDestroy()
        {
            weaponsHolder.OnWeaponSwitch -= SetPlayersWeapon;
        }
    }
}
