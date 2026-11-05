using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>Draws the board base, cells, doors and blocks from game data. Holds no game state.</summary>
    public sealed class BoardView : MonoBehaviour
    {
        // Cells sit on the base: the base's top is at -0.1 and each cell is 0.1 thick.
        private const float CellWorldHeight = -0.05f;

        private const float DoorThickness = 0.2f;
        private const float DoorOutwardOffset = 0.15f;
        private const float DoorHeight = 0.4f;
        private const float DoorWorldHeight = 0.1f;

        private const float BlockHeight = 0.6f;
        private const float BlockGap = 0.05f;

        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform boardBase;

        [Tooltip("A plain 1x1x1 box with no collider, used to build doors and blocks.")]
        [SerializeField] private GameObject partPrefab;

        [SerializeField] private ColorPaletteAsset colorPalette;

        [Tooltip("How far the base extends past the grid on every side, in cells.")]
        [SerializeField, Min(0f)] private float baseMargin = 0.3f;

        public void ShowBoard(Board board)
        {
            ClearBoard();

            var baseScale = boardBase.localScale;
            baseScale.x = board.Width + (baseMargin * 2f);
            baseScale.z = board.Height + (baseMargin * 2f);
            boardBase.localScale = baseScale;

            var grid = new BoardGrid(board.Width, board.Height);
            ShowCells(board, grid);
            ShowDoors(board, grid);
            ShowBlocks(board, grid);
        }

        private void ClearBoard()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void ShowCells(Board board, BoardGrid grid)
        {
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

        private void ShowDoors(Board board, BoardGrid grid)
        {
            foreach (var door in board.Doors)
            {
                var part = CreatePart(transform, door.Color, $"Door_{door.Side}_{door.Color}");
                part.position = grid.GetDoorCenter(door, DoorOutwardOffset, DoorWorldHeight);
                part.localScale = BoardGrid.GetDoorSize(door, DoorThickness, DoorHeight);
            }
        }

        private void ShowBlocks(Board board, BoardGrid grid)
        {
            foreach (var block in board.Blocks)
            {
                var root = new GameObject($"Block_{block.Id}_{block.Shape.Id}_{block.Color}");
                root.transform.SetParent(transform, false);
                root.transform.position = grid.CellToWorld(block.Position);
                root.AddComponent<BlockView>().Initialize(block);

                for (var i = 0; i < block.Shape.Cells.Count; i++)
                {
                    BlockPartLayout.GetPart(block.Shape, i, BlockGap, out var center, out var size);

                    var part = CreatePart(root.transform, block.Color, $"Part_{i}");
                    part.localPosition = new Vector3(center.x, BlockHeight * 0.5f, center.y);
                    part.localScale = new Vector3(size.x, BlockHeight, size.y);
                }
            }
        }

        private Transform CreatePart(Transform parent, string colorId, string partName)
        {
            var part = Instantiate(partPrefab, parent);
            part.name = partName;

            if (colorPalette.TryGetMaterial(colorId, out var material))
            {
                part.GetComponent<MeshRenderer>().sharedMaterial = material;
            }
            else
            {
                Debug.LogError($"Color Palette has no material for color '{colorId}'.", colorPalette);
            }

            return part.transform;
        }
    }
}
