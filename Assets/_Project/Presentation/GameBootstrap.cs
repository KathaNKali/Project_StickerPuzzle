using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>Starts the game: turns the data assets into a playable board, then hands it to the views.</summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private ShapeCatalogAsset shapeCatalog;
        [SerializeField] private BoardLayoutAsset boardLayout;
        [SerializeField] private BoardView boardView;
        [SerializeField] private BoardCamera boardCamera;

        /// <summary>The current play session, or null if the board could not be built.</summary>
        public PuzzleSession Session { get; private set; }

        private void Start()
        {
            if (shapeCatalog == null || boardLayout == null || boardView == null || boardCamera == null)
            {
                Debug.LogError("GameBootstrap: a field is empty in the Inspector.", this);
                return;
            }

            if (!shapeCatalog.TryBuild(out var catalog, out var errors))
            {
                Debug.LogError("Shape catalog problems:\n" + string.Join("\n", errors), shapeCatalog);
                return;
            }

            if (!boardLayout.TryBuild(catalog, out var board, out errors))
            {
                Debug.LogError("Board layout problems:\n" + string.Join("\n", errors), boardLayout);
                return;
            }

            Session = new PuzzleSession(board);
            boardCamera.SetBoardSize(board.Width, board.Height);
            boardView.ShowBoard(board);
            Session.BlockMoved += boardView.HandleBlockMoved;
        }

        private void OnDestroy()
        {
            if (Session != null)
            {
                Session.BlockMoved -= boardView.HandleBlockMoved;
            }
        }
    }
}
