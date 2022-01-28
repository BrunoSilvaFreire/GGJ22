using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Shiroi.FX.Services.BuiltIn.Audio {
    [Serializable]
    public struct AudioEvent {
        [SerializeField]
        private AudioClip clip;

        [SerializeField]
        private ParticleSystem.MinMaxCurve pitch;

        [SerializeField]
        private ParticleSystem.MinMaxCurve volume;

        [SerializeField]
        private float range;

        [SerializeField]
        private bool loop;

        [SerializeField]
        private float loopDuration;

        [SerializeField]
        private Transform attachment;

        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private Vector3 velocity;

        [SerializeField]
        private bool bypassEffects;

        public AudioEvent(
            AudioClip clip,
            ParticleSystem.MinMaxCurve pitch,
            ParticleSystem.MinMaxCurve volume,
            bool loop,
            float loopDuration,
            float range,
            AudioMixerGroup group = null,
            Transform attachment = null
        ) : this() {
            this.clip = clip;
            this.pitch = pitch;
            this.volume = volume;
            Group = group;
            this.loop = loop;
            this.loopDuration = loopDuration;
            this.range = range;
            this.attachment = attachment;
        }


        public AudioEvent(
            AudioClip clip,
            ParticleSystem.MinMaxCurve pitch,
            ParticleSystem.MinMaxCurve volume,
            Vector3 position,
            bool loop,
            float loopDuration,
            float range,
            AudioMixerGroup group = null,
            Vector3 velocity = default) : this() {
            this.clip = clip;
            this.pitch = pitch;
            this.volume = volume;
            this.loop = loop;
            this.loopDuration = loopDuration;
            this.range = range;
            Group = group;
            this.position = position;
            this.velocity = velocity;
        }

        public Vector3 Position => attachment == null ? position : attachment.position;

        public AudioClip Clip => clip;

        public ParticleSystem.MinMaxCurve Pitch => pitch;

        public float Range => range;

        public ParticleSystem.MinMaxCurve Volume => volume;

        public Transform Attachment => attachment;

        public Vector3 Velocity => velocity;

        public AudioMixerGroup Group { get; }

        public bool Loop => loop;

        public float LoopDuration => loopDuration;

        public bool BypassEffects {
            get => bypassEffects;
            set => bypassEffects = value;
        }
    }
}