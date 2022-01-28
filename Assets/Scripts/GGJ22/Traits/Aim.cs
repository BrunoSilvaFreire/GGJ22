using System;
using Lunari.Tsuki.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
namespace GGJ22.Traits {
    public class Aim : Trait {
        public Vector2 worldPosition;
        [Required]
        public new Camera camera;

        public Vector2 AimDirection => worldPosition - (Vector2) transform.position;

        private void Update() {
            if (camera != null) {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                mousePos.z = 0;
                worldPosition = camera.ScreenToWorldPoint(mousePos);
            }
        }
    }
}