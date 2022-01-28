using UnityEngine;

namespace Shiroi.FX.Services {
    public interface ITimedServiceTickable {
        void Tick(ITimedService service);
    }


    public interface ITimedService {
        float TotalDuration { get; }

        float TimeLeft { get; }

        float PercentageCompleted { get; }

        bool IgnoreTimeScale { get; set; }
    }

    /// <summary>
    ///     A service that is executed over a certain timespan.
    /// </summary>
    /// <typeparam name="T">The type of the meta this service carries.</typeparam>
    public class TimedService<T> : Service<T>, ITimedService {
        public TimedService(float duration, T meta, bool ignoreTimeScale = false, ushort priority = DefaultPriority) :
            base(meta, priority) {
            TotalDuration = duration;
            TimeLeft = duration;
            IgnoreTimeScale = ignoreTimeScale;
        }

        public TimedService(ushort priority, T meta) : base(meta, priority) {
        }

        public bool IgnoreTimeScale { get; set; }

        public float TotalDuration { get; }

        public float TimeLeft { get; private set; }

        public float PercentageCompleted => 1 - TimeLeft / TotalDuration;

        public override bool Tick() {
            if (Meta is ITimedServiceTickable timed) {
                timed.Tick(this);
            }

            return (TimeLeft -= GetDeltaTime()) <= 0;
        }

        private float GetDeltaTime() {
            return IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        public override void Cancel() {
            TimeLeft = 0;
        }
    }
}