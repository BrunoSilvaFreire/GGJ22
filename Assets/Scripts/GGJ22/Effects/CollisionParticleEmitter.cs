using System;
using Lunari.Tsuki.Entities;
using UnityEngine;
namespace GGJ22.Effects {
    public class CollisionParticleEmitter : Trait {
        public ParticleSystem system;
        private void OnCollisionEnter2D(Collision2D col) {
            system.Play();
        }
    }
}