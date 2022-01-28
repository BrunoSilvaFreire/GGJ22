using Lunari.Tsuki.Editor;
using Shiroi.FX.Effects.BuiltIn;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

namespace Shiroi.FX.Editor.Editors {
    [CustomEditor(typeof(AudioEffect))]
    [CanEditMultipleObjects]
    public class AudioEffectEditor : UnityEditor.Editor {
        public static readonly GUIContent HeaderTitle = new GUIContent("Audio Effect");
        public static readonly GUIContent HeaderSubtitle = new GUIContent("An audio effect plays an audio dynamically");
        public static readonly GUIContent GeneralTitle = new GUIContent("General");
        public static readonly GUIContent GeneralSubtitle = new GUIContent("Clips, volume and pitch configuration");
        public static readonly GUIContent LoopTitle = new GUIContent("Looping");
        public static readonly GUIContent LoopSubtitle = new GUIContent("Loop information, such as duration and mode");
        private AnimBool bypassEffect;

        private ReorderableList clipList;
        private EffectHeader header;
        private AnimBool loops;

        private AnimBool showPitch;

        private void OnEnable() {
            header = new EffectHeader(typeof(AudioEffect));
            loops = new AnimBool();
            bypassEffect = new AnimBool();
            loops.valueChanged.AddListener(Repaint);
            bypassEffect.valueChanged.AddListener(Repaint);
            var clips = serializedObject.FindProperty("Clips");
            clipList = new ReorderableList(serializedObject, clips) {
                drawElementCallback = DrawClip,
                drawHeaderCallback = DrawClipsHeader
            };
            showPitch = new AnimBool();
            showPitch.valueChanged.AddListener(Repaint);
        }

        private void DrawClipsHeader(Rect rect) {
            EditorGUI.LabelField(rect, "Audio Clips");
        }

        private void DrawClip(Rect rect, int index, bool isactive, bool isfocused) {
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                serializedObject.FindProperty("Clips").GetArrayElementAtIndex(index),
                GUIContent.none);
        }

        public override void OnInspectorGUI() {
            header.DoLayout();
            EditorGUILayout.BeginVertical(Styles.box);
            ShiroiFXGUI.DrawTitle(HeaderTitle, HeaderSubtitle);
            EditorGUI.BeginChangeCheck();
            DrawVolume();
            DrawLoop();
            if (EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawLoop() {
            ShiroiFXGUI.DrawTitle(LoopTitle, LoopSubtitle);
            ShiroiFXGUI.DrawFadeProperty(
                ref loops,
                serializedObject.FindProperty("Loop"),
                delegate {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("LoopDuration"));
                    EditorGUI.indentLevel--;
                }
            );
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AudioEffect.BypassEffects)));
        }

        private void DrawVolume() {
            ShiroiFXGUI.DrawTitle(GeneralTitle, GeneralSubtitle);
            clipList.DoLayoutList();
            var useAudioController = serializedObject.FindProperty("UseAudioControllerIfPresent");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Volume"));
            if (!showPitch.target) {
                var volume = ((AudioEffect) target).Volume;
                var volumeCurve = volume.Value;
                if (volumeCurve.mode == ParticleSystemCurveMode.Curve ||
                    volumeCurve.mode == ParticleSystemCurveMode.TwoCurves) {
                    serializedObject.ApplyModifiedProperties();
                    volumeCurve.mode = ParticleSystemCurveMode.Constant;
                    volume.Value = volumeCurve;
                    serializedObject.Update();
                }
            }

            using (var group = new EditorGUILayout.FadeGroupScope(showPitch.faded)) {
                if (group.visible) {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Pitch"));
                }
            }

            EditorGUILayout.PropertyField(useAudioController);
            showPitch.target = useAudioController.boolValue;
        }
    }
}