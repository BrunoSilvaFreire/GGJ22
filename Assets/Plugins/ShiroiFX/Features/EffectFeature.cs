using UnityEngine;

namespace Shiroi.FX.Features {
    public abstract class EffectFeature {
        protected EffectFeature(params PropertyName[] tags) {
            Tags = tags;
        }

        public PropertyName[] Tags { get; }
    }
}