using System.Collections;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Starts the game and runs the waves: plays each level in Level Data order, and after a board is
    /// cleared and its last block has slid out, pauses and shows the next one.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private ShapeCatalogAsset shapeCatalog;
        [SerializeField] private LevelDataAsset levelData;
        [SerializeField] private BoardView boardView;
        [SerializeField] private BoardCamera boardCamera;

        [Tooltip("Seconds between the last block leaving and the next board appearing.")]
        [SerializeField, Min(0f)] private float pauseBetweenWaves = 0.5f;

        private ShapeCatalog catalog;
        private LevelFlow flow;
        private Coroutine nextWaveRoutine;

        /// <summary>The current play session, or null between boards or if a board could not be built.</summary>
        public PuzzleSession Session { get; private set; }

        /// <summary>Debug: throws away the current play and starts the current level again.</summary>
        public void ResetBoard()
        {
            if (flow == null)
            {
                return;
            }

            CancelNextWave();
            StopSession();
            StartCurrentLevel();
        }

        private void Start()
        {
            if (shapeCatalog == null || levelData == null || boardView == null || boardCamera == null)
            {
                Debug.LogError("GameBootstrap: a field is empty in the Inspector.", this);
                return;
            }

            if (!shapeCatalog.TryBuild(out catalog, out var errors))
            {
                Debug.LogError("Shape catalog problems:\n" + string.Join("\n", errors), shapeCatalog);
                return;
            }

            flow = new LevelFlow(levelData.LevelCount);
            StartCurrentLevel();
        }

        private void OnDestroy()
        {
            StopSession();
        }

        private void StartCurrentLevel()
        {
            if (flow.IsFinished)
            {
                // Placeholder until task 6 adds the on-screen message.
                Debug.Log("No more levels.", levelData);
                return;
            }

            var levelNumber = flow.CurrentIndex + 1;
            var level = levelData.GetLevel(flow.CurrentIndex);
            if (level == null)
            {
                Debug.LogError($"Level Data row {levelNumber} is empty.", levelData);
                return;
            }

            if (!level.TryBuild(catalog, out var board, out var errors))
            {
                Debug.LogError($"Level {levelNumber} '{level.DisplayName}' has problems:\n" + string.Join("\n", errors), level);
                return;
            }

            Session = new PuzzleSession(board);
            boardCamera.SetBoardSize(board.Width, board.Height);
            boardView.ShowBoard(board);
            Session.BlockMoved += boardView.HandleBlockMoved;
            Session.BlockExited += boardView.HandleBlockExited;
            Session.BoardCleared += HandleBoardCleared;
        }

        private void StopSession()
        {
            if (Session != null)
            {
                Session.BlockMoved -= boardView.HandleBlockMoved;
                Session.BlockExited -= boardView.HandleBlockExited;
                Session.BoardCleared -= HandleBoardCleared;
                Session = null;
            }
        }

        private void HandleBoardCleared()
        {
            CancelNextWave();
            nextWaveRoutine = StartCoroutine(PlayNextWave());
        }

        private IEnumerator PlayNextWave()
        {
            while (boardView.IsExitPlaying)
            {
                yield return null;
            }

            if (pauseBetweenWaves > 0f)
            {
                yield return new WaitForSeconds(pauseBetweenWaves);
            }

            nextWaveRoutine = null;
            StopSession();
            flow.Advance();
            StartCurrentLevel();
        }

        private void CancelNextWave()
        {
            if (nextWaveRoutine != null)
            {
                StopCoroutine(nextWaveRoutine);
                nextWaveRoutine = null;
            }
        }
    }
}
