using System.Collections.Generic;
using System.Linq;
using Lunari.Tsuki;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shiroi.FX.Utilities {
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component {
        public ushort Prewarm = 20;
        public ushort AllowedTemporaryObjects = 5;

        [AssetsOnly]
        public T Prefab;

        [SerializeField]
        [HideInInspector]
        private List<T> idle = new List<T>();

        [SerializeField]
        [HideInInspector]
        private List<T> allocatedTemporarily = new List<T>();

        [SerializeField]
        [HideInInspector]
        private List<T> inUse = new List<T>();

        private void Start() {
            if (Prefab == null) {
                Debug.LogWarning("Prefab null, unable to instantiate pool.", this);
                return;
            }

            for (ushort i = 0; i < Prewarm; i++) {
                idle.Add(CreateNew(false, true));
            }
        }

        private T CreateNew(bool active, bool attachToTransform) {
            var obj = Instantiate(Prefab);
            obj.gameObject.SetActive(active);
            if (attachToTransform) {
                obj.transform.SetParent(transform);
            }

            return obj;
        }

        public void Return(T obj) {
            if (allocatedTemporarily.Contains(obj)) {
                allocatedTemporarily.Remove(obj);
                Destroy(obj);
                return;
            }

            if (!inUse.Contains(obj)) {
                return;
            }

            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            inUse.Remove(obj);
            idle.Add(obj);
        }

        public T Get() {
            return idle.IsEmpty() ? GetTemporary() : GetIdle();
        }

        private T GetIdle() {
            var obj = idle.First();
            var o = obj.gameObject;
            o.SetActive(true);
            o.transform.SetParent(null);
            idle.Remove(obj);
            inUse.Add(obj);
            return obj;
        }

        private T GetTemporary() {
            if (allocatedTemporarily.Count >= AllowedTemporaryObjects) {
                Debug.LogWarning(string.Format("There are too many temporary objects created by this pool! (Max: {0})",
                    AllowedTemporaryObjects));
                return null;
            }

            var temp = CreateNew(true, false);
            allocatedTemporarily.Add(temp);
            return temp;
        }
    }

    public static class ObjectPools {
        public static void ReturnOrDestroy<T>(this ObjectPool<T> pool, T damageIndicator) where T : Component {
            if (pool != null) {
                pool.Return(damageIndicator);
            } else {
                Object.Destroy(damageIndicator);
            }
        }
    }
}