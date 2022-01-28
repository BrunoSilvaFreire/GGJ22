using System;
using Lunari.Tsuki;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GGJ22.Movement {
    public class Rope : MonoBehaviour {
        public float maxDistanceBetweenLinks = .5F;
        private float currentLength;
        private HingeJoint2D[] joints;

        [ShowInInspector]
        public float Length {
            get => currentLength;
            set {
                currentLength = value;
                var numLinks = Mathf.FloorToInt(value / maxDistanceBetweenLinks);
                if (numLinks - currentLength > 0) {
                    numLinks += 1;
                }
                var distanceBetweenLinks = value / numLinks;
                DisposeCurrentJoints();
                joints = new HingeJoint2D[numLinks];
                for (var i = 0; i < numLinks; i++) {
                    var (gameObject, hinge) = GameObjects.CreateWith<HingeJoint2D>($"Hinge-{i}");
                }
            }
        }

        private void DisposeCurrentJoints() {
            if (joints != null) {
                return;
            }
            foreach (var joint in joints) {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying) {
                    DestroyImmediate(joint.gameObject);
                    continue;
                }
#endif
                Destroy(joint.gameObject);
            }
        }
    }
}