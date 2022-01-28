using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Shiroi.FX.Services.BuiltIn {
    public class TimeController : SingletonServiceController<TimeController, TimeMeta> {
        public float DefaultTimeScale = 1;

        private void Start() {
            UpdateGameToDefault();
        }

        protected override void UpdateGameToDefault() {
            Time.timeScale = DefaultTimeScale;
        }

        protected override void UpdateGameTo(IEnumerable<WeightnedMeta<TimeMeta>> activeMetas) {
            Time.timeScale = activeMetas.Sum(weightnedMeta => weightnedMeta.Weight * weightnedMeta.Meta.GetTimeScale());
        }

        protected override void UpdateGameTo(TimeMeta meta) {
            Time.timeScale = meta.GetTimeScale();
        }

        public override void RegisterService(Service service) {
            base.RegisterService(service);
            var timed = service as ITimedService;
            if (timed != null && !timed.IgnoreTimeScale) {
                timed.IgnoreTimeScale = true;
            }
        }
    }

    public class AnimatedTimeMeta : TimeMeta, ITimedServiceTickable {
        private float currentPosition;
        public AnimationCurve Curve;

        public AnimatedTimeMeta(AnimationCurve curve) {
            Curve = curve;
            currentPosition = 0;
        }

        public void Tick(ITimedService service) {
            currentPosition = service.PercentageCompleted;
        }

        public override float GetTimeScale() {
            return Curve.Evaluate(currentPosition);
        }
    }

    public class ConstantTimeMeta : TimeMeta {
        public float timeScale;

        public ConstantTimeMeta(float timeScale) {
            this.timeScale = timeScale;
        }

        public override float GetTimeScale() {
            return timeScale;
        }
    }

    public abstract class TimeMeta {
        public abstract float GetTimeScale();
    }
}