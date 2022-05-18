using Enemy;
using UnityEngine;

namespace Player
{
    public class AnimationHandler : MonoBehaviour
    {
        private CharacterController _characterController;
        private Animator _animator;

        private float _velocity;
        private float _health;
        private int _velocityHash;
        private int _healthHash;
   
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
          

            _velocityHash = Animator.StringToHash("Velocity");
            _healthHash = Animator.StringToHash("Health");
            
        }

        // Update is called once per frame
        void Update()
        {
            AnimationConditions();
        }

        private void AnimationConditions()
        {
            _velocity = _characterController.velocity.magnitude;
            _health = GetComponent<Health>().GetCurrentHealth();
            _animator.SetFloat(_velocityHash,_velocity);
            _animator.SetFloat(_healthHash,_health);
        }
    }
}