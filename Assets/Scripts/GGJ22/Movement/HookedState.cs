using System;
using System.Linq;
using GGJ22.Game;
using GGJ22.Input;
using GGJ22.Traits.Movement.Hook;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.States;
using Shiroi.FX.Effects;
using Shiroi.FX.Features;
using Sirenix.OdinInspector;
using UnityEngine;
namespace GGJ22.Movement {
    public class HookedState : MotorState<BlobInput> {
        public float linecastOffset = 1;
        public WobblyController wobbly;
        public Effect onHookBreak;
        public Hook hook;
        public GroundedState normal;
        public bool current;
        public float airControlStrength = 5;
        public DistanceJoint2D joint;

        private Rigidbody2D _hookedTo;
        private Vector2 _hookTip;
        private float _maxLength;
        public override void Begin(Motor motor, BlobInput input, ref Vector2 velocity) {
            base.Begin(motor, input, ref velocity);
            hook.enabled = false;
            hook.aimIndicator.enabled = false;
            wobbly.DeWobble();
            wobbly.wobbly.lineRenderer.enabled = true;
            joint.enabled = true;
            current = true;
        }
        public override void End(Motor motor, BlobInput input, ref Vector2 velocity) {
            base.End(motor, input, ref velocity);
            wobbly.deWobbling = false;
            hook.enabled = true;
            hook.aimIndicator.enabled = true;
            hook.ForceBeginRetract(_hookTip);
            wobbly.wobbly.lineRenderer.enabled = false;
            joint.enabled = false;
            current = false;
        }
        private void Update() {
            if (current) {
                wobbly.wobbly.origin = transform.position;
                wobbly.wobbly.tip = _hookTip;
            }
        }

        public override void Tick(Motor motor, BlobInput input, ref Vector2 velocity) {
            if (!input.shoot.Current) {
                motor.ActiveState = normal;
                return;
            }
            var horizontal = input.horizontal;
            var inputDir = Math.Sign(horizontal);
            var results = new RaycastHit2D[1];
            var origin = (Vector2) transform.position;

            var end = _hookTip;
            var dir = end - origin;
            dir.Normalize();
            var fallback = -dir;
            fallback *= linecastOffset;
            end += fallback;
            if (Physics2D.LinecastNonAlloc(origin, end, results, GameConfiguration.Instance.worldMask) > 0) {
                motor.ActiveState = normal;
                BreakHook(results.Single().point);
                return;
            }
            velocity.x += airControlStrength * inputDir * motor.GetDirectionControl(inputDir);
        }
        private void BreakHook(Vector2 point) {
            onHookBreak.PlayIfPresent(
                this,
                false,
                new PositionFeature(point)
            );
        }
        public void Attach(Rigidbody2D body, Vector2 point) {
            _hookedTo = body;
            _hookTip = point;
            _maxLength = Vector2.Distance(transform.position, point);
            joint.maxDistanceOnly = true;
            joint.distance = _maxLength;
            joint.connectedAnchor = point;
        }
    }
}