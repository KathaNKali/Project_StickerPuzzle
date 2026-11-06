using StickerPuzzle.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Turns the pointer into game commands. Uses Pointer, which covers mouse now and touch later.
    /// Finds the cell with grid math rather than physics, so blocks need no colliders.
    /// </summary>
    public sealed class BoardInput : MonoBehaviour
    {
        // Halfway up a block, so clicks land on what the player sees in the tilted view.
        private const float PointerPlaneHeight = 0.3f;

        [SerializeField] private GameBootstrap bootstrap;
        [SerializeField] private Camera boardCamera;
        [SerializeField] private BoardView boardView;

        private BlockView heldView;

        private void Update()
        {
            var session = bootstrap.Session;
            var pointer = Pointer.current;
            if (session == null || pointer == null)
            {
                return;
            }

            if (pointer.press.wasPressedThisFrame)
            {
                PickUp(session, pointer.position.ReadValue());
            }

            if (pointer.press.wasReleasedThisFrame)
            {
                LetGo(session);
            }
        }

        private void PickUp(PuzzleSession session, Vector2 screenPosition)
        {
            var ray = boardCamera.ScreenPointToRay(screenPosition);
            if (!BoardGrid.TryGetPointAtHeight(ray, PointerPlaneHeight, out var world))
            {
                return;
            }

            var grid = new BoardGrid(session.Board.Width, session.Board.Height);
            if (session.TryPickUp(grid.WorldToCell(world))
                && boardView.TryGetBlockView(session.HeldBlock.Id, out heldView))
            {
                heldView.SetHeld(true);
            }
        }

        private void LetGo(PuzzleSession session)
        {
            session.LetGo();

            if (heldView != null)
            {
                heldView.SetHeld(false);
                heldView = null;
            }
        }
    }
}
