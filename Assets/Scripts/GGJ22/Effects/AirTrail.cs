using System;
using Lunari.Tsuki;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki2D.Runtime.Movement;
using Sirenix.OdinInspector;
using UnityEngine;
namespace GGJ22.Effects {
    public class AirTrail : Trait {
        private Motor _motor;
        [Required]
        public ParticleSystem trail;
        public float maxEmissionRate = 30;
        public AnimationCurve amount;
        public override void Configure(TraitDescriptor descriptor) {
            if (descriptor.DependsOn(out _motor)) { }
        }
        private void Update() {
            if (trail == null || _motor == null) {
                return;
            }

            var value = amount.Evaluate(
                _motor.rigidbody.velocity.magnitude / _motor.maxSpeed
            ) * maxEmissionRate;
            var module = trail.emission;
            module.rateOverTimeMultiplier = value;
        }
    }
}