using UnityEngine;
namespace GGJ22.Traits.Movement.Hook {
    public class WobblyController : MonoBehaviour {
        public Wobbly wobbly;
        public bool deWobbling;

        public float initialDeWobble = 15;
        public float deWobbleSpeed;
        public float deWobble;

        private void Update() {
            if (deWobbling) {
                deWobble += deWobbleSpeed;
                wobbly.wobbleMultiplier += deWobble * -Mathf.Sign(wobbly.wobbleMultiplier) * Time.deltaTime;
            }
        }
        public void DeWobble() {
            deWobbling = true;
            deWobble = initialDeWobble;
        }
    }
}