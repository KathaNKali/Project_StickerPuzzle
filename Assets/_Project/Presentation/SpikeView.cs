using UnityEngine;
using UnityEngine.InputSystem;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Build-spike only: proves 3D rendering, animation, pointer input and on-screen text
    /// work on each platform. Holds no game logic. Remove when the real board view arrives.
    /// </summary>
    public sealed class SpikeView : MonoBehaviour
    {
        [SerializeField] private Transform spinner;
        [SerializeField] private Transform slider;

        private Vector3 sliderStart;
        private float smoothedDelta = 1f / 60f;
        private int taps;

        public int FramesRun { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            sliderStart = slider.position;
        }

        private void Update()
        {
            FramesRun++;
            smoothedDelta = Mathf.Lerp(smoothedDelta, Time.unscaledDeltaTime, 0.05f);

            spinner.Rotate(0f, 90f * Time.deltaTime, 0f);
            slider.position = sliderStart + Vector3.right * Mathf.PingPong(Time.time, 2f);

            var pointer = Pointer.current;
            if (pointer != null && pointer.press.wasPressedThisFrame)
            {
                taps++;
            }
        }

        private void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.Max(18, Screen.height / 32),
            };
            style.normal.textColor = Color.white;

            var text = $"Build spike  |  {1f / smoothedDelta:0} FPS  |  taps: {taps}";
            GUI.Label(new Rect(20, 20, Screen.width - 40, style.fontSize * 2), text, style);
        }
    }
}
