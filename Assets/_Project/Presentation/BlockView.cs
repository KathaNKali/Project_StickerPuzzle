using StickerPuzzle.Core;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>The 3D look of one block. Shows the game data; never changes it.</summary>
    public sealed class BlockView : MonoBehaviour
    {
        public Block Model { get; private set; }

        public void Initialize(Block model)
        {
            Model = model;
        }
    }
}
