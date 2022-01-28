using System;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki2D.Input.Platformer;
using Lunari.Tsuki2D.Runtime.Movement;
namespace GGJ22.Traits.Movement {
    [TraitLocation("Movement")]
    public class Facing : Trait {
        private int direction;
        private Motor motor;

        public int Direction {
            get {
                if (motor == null) {
                    return 0;
                }
                if (motor.TryGetInput(out PlatformerInput input)) {
                    var dir = Math.Sign(input.horizontal);
                    if (dir != 0) {
                        return dir;
                    }
                }

                return direction;
            }
            set {
                direction = value;
                if (motor.TryGetInput(out PlatformerInput input)) {
                    input.horizontal = value;
                }
            }
        }

        private void Update() {
            direction = Direction;
        }

        public override void Configure(TraitDescriptor descriptor) {
            descriptor.DependsOn(out motor);
        }
    }
}