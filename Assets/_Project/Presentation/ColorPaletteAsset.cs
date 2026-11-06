using System;
using System.Collections.Generic;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Matches each game color ID to a material. Blocks and doors share these materials,
    /// which lets Unity draw them efficiently.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "Sticker Puzzle/Color Palette")]
    public sealed class ColorPaletteAsset : ScriptableObject
    {
        [Serializable]
        private sealed class Entry
        {
            public string colorId;
            public Material material;
        }

        [SerializeField] private List<Entry> colors = new List<Entry>();

        public void AddColor(string colorId, Material material)
        {
            colors.Add(new Entry { colorId = colorId, material = material });
        }

        public bool TryGetMaterial(string colorId, out Material material)
        {
            foreach (var entry in colors)
            {
                if (entry.colorId == colorId && entry.material != null)
                {
                    material = entry.material;
                    return true;
                }
            }

            material = null;
            return false;
        }
    }
}
