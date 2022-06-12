using Mirror;
using UnityEngine;

namespace Gameplay.Units
{
    public class Targetable : NetworkBehaviour
    {
        [SerializeField]
        private Transform aimPoint;

        public Transform AimPoint => aimPoint;
    }
}