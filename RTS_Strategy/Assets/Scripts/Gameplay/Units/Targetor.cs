using Mirror;
using UnityEngine;

namespace Gameplay.Units
{
    public class Targetor : NetworkBehaviour
    {
        private Targetable _target;
        
        public Targetable Target => _target;

        #region Server

        [Command]
        public void ResetTarget()
        {
            _target = null;
        }
        
        [Command]
        public void CmdSetTarget(GameObject target)
        {
            var targetable = target.GetComponent<Targetable>();
            if (targetable == null) return;

            _target = targetable;
        }

        #endregion
    }
}