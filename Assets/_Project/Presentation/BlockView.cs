using System;
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

        private bool isExiting;
        private bool isOnFinalExitLeg;
        private Vector3 exitEndPosition;
        private float exitSpeed;
        private Action<BlockView> exitFinished;

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

        /// <summary>Sets up the block for a model. Also clears any old exit, so a reused block starts fresh.</summary>
        public void Initialize(Block model)
        {
            Model = model;
            isExiting = false;
            isOnFinalExitLeg = false;
            exitFinished = null;
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
            if (isExiting)
            {
                return;
            }

            targetPosition.y = held ? HeldLift : 0f;
            StartMoving();
        }

        /// <summary>
        /// Finishes travelling to the block's last cell, then slides straight out to the end point.
        /// Starting from the last cell keeps the exit straight even if the visible block was
        /// trailing behind the mouse. When done, it calls onFinished so the block can be reused;
        /// with no onFinished, it removes itself.
        /// </summary>
        public void PlayExit(Vector3 lastCellWorld, Vector3 endWorld, float cellsPerSecond, Action<BlockView> onFinished = null)
        {
            exitFinished = onFinished;
            targetPosition = lastCellWorld;
            exitEndPosition = endWorld;
            exitSpeed = cellsPerSecond;
            isExiting = true;
            isOnFinalExitLeg = false;
            enabled = true;
        }

        private void StartMoving()
        {
            if (isExiting || moveSpeed > 0f || smoothTime > 0f)
            {
                enabled = true;
            }
            else
            {
                PlaceAt(targetPosition);
            }
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            var speed = isOnFinalExitLeg ? exitSpeed : moveSpeed;
            guidePosition = TravelStep(guidePosition, targetPosition, speed, deltaTime);

            if (isExiting && !isOnFinalExitLeg && guidePosition == targetPosition)
            {
                isOnFinalExitLeg = true;
                targetPosition = exitEndPosition;
            }

            var next = SmoothStep(transform.position, guidePosition, smoothTime, deltaTime);

            if (guidePosition == targetPosition && (targetPosition - next).sqrMagnitude < ArrivedDistanceSquared)
            {
                FinishMoving();
            }
            else
            {
                transform.position = next;
            }
        }

        private void FinishMoving()
        {
            if (isExiting)
            {
                enabled = false;

                if (exitFinished != null)
                {
                    exitFinished(this);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                PlaceAt(targetPosition);
            }
        }
    }
}
