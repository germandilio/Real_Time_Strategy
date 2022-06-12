using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Units
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject spawnPrefab;

        [SerializeField]
        private Transform spawnPoint;
        
        #region Client

        [ClientCallback]
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
            
            CmdSpawnUnit();
        }
        
        #endregion

        #region Server

        [Command]
        private void CmdSpawnUnit()
        {
            var spawnedUnit = Instantiate(spawnPrefab,
                spawnPoint.position,
                spawnPoint.rotation);
            
            NetworkServer.Spawn(spawnedUnit, connectionToClient);
        } 

        #endregion
    }
}