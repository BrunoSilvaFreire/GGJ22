using System.Collections.Generic;
using System.Linq;
using Shiroi.FX.Effects;
using Shiroi.FX.Effects.BuiltIn;
using Shiroi.FX.Utilities;
using UnityEngine;

namespace Shiroi.FX.Services.BuiltIn {
    public class ObjectShakeController : ServiceController<ShakeMeta> {
        public GameObject Object;
        public Vector3 DefaultPosition;

        protected override void UpdateGameToDefault() {
            Object.transform.localPosition = DefaultPosition;
        }

        protected override void UpdateGameTo(IEnumerable<WeightnedMeta<ShakeMeta>> activeMetas) {
            var result = activeMetas.Aggregate(Vector3.zero,
                (current, weightedMeta) => current + weightedMeta.Meta.GetShake() * weightedMeta.Weight
            );

            Object.transform.localPosition = result;
        }

        protected override void UpdateGameTo(ShakeMeta meta) {
            Object.transform.localPosition = meta.GetShake();
        }
    }

    public class ShakeMeta : ITimedServiceTickable {
        private readonly EffectContext context;
        private readonly ObjectShakeEffect.ShakeDimension dimensions;
        private readonly ContinousModularFloat frequency;
        private readonly ContinousModularFloat intensity;

        private readonly ObjectShakeEffect.ShakeMode mode;
        private Vector3 currentShake;
        private float timeUntilNextShake;
        private ObjectShakeEffect.PingPongValues values;

        public ShakeMeta(ContinousModularFloat frequency, ContinousModularFloat intensity, EffectContext context,
            ObjectShakeEffect.ShakeMode mode, ObjectShakeEffect.ShakeDimension dimensions) {
            this.frequency = frequency;
            this.intensity = intensity;
            this.context = context;
            this.mode = mode;
            this.dimensions = dimensions;
            values.Randomize();
        }

        public void Tick(ITimedService service) {
            var timePassed = service.TotalDuration - service.TimeLeft;
            if (timeUntilNextShake <= 0) {
                currentShake =
                    ObjectShakeEffect.GetOffset(context, timePassed, values, mode, intensity, dimensions);
                timeUntilNextShake = 1 / frequency.Evaluate(context, timePassed);
                values.Toggle();
            }

            timeUntilNextShake -= Time.deltaTime;
        }

        public Vector3 GetShake() {
            return currentShake;
        }
    }
}