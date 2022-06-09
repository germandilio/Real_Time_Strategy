using Mirror;
using UnityEngine;

namespace Networking
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject unitSpawnerPrefab;
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            var clientTransform = conn.identity.transform;
            var newUnitSpawner = Instantiate(unitSpawnerPrefab, clientTransform.position, clientTransform.rotation);
            
            NetworkServer.Spawn(newUnitSpawner, conn);
        }
    }
}