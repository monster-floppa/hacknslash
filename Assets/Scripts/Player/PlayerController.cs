using UnityEngine;

namespace HacknSlash.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float dashSpeed = 12f;
        [SerializeField] private float dashTime = 0.16f;
        [SerializeField] private float jumpForce = 11f;
        [SerializeField] private float gravity = 25f;

        private CharacterController characterController;
        private PlayerStateMachine stateMachine = new PlayerStateMachine();
        private Vector3 velocity;
        private float dashTimer;
        private bool canDoubleJump;

        public PlayerStateMachine StateMachine { get { return stateMachine; } }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            stateMachine.Initialize(PlayerState.Grounded);
        }

        private void Update()
        {
            UpdateMovement();
            UpdateJump();
            characterController.Move(velocity * Time.deltaTime);
        }

        private void UpdateMovement()
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            input = Vector3.ClampMagnitude(input, 1f);

            if (Input.GetButtonDown("Fire3") && stateMachine.CurrentState != PlayerState.Dashing)
            {
                dashTimer = dashTime;
                stateMachine.ChangeState(PlayerState.Dashing);
            }

            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            if (stateMachine.CurrentState == PlayerState.Dashing)
            {
                dashTimer -= Time.deltaTime;
                speed = dashSpeed;
                if (dashTimer <= 0f)
                {
                    stateMachine.ChangeState(characterController.isGrounded ? PlayerState.Grounded : PlayerState.Falling);
                }
            }

            Vector3 worldMove = transform.TransformDirection(input) * speed;
            velocity.x = worldMove.x;
            velocity.z = worldMove.z;

            if (input.sqrMagnitude > 0.1f)
            {
                Quaternion look = Quaternion.LookRotation(transform.TransformDirection(input));
                transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 12f);
            }
        }

        private void UpdateJump()
        {
            if (characterController.isGrounded)
            {
                canDoubleJump = true;
                if (velocity.y < 0f)
                {
                    velocity.y = -2f;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    velocity.y = jumpForce;
                    stateMachine.ChangeState(PlayerState.Jumping);
                }
                else if (stateMachine.CurrentState != PlayerState.Dashing)
                {
                    stateMachine.ChangeState(PlayerState.Grounded);
                }
            }
            else
            {
                if (Input.GetButtonDown("Jump") && canDoubleJump)
                {
                    canDoubleJump = false;
                    velocity.y = jumpForce;
                    stateMachine.ChangeState(PlayerState.Jumping);
                }

                if (velocity.y < 0f && stateMachine.CurrentState != PlayerState.AirAttacking)
                {
                    stateMachine.ChangeState(PlayerState.Falling);
                }
            }

            velocity.y -= gravity * Time.deltaTime;
        }
    }
}
