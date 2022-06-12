using System;
using System.Collections.Generic;
using Gameplay.Player;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Units
{
    public class UnitsSelectionController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;

        [SerializeField]
        private RectTransform selectionPanel;

        public List<Unit> SelectedUnits { get; } = new List<Unit>();

        private Vector2 _startPosition;

        private RTSPlayer _rtsPlayer;

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                Debug.LogWarning("No mouse connected.");
                return;
            }

            if (_rtsPlayer == null)
                _rtsPlayer = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            if (mouse.rightButton.wasPressedThisFrame)
            {
                _startPosition = Mouse.current.position.ReadValue();
                selectionPanel.gameObject.SetActive(true);
                UpdateSelection();
                
                if (!Keyboard.current.shiftKey.isPressed)
                    ClearSelection();
            }
            else if (mouse.rightButton.wasReleasedThisFrame)
            {
                selectionPanel.gameObject.SetActive(false);

                TrySelect(mouse.position.ReadValue());
            }
            else if (mouse.rightButton.isPressed)
            {
                UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            Vector2 cursorPos = Mouse.current.position.ReadValue();

            float width = cursorPos.x - _startPosition.x;
            float height = cursorPos.y - _startPosition.y;
            selectionPanel.sizeDelta = new Vector2(Math.Abs(width), Math.Abs(height));
            
            // update anchor position to half of width and height
            selectionPanel.anchoredPosition = _startPosition + new Vector2(width / 2, height / 2);
        }

        private bool TrySelect(Vector3 cursorPosition)
        {
            if (Mathf.Approximately(selectionPanel.sizeDelta.magnitude, 0))
                return TrySelectSingle(cursorPosition);

            return TrySelectMultiple(cursorPosition);
        }

        private bool TrySelectMultiple(Vector3 cursorPosition)
        {
            var camera = Camera.main;
            if (camera == null)
                return false;
            
            foreach (var unit in _rtsPlayer.Units)
            {
                if (SelectedUnits.Contains(unit)) continue;
                var screenPos = camera.WorldToScreenPoint(unit.transform.position);
                var panelMin = selectionPanel.anchoredPosition - (selectionPanel.sizeDelta / 2);
                var panelMax = selectionPanel.anchoredPosition + (selectionPanel.sizeDelta / 2);

                if (screenPos.x > panelMin.x &&
                    screenPos.x < panelMax.x &&
                    screenPos.y > panelMin.y &&
                    screenPos.y < panelMax.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }

            return true;
        }

        private bool TrySelectSingle(Vector3 cursorPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
            if (!Physics.Raycast(ray, out var hitInfo, Single.MaxValue, layerMask)) return false;
            if (!hitInfo.collider.TryGetComponent<Unit>(out var unit)) return false;
            if (!unit.hasAuthority) return false;

            if (!SelectedUnits.Contains(unit))
                SelectedUnits.Add(unit);

            foreach (var unitController in SelectedUnits)
            {
                unitController.Select();
            }
            return true;
        }

        private void ClearSelection()
        {
            foreach (var unitController in SelectedUnits)
            {
                unitController.Deselect();
            }
            
            SelectedUnits.Clear();
        }
    }
}