using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Suriyun
{
    public class ControllerHamster : MonoBehaviour
    {

        public float stopRate = 0.5f;

        public KeyCode key_Forward = KeyCode.D;
        public KeyCode key_Backward = KeyCode.A;
        public KeyCode key_Jump = KeyCode.Space;
        public KeyCode key_Dash = KeyCode.LeftShift;

        public float forwardProfileAngle = 120f;
        public float backwardProfileAngle = 240f;

        public float mass = 1f;
        public float walkSpeed = 3f;
        public float dashSpeed = 6f;
        public float turnSpeed = 0.5f;
        public float jumpPower = 1f;
        public float gravity_multiplier = 2f;
        public float alert_time = 3f;

        protected float jumpImpact = 0;
        protected Vector3 move = Vector3.zero;
        protected float targetSpeed = 0;
        protected float time_to_idle = 0f;
        protected float targetTurnDegree = 0;

        protected CharacterController ctrl;
        protected Transform trans;
        protected Animator animator;
        protected RuntimeAnimatorController animator_controller;

        private Vector3 lastPos;
        //private int state = 0;

        void Awake()
        {
            time_to_idle = alert_time;
            trans = GetComponent<Transform>();
            ctrl = GetComponent<CharacterController>();
            lastPos = trans.position;
            targetTurnDegree = forwardProfileAngle;

            if (animator == null)
                animator = gameObject.GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogError("Missing : animator.");

            animator_controller = animator.runtimeAnimatorController;
            if (animator_controller == null)
                Debug.LogError("Missing : animator_controller.");
        }

        void Update()
        {
            if (lastPos == trans.position)
            {
                time_to_idle -= Time.deltaTime;
                if (time_to_idle < 0)
                {
                    if (Random.value * 2 < 1)
                        animator.SetTrigger("Idle1");
                    else
                        animator.SetTrigger("Idle2");
                    time_to_idle = alert_time;
                }
            }
            else
            {
                time_to_idle = alert_time;
            }

            if (Input.GetKey(key_Forward))
            {
                if (Input.GetKey(key_Dash))
                    targetSpeed = dashSpeed;
                else
                    targetSpeed = walkSpeed;

                targetTurnDegree = forwardProfileAngle;
            }
            else if (Input.GetKey(key_Backward))
            {
                if (Input.GetKey(key_Dash))
                    targetSpeed = -dashSpeed;
                else
                    targetSpeed = -walkSpeed;

                targetTurnDegree = backwardProfileAngle;
            }
            else
            {
                targetSpeed = 0;
            }

            move.x = Mathf.Lerp(move.x, targetSpeed, stopRate * Time.deltaTime);
            move.y = Physics.gravity.y * gravity_multiplier;

            if (ctrl.isGrounded)
            {
                jumpImpact = 0;
                if (Input.GetKeyDown(key_Jump))
                {
                    Debug.Log("Jump");
                    animator.SetTrigger("Jump");
                    jumpImpact = jumpPower / mass;
                }
            }

            animator.SetFloat("Speed", ctrl.velocity.magnitude / dashSpeed);
            lastPos = trans.position;

            UpdateMove();
        }


        void UpdateMove()
        {
            jumpImpact = Mathf.Lerp(jumpImpact, 0, Time.deltaTime);
            move.y += jumpImpact;

            ctrl.Move(move * Time.deltaTime);

            Vector3 eulerAngle = this.transform.rotation.eulerAngles;
            eulerAngle.y = Mathf.Lerp(eulerAngle.y, targetTurnDegree, turnSpeed * Time.deltaTime);
            this.transform.rotation = Quaternion.Euler(eulerAngle);
        }


        //public virtual void ClearState()
        //{
        //    state = 0;
        //}
        //
        //public virtual void Sit()
        //{
        //    state = 1;
        //}
        //
        //public virtual void Die()
        //{
        //    state = 2;
        //}
        //
        //public virtual void Cheer()
        //{
        //    state = 3;
        //}

        //void OnGUI()
        //{
        //    if (GUILayout.Button("Sit"))
        //    {
        //        Sit();
        //    }
        //    if (GUILayout.Button("Die"))
        //    {
        //        Die();
        //    }
        //    if (GUILayout.Button("Cheer"))
        //    {
        //        Cheer();
        //    }
        //}
    }
}
