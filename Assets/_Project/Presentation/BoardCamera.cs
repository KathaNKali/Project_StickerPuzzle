using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// An orthographic camera that keeps the whole board in view on any screen shape. The board
    /// is centered on the world origin, lying flat: grid X is world X, and grid Y is world Z.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class BoardCamera : MonoBehaviour
    {
        // In orthographic view, distance does not change how big things look.
        // It only needs to keep the board between the camera's clip planes.
        private const float CameraDistance = 30f;

        [Tooltip("90 looks straight down. Lower values tilt the view.")]
        [SerializeField, Range(60f, 90f)] private float pitch = 76f;

        [Tooltip("Empty space kept around the board on every side, in cells.")]
        [SerializeField, Min(0f)] private float padding = 0.6f;

        [Tooltip("Used until the game tells the camera the real board size.")]
        [SerializeField, Min(1)] private int boardWidth = 6;

        [SerializeField, Min(1)] private int boardHeight = 6;

        private Camera targetCamera;
        private int lastScreenWidth;
        private int lastScreenHeight;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            Apply();
        }

        private void Update()
        {
            // Only recalculates when the screen changes, such as on rotation or a window resize.
            if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            {
                Apply();
            }
        }

        private void OnValidate()
        {
            // Lets the designer tune pitch and padding live during Play.
            if (Application.isPlaying && targetCamera != null)
            {
                Apply();
            }
        }

        public void SetBoardSize(int width, int height)
        {
            boardWidth = width;
            boardHeight = height;
            Apply();
        }

        /// <summary>
        /// The orthographic size (half the visible height) that fits a flat width-by-depth area
        /// on screen. The area must fit both ways; tilting makes its depth look shorter.
        /// </summary>
        public static float OrthographicSizeToFit(float width, float depth, float pitchDegrees, float aspect)
        {
            var sizeForWidth = width * 0.5f / aspect;
            var sizeForDepth = depth * 0.5f * Mathf.Sin(pitchDegrees * Mathf.Deg2Rad);
            return Mathf.Max(sizeForWidth, sizeForDepth);
        }

        private void Apply()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            var fitWidth = boardWidth + (padding * 2f);
            var fitDepth = boardHeight + (padding * 2f);

            targetCamera.orthographic = true;
            targetCamera.orthographicSize = OrthographicSizeToFit(fitWidth, fitDepth, pitch, targetCamera.aspect);

            var rotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.SetPositionAndRotation(rotation * Vector3.back * CameraDistance, rotation);
        }
    }
}
