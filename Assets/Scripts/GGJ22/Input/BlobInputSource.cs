using Lunari.Tsuki2D.Input.Platformer;
using UnityEngine.InputSystem;
namespace GGJ22.Input {
    public class BlobInputSource : InputSystemPlatformerSource {
        private InputAction shoot;
        private InputAction pull;
        private InputAction vertical;
        public string shootInputName = "shoot";
        public string pullInputName = "pull";
        public string verticalInputName = "vertical";
        protected override void Start() {
            base.Start();
            shoot = input.actions[shootInputName];
            pull = input.actions[pullInputName];
            vertical = input.actions[verticalInputName];
        }
        public bool GetShoot() {
            return shoot.triggered || shoot.inProgress;
        }
        public bool GetPull() {
            return shoot.triggered || shoot.inProgress;
        }
        protected override void TransferTo(PlatformerInput input) {
            base.TransferTo(input);
            if (input is BlobInput i) {
                i.shoot.Current = GetShoot();
                i.pull.Current = GetPull();
                i.vertical = vertical.ReadValue<float>();
            }
        }
    }
}