using UnityEngine;
using System.Collections;

namespace UMA.Examples
{
    public class Locomotion : MonoBehaviour
    {

        protected Animator animator;
        public float DirectionDampTime = .25f;
        private CharacterController characterController;

        void Start()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponentInParent<CharacterController>();

            if (animator == null) return;
            if (animator.layerCount >= 2)
                animator.SetLayerWeight(1, 1);
        }

        void Update()
        {
            if (animator)
            {
                float h = Input.GetAxis("Horizontal");
                float speed = characterController.velocity.magnitude;

                animator.SetFloat("Speed", speed / transform.lossyScale.y);
                animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
            }
            else
            {
                animator = GetComponent<Animator>();
            }
        }


        void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.collider.name + ":" + name);
        }
    }
}
