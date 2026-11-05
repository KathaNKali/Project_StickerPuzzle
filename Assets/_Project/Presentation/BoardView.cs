using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>Draws the board base and its grid cells from game data. Holds no game state.</summary>
    public sealed class BoardView : MonoBehaviour
    {
        // Cells sit on the base: the base's top is at -0.1 and each cell is 0.1 thick.
        private const float CellWorldHeight = -0.05f;

        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform boardBase;

        [Tooltip("How far the base extends past the grid on every side, in cells.")]
        [SerializeField, Min(0f)] private float baseMargin = 0.3f;

        public void ShowBoard(Board board)
        {
            ClearCells();

            var baseScale = boardBase.localScale;
            baseScale.x = board.Width + (baseMargin * 2f);
            baseScale.z = board.Height + (baseMargin * 2f);
            boardBase.localScale = baseScale;

            var grid = new BoardGrid(board.Width, board.Height);
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    var position = grid.CellToWorld(new GridPosition(x, y), CellWorldHeight);
                    var cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                    cell.name = $"Cell_{x}_{y}";
                }
            }
        }

        private void ClearCells()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
