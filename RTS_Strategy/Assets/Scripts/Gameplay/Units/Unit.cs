using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private UnityEvent selecting;

        [SerializeField]
        private UnityEvent deselecting;

        [SerializeField]
        private Targetor targetor;

        #region Client

        public static event Action<Unit> AuthorityUnitSpawnedStarted;
        public static event Action<Unit> AuthorityUnitSpawnedStopped;

        public Targetor Targetor => targetor;

        public override void OnStartClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            AuthorityUnitSpawnedStarted?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            AuthorityUnitSpawnedStopped?.Invoke(this);
        }
        
        [ClientCallback]
        public void Select()
        {
            if (!hasAuthority) return;
            
            selecting?.Invoke();
        }

        [ClientCallback]
        public void Deselect()
        {
            if (!hasAuthority) return;

            deselecting?.Invoke();
        }

        #endregion

        #region Server

        public static event Action<Unit> ServerUnitSpawnerStarted;
        public static event Action<Unit> ServerUnitDespawnerStopped;

        public override void OnStartServer()
        {
            ServerUnitSpawnerStarted?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerUnitDespawnerStopped?.Invoke(this);
        }

        #endregion
    }
}