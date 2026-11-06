using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// The 3D look of one block. Shows the game data; never changes it. The game moves the block
    /// instantly from cell to cell; this animates the visible block after it, in two separate stages:
    /// an invisible guide travels to the target at a constant speed, and the block softly follows
    /// the guide. So speed sets how fast it travels, and smoothness only softens starts and stops.
    /// It only runs while actually moving, so resting blocks cost nothing per frame.
    /// </summary>
    public sealed class BlockView : MonoBehaviour
    {
        /// <summary>How far a block lifts while held, to show it was picked up.</summary>
        public const float HeldLift = 0.2f;

        private const float ArrivedDistanceSquared = 0.000001f;

        private Vector3 targetPosition;
        private Vector3 guidePosition;
        private float moveSpeed;
        private float smoothTime;

        public Block Model { get; private set; }

        /// <summary>
        /// One frame of the guide's travel: constant speed, in cells per second, without
        /// overshooting. A speed of 0 or less means no speed limit.
        /// </summary>
        public static Vector3 TravelStep(Vector3 current, Vector3 target, float cellsPerSecond, float deltaSeconds)
        {
            return cellsPerSecond <= 0f
                ? target
                : Vector3.MoveTowards(current, target, cellsPerSecond * deltaSeconds);
        }

        /// <summary>
        /// One frame of the block softly following the guide. Depends on elapsed time rather than
        /// frame count, so it feels the same on a slow phone and a fast computer. Never overshoots.
        /// </summary>
        public static Vector3 SmoothStep(Vector3 current, Vector3 target, float smoothSeconds, float deltaSeconds)
        {
            if (smoothSeconds <= 0f)
            {
                return target;
            }

            return Vector3.Lerp(current, target, 1f - Mathf.Exp(-deltaSeconds / smoothSeconds));
        }

        public void Initialize(Block model)
        {
            Model = model;
        }

        /// <summary>Sets how the block animates. Both at 0 means moves happen instantly.</summary>
        public void SetMotion(float cellsPerSecond, float smoothSeconds)
        {
            moveSpeed = cellsPerSecond;
            smoothTime = smoothSeconds;
            StartMoving();
        }

        /// <summary>Puts the block at a spot with no animation.</summary>
        public void PlaceAt(Vector3 world)
        {
            targetPosition = world;
            guidePosition = world;
            transform.position = world;
            enabled = false;
        }

        /// <summary>Sends the block to a spot on the board, keeping the current lift.</summary>
        public void MoveTo(Vector3 world)
        {
            world.y = targetPosition.y;
            targetPosition = world;
            StartMoving();
        }

        public void SetHeld(bool held)
        {
            targetPosition.y = held ? HeldLift : 0f;
            StartMoving();
        }

        private void StartMoving()
        {
            if (moveSpeed <= 0f && smoothTime <= 0f)
            {
                PlaceAt(targetPosition);
            }
            else
            {
                enabled = true;
            }
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            guidePosition = TravelStep(guidePosition, targetPosition, moveSpeed, deltaTime);
            var next = SmoothStep(transform.position, guidePosition, smoothTime, deltaTime);

            if (guidePosition == targetPosition && (targetPosition - next).sqrMagnitude < ArrivedDistanceSquared)
            {
                PlaceAt(targetPosition);
            }
            else
            {
                transform.position = next;
            }
        }
    }
}
