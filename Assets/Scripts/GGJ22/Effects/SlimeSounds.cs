using System;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki2D.Runtime.Movement;
using Lunari.Tsuki2D.Runtime.Movement.Attachments;
using Shiroi.FX.Effects;
using UnityEngine;
namespace GGJ22.Effects {
    public class SlimeSounds : Trait {
        public Motor _motor;
        public AudioSource source;
        public AnimationCurve volume;
        public float soundAdjustmentSpeed = 100;
        public override void Configure(TraitDescriptor descriptor) {
            descriptor.DependsOn(out _motor);
        }
        private void Update() {
            float targetVolume;
            if (_motor.supportState.down) {
                targetVolume = volume.Evaluate(_motor.rigidbody.velocity.magnitude / _motor.maxSpeed);
            } else {
                targetVolume = 0;
            }
            source.volume = Mathf.Lerp(source.volume, targetVolume, soundAdjustmentSpeed * Time.deltaTime);
        }
    }
}