using System;
using GGJ22.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
namespace GGJ22.Effects {
    public class IntroBlocker : MonoBehaviour {
        private bool started;
        public BlobInputSource source;
        public UnityEvent onBegin;
        public Animator cardAnimator;
        private static readonly int Hide = Animator.StringToHash("Hide");
        private void Begin() {
            source.enabled = true;
            started = true;
            enabled = false;
            onBegin.Invoke();
            cardAnimator.SetTrigger(Hide);
        }
        private void Start() {
            source.enabled = false;
        }
        private void Update() {
            if (!started && PressedAnything()) {
                Begin();
            }
        }
        private bool PressedAnything() {
            return Keyboard.current.anyKey.isPressed;
        }
    }
}