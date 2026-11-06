using UnityEngine;

namespace DesertQuiet.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class ThirdPersonMobileController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float gravity = -20f;

        private CharacterController characterController;
        private Vector2 moveInput;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        // Connect a mobile joystick's Vector2 output to this method.
        public void SetMoveInput(Vector2 input)
        {
            moveInput = Vector2.ClampMagnitude(input, 1f);
        }

        private void Update()
        {
            Vector3 forward = cameraTransform != null
                ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized
                : Vector3.forward;
            Vector3 right = cameraTransform != null
                ? Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized
                : Vector3.right;

            Vector3 move = (forward * moveInput.y + right * moveInput.x);
            if (move.sqrMagnitude > 1f) move.Normalize();

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime);
            }

            if (characterController.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
