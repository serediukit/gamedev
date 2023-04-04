using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]

    public class PlayerEntity : MonoBehaviour
    {
        [Header("HorizontalMovement")]
        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private bool _faceRight;

        [Header("Jump")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _gravityScale;

        private Rigidbody2D _rigidbody;

        private bool _isJumping;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if(_isJumping)
                UpdateJump();
        }

        public void MoveHorizontally(float direction)
        {
            SetDirection(direction);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = direction * _horizontalSpeed;
            _rigidbody.velocity = velocity;
        }

        public void Jump()
        {
            if (_isJumping)
                return;

            _isJumping = true;
            _rigidbody.AddForce(Vector2.up * _jumpForce);
            _rigidbody.gravityScale = _gravityScale;
        }

        private void SetDirection(float direction)
        {
            if (_faceRight && direction < 0 || !_faceRight && direction > 0)
                Flip();
        }

        private void Flip()
        {
            transform.Rotate(0, 180, 0);
            _faceRight = !_faceRight;
        }

        private void UpdateJump()
        {
            if(IsGrounded())
                _isJumping = false;
        }

        private bool IsGrounded()
        {
            return Physics2D.Raycast(transform.position, -Vector2.up, 0.01f);
        }
    }
}
