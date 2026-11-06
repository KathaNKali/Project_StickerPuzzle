using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>The 3D look of one block. Shows the game data; never changes it.</summary>
    public sealed class BlockView : MonoBehaviour
    {
        /// <summary>How far a block lifts while held, to show it was picked up.</summary>
        public const float HeldLift = 0.2f;

        public Block Model { get; private set; }

        public void Initialize(Block model)
        {
            Model = model;
        }

        /// <summary>Moves to a spot on the board, keeping the current lift.</summary>
        public void MoveTo(Vector3 world)
        {
            world.y = transform.position.y;
            transform.position = world;
        }

        public void SetHeld(bool held)
        {
            var position = transform.position;
            position.y = held ? HeldLift : 0f;
            transform.position = position;
        }
    }
}
