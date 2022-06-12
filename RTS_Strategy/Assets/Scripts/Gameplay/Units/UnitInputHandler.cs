using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Units
{
    public class UnitInputHandler : MonoBehaviour
    {
        [SerializeField]
        private UnitsSelectionController selectionController;

        [ClientCallback]
        private void Awake()
        {
            selectionController = GetComponent<UnitsSelectionController>();
        }

        [ClientCallback]
        void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return;

            if (!mouse.leftButton.wasPressedThisFrame) return;
            Ray cursorRay = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(cursorRay, out var hit))
            {
                var isTarget = hit.collider.TryGetComponent<Targetable>(out var targetable);
                // no target or our unit
                if (!isTarget || targetable.hasAuthority)
                {
                    MoveSelectedUnits(hit.point);
                    return;
                }
                
                // select target
                TargetSelectedUnits(targetable);
            }
        }

        private void MoveSelectedUnits(Vector3 destination)
        {
            foreach (var unit in selectionController.SelectedUnits)
            {
                unit.Targetor.ResetTarget();

                var unitMovement = unit.GetComponent<UnitMovement>();
                if (unitMovement == null)
                {
                    Debug.LogWarning($"{unit} has no UnitMovement component attached.");
                    continue;
                }
                
                unitMovement.CmdUpdatePosition(destination);
            }
        }

        private void TargetSelectedUnits(Targetable target)
        {
            foreach (var unit in selectionController.SelectedUnits)
            {
                // TODO replace with Targetable only
                unit.Targetor.CmdSetTarget(target.gameObject);
            }            
        }
    }
}