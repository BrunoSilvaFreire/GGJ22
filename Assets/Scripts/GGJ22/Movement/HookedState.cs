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
        private Rigidbody2D hookedTo;
        private Vector2 hookTip;
        public float linecastOffset = 1;
        public WobblyController wobbly;
        public Effect onHookBreak;
        public LineRenderer indicator;
        public Hook hook;
        public GroundedState normal;
        public bool current;
        public float airControlStrength = 5;
        public float maxHookSpeed;
        public float hookStrength;

        public override void Begin(Motor motor, BlobInput input, ref Vector2 velocity) {
            base.Begin(motor, input, ref velocity);
            hook.enabled = false;
            hook.aimIndicator.enabled = false;
            wobbly.DeWobble();
            wobbly.wobbly.lineRenderer.enabled = true;
            current = true;
        }
        public override void End(Motor motor, BlobInput input, ref Vector2 velocity) {
            base.End(motor, input, ref velocity);
            wobbly.deWobbling = false;
            hook.enabled = true;
            hook.aimIndicator.enabled = true;
            wobbly.wobbly.lineRenderer.enabled = false;
            current = false;
        }
        private void Update() {
            if (current) {
                wobbly.wobbly.origin = transform.position;
                wobbly.wobbly.tip = hookTip;
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

            var end = hookTip;
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
            var control = motor.Control;
            var max = maxHookSpeed * control;
            velocity.x += airControlStrength * inputDir * motor.GetDirectionControl(inputDir);
            velocity += dir * hookStrength;
            velocity = Vector2.ClampMagnitude(velocity, max);
        }
        private void BreakHook(Vector2 point) {
            onHookBreak.PlayIfPresent(
                this,
                false,
                new PositionFeature(point)
            );
        }
        public void Attach(Rigidbody2D body, Vector2 point) {
            hookedTo = body;
            hookTip = point;
        }
    }
}