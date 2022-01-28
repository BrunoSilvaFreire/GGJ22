using GGJ22.Traits.Movement;
using Lunari.Tsuki.Entities;
using UnityEngine;
namespace GGJ22.Traits.Juice {
    [TraitLocation(CommonLocation.View)]
    public class SpriteFlipper : Trait {
        public bool facesRight;
        private Facing facing;
        private SpriteRenderer spriteRenderer;

        private void Update() {
            if (facing != null) {
                UpdateTo(facing.Direction);
            }
        }

        public override void Configure(TraitDescriptor descriptor) {
            facing = descriptor.RequiresComponent<Facing>(CommonLocation.Root);
            spriteRenderer = descriptor.RequiresComponent<SpriteRenderer>(CommonLocation.View);
        }

        public void UpdateTo(float dir) {
            if (dir > 0) {
                spriteRenderer.flipX = facesRight;
            }

            if (dir < 0) {
                spriteRenderer.flipX = !facesRight;
            }
        }
    }
}