using System;
using Core.Enums;
using UnityEngine;
using Core.Tools;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]

    public class PlayerEntity : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [Header("HorizontalMovement")]
        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private Direction _direction;

        [Header("Jump")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _gravityScale;

        [SerializeField] private DirectionalCameraPair _cameras;

        private Rigidbody2D _rigidbody;

        private bool _isJumping;
        private bool _isAttacking;
        private bool _isDashing;

        private Vector2 _movement;
        private AnimationType _currentAnimationType;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _isAttacking = false;
            _isDashing = false;
        }

        void Update()
        {
            if(_isJumping)
                UpdateJump();

            UpdateAnimations();
        }

        private void UpdateAnimations()
        {
            PlayAnimation(AnimationType.Idle, true);
            PlayAnimation(AnimationType.Run, _movement.magnitude > 0);
            PlayAnimation(AnimationType.Attack, _isAttacking);
            PlayAnimation(AnimationType.Dash, _isDashing);
            PlayAnimation(AnimationType.Jump, _isJumping);
        }

        public void MoveHorizontally(float direction)
        {
            _movement.x = direction;
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
            return Physics2D.Raycast(transform.position, -Vector2.up, 0.01f) && _rigidbody.velocity.y < 0;
        }

        private void PlayAnimation(AnimationType animationType, bool active)
        {
            if (!active)
            {
                if (_currentAnimationType == AnimationType.Idle || _currentAnimationType != animationType)
                    return;

                _currentAnimationType = AnimationType.Idle;
                PlayAnimation(_currentAnimationType);
                return;
            }

            if (_currentAnimationType >= animationType)
                return;

            _currentAnimationType = animationType;
            PlayAnimation(_currentAnimationType);
        }

        private void PlayAnimation(AnimationType animationType)
        {
            _animator.SetInteger(nameof(AnimationType), (int)animationType);
        }
    }
}
