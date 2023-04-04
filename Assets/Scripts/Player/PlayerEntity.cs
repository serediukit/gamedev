using System;
using Core.Enums;
using UnityEngine;
using Core.Tools;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]

    public class PlayerEntity : MonoBehaviour
    {
        [Header("HorizontalMovement")]
        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private Direction _direction;

        [Header("Jump")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _gravityScale;

        [SerializeField] private DirectionalCameraPair _cameras;

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
            if (_direction == Direction.Right && direction < 0 || _direction == Direction.Left && direction > 0)
                Flip();
        }

        private void Flip()
        {
            transform.Rotate(0, 180, 0);
            _direction = _direction == Direction.Right ? Direction.Left : Direction.Right;
            foreach (var cameraPair in _cameras.DirectionalCameras)
                cameraPair.Value.enabled = cameraPair.Key == _direction;
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
