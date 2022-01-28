using Lunari.Tsuki.Singletons;
using UnityEngine;
namespace GGJ22.Game {
    [CreateAssetMenu(menuName = "GGJ22/GameConfiguration")]
    public class GameConfiguration : ScriptableSingleton<GameConfiguration> {
        public LayerMask worldMask;
    }
}