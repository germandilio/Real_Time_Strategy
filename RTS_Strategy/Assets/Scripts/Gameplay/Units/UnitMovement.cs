using System;
using System.IO;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField]
        private float chaseDistance = 10f;
        
        private NavMeshAgent _navMesh;
        private Targetor _targetor;

        [ServerCallback]
        public void Awake()
        {
            _targetor = GetComponent<Targetor>();
            _navMesh = GetComponent<NavMeshAgent>();
        }

        [ServerCallback]
        private void Update()
        {
            if (_targetor.Target != null)
            {
                ChaseBehaviour(_targetor.Target);
                return;
            }

            // preventing units to pushing each other at the end of th path
            if (_navMesh.hasPath && _navMesh.remainingDistance < _navMesh.stoppingDistance)
                _navMesh.ResetPath();
        }

        private void ChaseBehaviour(Targetable target)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > Mathf.Pow(chaseDistance, 2))
                _navMesh.SetDestination(target.transform.position);
            else
                _navMesh.ResetPath();
        }

        #region Server

        [Command]
        public void CmdUpdatePosition(Vector3 destination)
        {
            if (_navMesh == null) return;
            
            if (NavMesh.SamplePosition(destination, out var hit, Single.MaxValue, NavMesh.AllAreas))
            {
                _navMesh.SetDestination(hit.position);
            }
        }
        
        #endregion
    }   
}
 