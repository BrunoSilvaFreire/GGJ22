using System;
using Sirenix.OdinInspector;
using UnityEngine;
namespace GGJ22.Traits.Movement.Hook {
    [ExecuteInEditMode]
    public class Wobbly : MonoBehaviour {
        public uint numPoints;
        public Vector2 origin;
        public Vector2 tip;
        public AnimationCurve wobbliness;
        public float wobbleMultiplier;
        [Required]
        public LineRenderer lineRenderer;
        private Vector3[] points;
        private void Start() {
            points = new Vector3[numPoints + 1];
        }
        private void OnValidate() {
            points = new Vector3[numPoints + 1];
        }
        private void Update() {
            if (lineRenderer != null) {
                Compute();
            }
        }
        private void Compute() {
            lineRenderer.positionCount = (int) (numPoints + 1);
            var dir = tip - Vector2.one;
            var forwardAngle = Mathf.Atan2(dir.y, dir.x);
            // Rotate 90 deg
            var upAngleRad = forwardAngle + (Mathf.Deg2Rad * 90);
            var up = new Vector2(
                Mathf.Cos(upAngleRad),
                Mathf.Sin(upAngleRad)
            );
            for (var i = 0; i < numPoints + 1; i++) {
                var t = (float) i / numPoints;
                var wobble = wobbliness.Evaluate(t);
                var offset = wobbleMultiplier * wobble;
                var pos = Vector2.Lerp(origin, tip, t);
                points[i] = pos + (up * offset);
            }
            lineRenderer.SetPositions(points);
        }
    }
}