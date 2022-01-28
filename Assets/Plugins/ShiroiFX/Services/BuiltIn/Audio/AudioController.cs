using System.Collections;
using Lunari.Tsuki.Singletons;
using UnityEngine;

namespace Shiroi.FX.Services.BuiltIn.Audio {
    [RequireComponent(typeof(AudioPool))]
    public class AudioController : Singleton<AudioController> {
        public AudioPool Pool;

        public void PlayAudioEvent(AudioEvent audioEvent) {
            var source = Pool.Get();
            if (source == null) {
                Debug.LogWarning(
                    $"Unable to play audio event {audioEvent} because the audio pool was unable to provide an Audio Source"
                );
                return;
            }

            source.bypassEffects = audioEvent.BypassEffects;
            source.PlayOneShot(audioEvent.Clip);
            StartCoroutine(RunAudioEvent(source, audioEvent));
        }

        private IEnumerator RunAudioEvent(AudioSource source, AudioEvent audio) {
            float currentTime = 0;
            var attachment = audio.Attachment;
            if (attachment != null) {
                source.transform.parent = attachment;
            } else {
                source.transform.position = audio.Position;
            }

            source.minDistance = audio.Range;
            source.outputAudioMixerGroup = audio.Group;
            var loop = audio.Loop;
            source.loop = loop;
            var duration = loop ? audio.LoopDuration : audio.Clip.length;
            source.pitch = audio.Pitch.Evaluate(Random.value);
            while ((currentTime += Time.deltaTime) < duration) {
                source.transform.position += audio.Velocity * Time.deltaTime;
                var pos = currentTime / duration;
                source.volume = audio.Volume.Evaluate(pos);
                //source.pitch = audio.Pitch.Evaluate(pos);
                yield return null;
            }

            Pool.Return(source);
        }
    }
}