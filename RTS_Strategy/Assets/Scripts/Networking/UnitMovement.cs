using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Networking
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        private NavMeshAgent _navMesh;

        public void Awake()
        {
            _navMesh = GetComponent<NavMeshAgent>();
        }


        [ClientCallback]
        void Update()
        {
            if (!hasAuthority) return;
            Mouse mouse = Mouse.current;
            if (mouse == null) return;
            
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Ray cursorRay = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
                if (Physics.Raycast(cursorRay, out var hit))
                {
                    CmdUpdatePosition(hit.point);
                }
            }
        }

        #region Server

        [Command]
        private void CmdUpdatePosition(Vector3 destination)
        {
            if (transform == null || _navMesh == null) return;

            if (NavMesh.SamplePosition(destination, out var hit, Single.MaxValue, NavMesh.AllAreas))
            {
                _navMesh.SetDestination(hit.position);
            }
        }
        
        #endregion
    }   
}
 