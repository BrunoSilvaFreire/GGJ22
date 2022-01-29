using System;
using System.Linq;
using GGJ22.Game;
using GGJ22.Input;
using GGJ22.Movement;
using GGJ22.Traits.Juice;
using Lunari.Tsuki.Entities;
using Lunari.Tsuki2D.Runtime.Movement;
using Shiroi.FX.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace GGJ22.Traits.Movement.Hook {
    public class Hook : Trait {
        public enum HookSlotType : sbyte {
            Min = -1,
            Mid = 0,
            Max = 1
        }
        [Serializable]
        public struct HookSlot {
            public HookSlotType horizontal, vertical;
        }
        private const string HookHasTargetName = "HookHasTarget";
        public float travelSpeed = 5;
        public float retractionSpeed = 1;
        [Required]
        public LineRenderer hookLine;
        public float hookLength = 5;
        public LineRenderer aimIndicator;
        public Transform hookOrigin;
        public AnimationCurve wobbleOverTime;
        public AnimationCurve wobbleOnReturn;
        [Required]
        public Wobbly wobbly;
        [Required]
        public HookedState toSet;
        public float penetrationDistance;

        #region Traits

        [ShowInInspector, ReadOnly]
        private Aim _aim;
        [ShowInInspector, ReadOnly]
        private BlobInput _input;
        [ShowInInspector, ReadOnly]
        private AnimatorBinder _animatorBinder;
        [ShowInInspector, ReadOnly]
        private Motor _motor;

        #endregion

        #region Internal Properties

        private Vector3[] _positions;
        private bool _hasTarget;
        private float _currentDistance;
        private bool _currentlyShot;
        private TravelDirection _direction;
        private Vector2 _shotDirection;

        #endregion

        private enum TravelDirection {
            Forward,
            Backward
        };
        private void Start() {
            _positions = new Vector3[2];
        }
        public override void Configure(TraitDescriptor descriptor) {
            descriptor.RequiresAnimatorParameter(
                HookHasTargetName,
                AnimatorControllerParameterType.Bool
            );
            if (descriptor.DependsOn(out _aim, out _animatorBinder, out _input, out _motor)) {
                _animatorBinder.BindBool(
                    HookHasTargetName,
                    () => _hasTarget
                );
            }
        }
        public void ForceBeginRetract(Vector2 fromPoint) {
            wobbly.tip = fromPoint;
            _currentlyShot = true;
            _shotDirection = fromPoint - (Vector2) hookOrigin.position;
            _shotDirection.Normalize();
            _currentDistance = Vector2.Distance(wobbly.tip, wobbly.origin);
            _direction = TravelDirection.Backward;
        }
        private void Update() {
            aimIndicator.enabled = !_currentlyShot;
            hookLine.enabled = _currentlyShot;
            if (_currentlyShot) {
                UpdateHook();
            } else {
                UpdateIndicator();
                if (_input.shoot.Consume()) {
                    Shoot();
                }
            }
        }
        private void UpdateHook() {
            var l = hookLength;
            float progress;
            switch (_direction) {
                case TravelDirection.Forward:
                    progress = travelSpeed * Time.deltaTime;
                    _currentDistance += progress;
                    if (_currentDistance >= l) {
                        //Extended Too
                        _currentDistance = l;
                        _direction = TravelDirection.Backward;
                    }
                    break;
                case TravelDirection.Backward:
                    progress = retractionSpeed * Time.deltaTime;
                    _currentDistance -= progress;
                    if (_currentDistance <= 0) {
                        _currentlyShot = false;
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var dir = _shotDirection * _currentDistance;
            var origin = (Vector2) hookOrigin.position;
            var end = origin + dir;
            var results = new RaycastHit2D[1];
            bool hit;
            if (_direction == TravelDirection.Backward) {
                hit = false;
            } else {
                var nHits = Physics2D.LinecastNonAlloc(
                    origin,
                    end,
                    results,
                    GameConfiguration.Instance.worldMask
                );
                hit = nHits > 0;
            }
            if (hit) {
                OnHooked(results.Single());
            } else {
                var wobbleCurve = _direction switch {
                    TravelDirection.Forward => wobbleOverTime,
                    TravelDirection.Backward => wobbleOnReturn,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var wobbleAmount = wobbleCurve.Evaluate(_currentDistance / hookLength);
                wobbly.origin = origin;
                wobbly.tip = end;
                wobbly.wobbleMultiplier = wobbleAmount;
            }
        }
        public HookSlotType FindTypeFor(float offset) {
            const float tileSize = 1;
            const float segmentLength = tileSize / 3;
            return (HookSlotType) (Mathf.FloorToInt(offset / segmentLength) - 1);
        }
        private void OnHooked(RaycastHit2D hit) {
            _currentlyShot = false;
            _currentDistance = 0;
            var normal = hit.normal;
            var point = hit.point;
            var innerPoint = point + (-normal * penetrationDistance);
          
            var tilemap = hit.transform.GetComponent<Tilemap>();
            if (tilemap == null) {
                return;
            }
            var cell = tilemap.WorldToCell(innerPoint);
            var cellSize = tilemap.cellSize;
            var cellCenter = tilemap.GetCellCenterWorld(cell);
            var offset = point - (Vector2) (cellCenter - (cellSize / 2));
            var slot = new HookSlot {
                horizontal = FindTypeFor(offset.x),
                vertical = FindTypeFor(offset.y),
            };
            
            toSet.Attach(
                slot,
                tilemap,
                hit.collider,
                cell
            );

            _motor.ActiveState = toSet;
        }
        private void UpdateIndicator() {
            if (_aim == null) {
                return;
            }
            var dir = _aim.AimDirection.normalized * hookLength;
            var origin = (Vector2) hookOrigin.position;
            var end = origin + dir;
            var results = new RaycastHit2D[1];
            var nHits = Physics2D.LinecastNonAlloc(
                origin,
                end,
                results,
                GameConfiguration.Instance.worldMask
            );
            _hasTarget = nHits > 0;

            _positions[0] = origin;
            if (_hasTarget) {
                var hit = results.Single();
                var hitPos = hit.point;
                _positions[1] = hitPos;
            } else {
                _positions[1] = end;
            }
            aimIndicator.SetPositions(_positions);
        }
        public void Shoot() {
            if (_currentlyShot) {
                return;
            }
            _currentlyShot = true;
            _direction = TravelDirection.Forward;
            _shotDirection = _aim.AimDirection.normalized;
        }
    }
}