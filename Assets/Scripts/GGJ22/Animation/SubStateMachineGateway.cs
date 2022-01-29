using System;
using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace GGJ22.Animation {
    public class SubStateMachineGateway : StateMachineBehaviour {
        [Serializable]
        public struct ConditionNode {
            public enum BitMode {
                Self,
                All,
                Any,
                None
            }
            [HideIf(nameof(IsSelf))]
            public List<ConditionNode> children;
            public BitMode mode;
            [ShowIf(nameof(IsSelf))]
            public int parameter;
            public bool boolValue;
            public int intValue;
            public float floatValue;
            public ConditionType type;
            public enum ConditionType {
                Equal,
                NotEqual,
                Greater,
                Less
            }

            private bool IsSelf() {
                return mode == BitMode.Self;
            }

            public bool IsMet(Animator animator) {
                switch (mode) {
                    case BitMode.All:
                        return children.All(node => node.IsMet(animator));
                    case BitMode.Any:
                        return children.Any(node => node.IsMet(animator));
                    case BitMode.None:
                        return !children.Any(node => node.IsMet(animator));
                    case BitMode.Self:
                        return CheckSelf(animator);
                    default:
                        return false;
                }
            }
            private bool CheckSelf(Animator animator) {
                var p = animator.GetParameter(parameter);
                switch (p.type) {
                    case AnimatorControllerParameterType.Float:
                        return CheckFloat(animator.GetFloat(p.nameHash));
                    case AnimatorControllerParameterType.Int:
                        return CheckInt(animator.GetInteger(p.nameHash));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        return CheckBool(animator.GetBool(p.nameHash));
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return false;
            }
            private bool CheckBool(bool value) {
                switch (type) {
                    case ConditionType.Equal:
                        return value == boolValue;
                    case ConditionType.NotEqual:
                        return value != boolValue;

                }
                return false;
            }
            private bool CheckInt(int value) {
                switch (type) {
                    case ConditionType.Equal:
                        return value == intValue;
                    case ConditionType.NotEqual:
                        return value != intValue;
                    case ConditionType.Greater:
                        return value > intValue;
                    case ConditionType.Less:
                        return value < intValue;
                    default:
                        return false;
                }
            }
            private bool CheckFloat(float value) {
                switch (type) {
                    case ConditionType.Equal:
                        return Mathf.Approximately(value, floatValue);
                    case ConditionType.NotEqual:
                        return !Mathf.Approximately(value, floatValue);
                    case ConditionType.Greater:
                        return value > floatValue;
                    case ConditionType.Less:
                        return value < floatValue;
                    default:
                        return false;
                }
            }
        }
        public ConditionNode condition;
        public int destination;
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) {
            if (condition.IsMet(animator)) {
                Debug.Log("Moving");
                animator.Play(destination);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(SubStateMachineGateway))]
    public class SubStateMachineGatewayEditor : OdinEditor {
        private SubStateMachineGateway _gateway;
        private AnimatorController _controller;
        private int _layer;
        private void OnEnable() {
            _gateway = (SubStateMachineGateway) target;
            var assetPath = AssetDatabase.GetAssetPath(_gateway);
            _controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);

            for (var i = 0; i < _controller.layers.Length; i++) {
                var layer = _controller.layers[i];
                var stateMachine = layer.stateMachine;
                var behaviours = stateMachine.behaviours;
                if (behaviours == null || !behaviours.Contains(_gateway)) {
                    continue;
                }
                _layer = i;

            }
        }
        public override void OnInspectorGUI() {


            var root = _controller.layers[_layer].stateMachine;
            var states = new List<ChildAnimatorState>();

            void Include(AnimatorStateMachine a) {
                states.AddRange(a.states);

                foreach (var stateMachine in a.stateMachines) {
                    Include(stateMachine.stateMachine);
                }
            }

            Include(root);
            var hashes = states.Select(state => state.state.nameHash).ToArray();
            var names = states.Select(state => state.state.name).ToArray();
            Tree.Draw();
            _gateway.destination = SirenixEditorFields.Dropdown("Destination", _gateway.destination, hashes, names);
        }
    }
    public class ConditionNodeDrawer : OdinValueDrawer<SubStateMachineGateway.ConditionNode> {
        protected override void DrawPropertyLayout(GUIContent label) {
            var value = ValueEntry.SmartValue;
            var indices = new List<int>();
            var names = new List<string>();
            var assetPath = AssetDatabase.GetAssetPath(ValueEntry.Property.Tree.UnitySerializedObject.targetObject);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
            for (int i = 0; i < controller.parameters.Length; i++) {
                indices.Add(i);
                names.Add(controller.parameters[i].name);
            }
            var hasLabel = label != null && !label.text.IsNullOrEmpty();
            if (hasLabel) {
                EditorGUILayout.PrefixLabel(label);
                EditorGUI.indentLevel++;
            }
            using (new EditorGUILayout.VerticalScope()) {
                ValueEntry.Property.Children["mode"].Draw();
                int paramIndex = value.parameter = SirenixEditorFields.Dropdown("Parameter", value.parameter, indices.ToArray(), names.ToArray());
                ValueEntry.SmartValue = value;
                ValueEntry.Property.Children[nameof(SubStateMachineGateway.ConditionNode.children)].Draw();
                var param = controller.parameters[paramIndex];
                string property = string.Empty;

                switch (param.type) {

                    case AnimatorControllerParameterType.Float:
                        property = nameof(SubStateMachineGateway.ConditionNode.floatValue);
                        break;
                    case AnimatorControllerParameterType.Int:
                        property = nameof(SubStateMachineGateway.ConditionNode.intValue);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        property = nameof(SubStateMachineGateway.ConditionNode.boolValue);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (!property.IsEmpty()) {
                    ValueEntry.Property.Children[nameof(SubStateMachineGateway.ConditionNode.type)].Draw();
                    ValueEntry.Property.Children[property].Draw();
                }
            }
            if (hasLabel) {
                EditorGUI.indentLevel--;
            }
        }
    }
#endif
}