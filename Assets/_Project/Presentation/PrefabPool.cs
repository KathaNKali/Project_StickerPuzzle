using System.Collections.Generic;
using UnityEngine;

namespace StickerPuzzle.Presentation
{
    /// <summary>
    /// Reuses copies of one prefab instead of creating and destroying them. A copy that comes back
    /// out of the pool has the prefab's local position, rotation and scale, like a fresh copy.
    /// </summary>
    public sealed class PrefabPool
    {
        private readonly GameObject prefab;
        private readonly Transform storage;
        private readonly Stack<GameObject> free = new Stack<GameObject>();

        /// <param name="storage">Where unused copies wait, switched off.</param>
        public PrefabPool(GameObject prefab, Transform storage)
        {
            this.prefab = prefab;
            this.storage = storage;
        }

        /// <summary>How many copies are waiting to be reused.</summary>
        public int FreeCount => free.Count;

        public GameObject Get(Transform parent)
        {
            if (free.Count == 0)
            {
                return Object.Instantiate(prefab, parent);
            }

            var item = free.Pop();
            var itemTransform = item.transform;
            var prefabTransform = prefab.transform;
            itemTransform.SetParent(parent, false);
            itemTransform.localPosition = prefabTransform.localPosition;
            itemTransform.localRotation = prefabTransform.localRotation;
            itemTransform.localScale = prefabTransform.localScale;
            item.SetActive(true);
            return item;
        }

        public void Release(GameObject item)
        {
            item.SetActive(false);
            item.transform.SetParent(storage, false);
            free.Push(item);
        }
    }
}
