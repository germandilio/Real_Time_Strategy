using System.Collections.Generic;
using Gameplay.Units;
using Mirror;

namespace Gameplay.Player
{
    /// <summary>
    /// Stores list of all client Units.
    /// </summary>
    public class RTSPlayer : NetworkBehaviour
    {
        public List<Unit> Units { get; } = new List<Unit>();

        #region Server

        public override void OnStartServer()
        {
            Unit.ServerUnitSpawnerStarted += OnServerUnitSpawnerStarted;
            Unit.ServerUnitDespawnerStopped += OnServerDespawnerStopped;
        }
        
        public override void OnStopServer()
        {
            Unit.ServerUnitSpawnerStarted -= OnServerUnitSpawnerStarted;
            Unit.ServerUnitDespawnerStopped -= OnServerDespawnerStopped;
        }

        private void OnServerDespawnerStopped(Unit obj)
        {
            // check that object was spawned for this client
            if (connectionToClient.connectionId != obj.connectionToClient.connectionId) return;
            
            Units.Remove(obj);
        }

        private void OnServerUnitSpawnerStarted(Unit obj)
        {
            // check that object was spawned for this client
            if (connectionToClient.connectionId != obj.connectionToClient.connectionId) return;

            Units.Add(obj);
        }

        #endregion

        #region Client
        
        public override void OnStartClient()
        {
            if (!isClientOnly) return;
            
            Unit.AuthorityUnitSpawnedStarted += OnAuthorityUnitSpawnerStarted;
            Unit.AuthorityUnitSpawnedStopped += OnAuthorityDespawnerStopped;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly) return;
            
            Unit.AuthorityUnitSpawnedStarted -= OnAuthorityUnitSpawnerStarted;
            Unit.AuthorityUnitSpawnedStopped -= OnAuthorityDespawnerStopped;
        }
        
        private void OnAuthorityDespawnerStopped(Unit obj)
        {
            if (!hasAuthority) return;
            
            Units.Remove(obj);
        }

        private void OnAuthorityUnitSpawnerStarted(Unit obj)
        {
            if (!hasAuthority) return;
            
            Units.Add(obj);
        }

        #endregion
    }
}