using UnityEngine;

namespace Main.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Physics parameters")]
        public float moveForce = 30f;
        public float maxControlSpeed = 10f;
        public float controlInAirMultiplier = 0.75f;
        public float fallingMultiplier = 15f;
        
        [Header("Ground Layer")]
        public LayerMask groundLayer;
        
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, .6f, groundLayer);
            if (_rb.linearVelocityY < 0 && !isGrounded)
            {
                _rb.linearVelocityY -= fallingMultiplier * Time.fixedDeltaTime;
            }
            
            float input = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(input) < 0.01f) return;  // CODE BELOW ONLY EXECUTES IF PLAYER IS GIVING INPUT
            
            float controlMultiplier = isGrounded ? 1f : controlInAirMultiplier;  // Decrease air horizontal control

            // Only add force if we're under max control speed in input direction
            if (Mathf.Approximately(Mathf.Sign(input), Mathf.Sign(_rb.linearVelocityX)) &&
                !(Mathf.Abs(_rb.linearVelocityX) < maxControlSpeed)) return;
            
            float velocityInInputDir = _rb.linearVelocityX * input;
            float forceScale = Mathf.Clamp01(1f - (velocityInInputDir / maxControlSpeed));
            
            // I won't lie gpt cooked this weird equation, but it feels good so we move.
            _rb.AddForce(Vector2.right * (input * moveForce * forceScale * controlMultiplier), ForceMode2D.Force);
        }
    }
}