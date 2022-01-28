using Lunari.Tsuki.Entities;
using Lunari.Tsuki2D.Input.Platformer;
using Lunari.Tsuki2D.Runtime.Movement;
using UnityEngine;
namespace GGJ22.Traits.Juice {
    [TraitLocation(TraitLocations.View)]
    public class GroundedAnimatorBinder : Trait {
        public string speedPercentKey = "SpeedPercent";
        public string speedRawKey = "SpeedRaw";
        public string groundedKey = "Grounded";
        public string ySpeedKey = "YSpeed";
        public string absXInput = "absXInput";
        private Motor motor;

        public override void Configure(TraitDescriptor descriptor) {
            var hasInput = descriptor.RequiresMotorInputOfType(out motor, out PlatformerInput input);
            if (descriptor.DependsOn(out AnimatorBinder binder)) {
                if (hasInput) {
                    binder.BindFloat(absXInput, () => Mathf.Abs(input.horizontal));
                }

                binder.BindFloat(ySpeedKey, () => motor.rigidbody.velocity.y);
                binder.BindFloat(speedRawKey, () => motor.rigidbody.velocity.x);
                binder.BindFloat(speedPercentKey, () => motor.rigidbody.velocity.magnitude / motor.maxSpeed);
                binder.BindBool(groundedKey, () => motor.supportState.down);
            }

            descriptor.RequiresAnimatorParameter(speedPercentKey, AnimatorControllerParameterType.Float);
            descriptor.RequiresAnimatorParameter(speedRawKey, AnimatorControllerParameterType.Float);
            descriptor.RequiresAnimatorParameter(groundedKey, AnimatorControllerParameterType.Bool);
            descriptor.RequiresAnimatorParameter(ySpeedKey, AnimatorControllerParameterType.Float);
            descriptor.RequiresAnimatorParameter(absXInput, AnimatorControllerParameterType.Float);
        }
    }
}