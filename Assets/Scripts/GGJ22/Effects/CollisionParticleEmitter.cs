using System;
using Lunari.Tsuki.Entities;
using Shiroi.FX.Effects;
using Shiroi.FX.Features;
using UnityEngine;
namespace GGJ22.Effects {
    public class CollisionParticleEmitter : Trait {
        public Effect effect;
        private void OnCollisionEnter2D(Collision2D col) {
            effect.PlayIfPresent(this);
        }
    }
}