using System.Collections.Generic;
using Lunari.Tsuki.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
namespace GGJ22.Traits.Juice {
    public delegate T Getter<T>();

    public abstract class Bind<T> {
        protected readonly int id;

        protected Getter<T> getter;

        protected Bind(string name, Getter<T> getter) {
            Name = name;
            id = Animator.StringToHash(name);
            this.getter = getter;
        }

        [ShowInInspector] public string Name { get; }

        public abstract void Update(Animator animator);
    }

    public class FloatBind : Bind<float> {
        public FloatBind(string id, Getter<float> getter) : base(id, getter) { }

        public override void Update(Animator animator) {
            animator.SetFloat(id, getter());
        }
    }

    public class BoolBind : Bind<bool> {
        public BoolBind(string id, Getter<bool> getter) : base(id, getter) { }

        public override void Update(Animator animator) {
            animator.SetBool(id, getter());
        }
    }

    public class TriggerBind : Bind<bool> {
        public TriggerBind(string id, Getter<bool> getter) : base(id, getter) { }

        public override void Update(Animator animator) {
            if (getter()) {
                animator.SetTrigger(id);
            }
        }
    }

    [TraitLocation(TraitLocations.View)]
    public class AnimatorBinder : Trait {
        public bool includeEntityBinds;

        private Animator animator;
        private bool awareLast;

        [ShowInInspector]
        private List<BoolBind> boolBinds;

        [ShowInInspector]
        private List<FloatBind> floatBinds;

        [ShowInInspector]
        private List<TriggerBind> triggerBinds;

        private void Start() {
            if (includeEntityBinds) {
                BindBool("Aware", () => Owner.Aware);
                BindTrigger("Spawn", () => !awareLast && Owner.Aware);
            }
        }

        private void Update() {
            if (animator == null) {
                return;
            }

            UpdateBinds<FloatBind, float>(floatBinds);
            UpdateBinds<BoolBind, bool>(boolBinds);
            UpdateBinds<TriggerBind, bool>(triggerBinds);
            awareLast = Owner.Aware;
        }

        public void BindFloat(string parameter, Getter<float> getter) {
            floatBinds ??= new List<FloatBind>();
            floatBinds.Add(new FloatBind(parameter, getter));
        }

        public void BindBool(string parameter, Getter<bool> getter) {
            boolBinds ??= new List<BoolBind>();
            boolBinds.Add(new BoolBind(parameter, getter));
        }

        public void BindTrigger(string parameter, Getter<bool> getter) {
            triggerBinds ??= new List<TriggerBind>();
            triggerBinds.Add(new TriggerBind(parameter, getter));
        }

        // private void Awake() {
        //     floatBinds = new List<FloatBind>();
        //     boolBinds = new List<BoolBind>();
        //     triggerBinds = new List<TriggerBind>();
        // }
        public override void Configure(TraitDescriptor descriptor) {
            descriptor.RequiresComponent(out animator);
            if (includeEntityBinds) {
                descriptor.RequiresAnimatorParameter("Aware", AnimatorControllerParameterType.Bool);
                descriptor.RequiresAnimatorParameter("Spawn", AnimatorControllerParameterType.Trigger);
            }
        }

        private void UpdateBinds<B, T>(IEnumerable<B> list) where B : Bind<T> {
            if (list == null) {
                return;
            }

            foreach (var bind in list) {
                bind.Update(animator);
            }
        }
    }
}