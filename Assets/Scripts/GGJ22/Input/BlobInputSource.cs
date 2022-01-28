using Lunari.Tsuki2D.Input.Platformer;
using UnityEngine.InputSystem;
namespace GGJ22.Input {
    public class BlobInputSource : InputSystemPlatformerSource {
        private InputAction shoot;
        public string shootInputName = "shoot";
        protected override void Start() {
            base.Start();
            shoot = input.actions[shootInputName];
        }
        public bool GetShoot() {
            return shoot.triggered || shoot.inProgress;
        }
        protected override void TransferTo(PlatformerInput input) {
            base.TransferTo(input);
            if (input is BlobInput i) {
                i.shoot.Current = GetShoot();
            }
        }
    }
}