using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cschmid.BoxPlatformer.MonoBehaviors
{
    public class PlayerControls : MonoBehaviour
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _jumpStrength;
        [SerializeField]
        private float _rotationStrength;

        private Rigidbody2D _rb2d;

        private Transform _jumpTriggerTransform;
        private TriggerHandler _jumpTriggerHandler;

        void Start()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _jumpTriggerHandler = transform.Find("JumpCollider").GetComponent<TriggerHandler>();
            _jumpTriggerTransform = _jumpTriggerHandler.transform;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            _jumpTriggerTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            float inputX = Input.GetAxis("Horizontal");

            Vector2 position = _rb2d.position;
            position.x += inputX * _speed * Time.deltaTime;
            _rb2d.position = position;

            _rb2d.SetRotation(_rb2d.rotation + (-5f * inputX * _rotationStrength));

            if (Input.GetKeyDown(KeyCode.Space) && _jumpTriggerHandler.IsTriggered)
            {
                Vector2 velocity = _rb2d.velocity;
                velocity.y = _jumpStrength;
                _rb2d.velocity = velocity;
            }
        }
    }
}
