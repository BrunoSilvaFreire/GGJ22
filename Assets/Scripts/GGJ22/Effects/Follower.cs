using System;
using UnityEngine;
namespace GGJ22.Effects {
    public class Follower : MonoBehaviour {
        public Transform target;
        public float followSpeed;
        public float speedAnimatorMultiplier;
        public Animator animator;
        public float velLerpSpeed = 3;
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Shown = Animator.StringToHash("Shown");
        private Vector2 velocity;

        public float zDistance;
        public void SetShown(bool value) {
            animator.SetBool(Shown, value);
        }
        private void Update() {
            if (target != null) {
                var pos = transform.position;
                var newPos = Vector3.Lerp(pos, target.position, followSpeed * Time.deltaTime);
                var nv = pos - newPos;
                velocity = Vector2.Lerp(
                    velocity,
                    nv,
                    velLerpSpeed * Time.deltaTime
                );
                if (animator != null) {
                    animator.SetFloat(Horizontal, velocity.x * speedAnimatorMultiplier);
                    animator.SetFloat(Vertical, velocity.y * speedAnimatorMultiplier);
                }
                newPos.z = zDistance;
                transform.position = newPos;
            }
        }
    }
}