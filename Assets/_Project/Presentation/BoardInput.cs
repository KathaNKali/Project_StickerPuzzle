using StickerPuzzle.Core;
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

        // Which of the block's cells was grabbed, so it does not jump to center on the pointer.
        private GridPosition grabOffset;

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

            if (pointer.press.isPressed && session.HeldBlock != null)
            {
                Drag(session, pointer.position.ReadValue());
            }

            if (pointer.press.wasReleasedThisFrame)
            {
                LetGo(session);
            }
        }

        private bool TryGetPointerCell(PuzzleSession session, Vector2 screenPosition, out GridPosition cell)
        {
            var ray = boardCamera.ScreenPointToRay(screenPosition);
            if (!BoardGrid.TryGetPointAtHeight(ray, PointerPlaneHeight, out var world))
            {
                cell = default;
                return false;
            }

            cell = new BoardGrid(session.Board.Width, session.Board.Height).WorldToCell(world);
            return true;
        }

        private void PickUp(PuzzleSession session, Vector2 screenPosition)
        {
            if (TryGetPointerCell(session, screenPosition, out var cell)
                && session.TryPickUp(cell)
                && boardView.TryGetBlockView(session.HeldBlock.Id, out heldView))
            {
                grabOffset = cell - session.HeldBlock.Position;
                heldView.SetHeld(true);
            }
        }

        private void Drag(PuzzleSession session, Vector2 screenPosition)
        {
            if (TryGetPointerCell(session, screenPosition, out var cell))
            {
                session.DragHeldToward(cell - grabOffset);
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
