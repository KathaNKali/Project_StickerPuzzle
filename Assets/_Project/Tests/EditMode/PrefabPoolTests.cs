using NUnit.Framework;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class PrefabPoolTests
    {
        private GameObject prefab;
        private Transform storage;
        private Transform parent;
        private PrefabPool pool;

        [SetUp]
        public void SetUp()
        {
            prefab = new GameObject("Prefab");
            prefab.transform.localScale = new Vector3(2f, 3f, 4f);
            storage = new GameObject("Storage").transform;
            parent = new GameObject("Parent").transform;
            pool = new PrefabPool(prefab, storage);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(storage.gameObject);
            Object.DestroyImmediate(parent.gameObject);
        }

        [Test]
        public void Get_PoolEmpty_CreatesCopyUnderParent()
        {
            var item = pool.Get(parent);

            Assert.That(item, Is.Not.SameAs(prefab));
            Assert.That(item.transform.parent, Is.SameAs(parent));
            Assert.That(item.activeSelf, Is.True);
        }

        [Test]
        public void Release_Item_SwitchesOffAndStores()
        {
            var item = pool.Get(parent);

            pool.Release(item);

            Assert.That(item.activeSelf, Is.False);
            Assert.That(item.transform.parent, Is.SameAs(storage));
            Assert.That(pool.FreeCount, Is.EqualTo(1));
        }

        [Test]
        public void Get_AfterRelease_ReusesSameCopy()
        {
            var item = pool.Get(parent);
            pool.Release(item);

            var reused = pool.Get(parent);

            Assert.That(reused, Is.SameAs(item));
            Assert.That(reused.activeSelf, Is.True);
            Assert.That(reused.transform.parent, Is.SameAs(parent));
            Assert.That(pool.FreeCount, Is.EqualTo(0));
        }

        [Test]
        public void Get_ReusedCopyWasMovedAndResized_ComesBackLikeFresh()
        {
            var item = pool.Get(parent);
            item.transform.localPosition = new Vector3(5f, 5f, 5f);
            item.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            item.transform.localScale = Vector3.one * 9f;
            pool.Release(item);

            var reused = pool.Get(parent);

            Assert.That(reused.transform.localPosition, Is.EqualTo(Vector3.zero));
            Assert.That(reused.transform.localRotation, Is.EqualTo(Quaternion.identity));
            Assert.That(reused.transform.localScale, Is.EqualTo(new Vector3(2f, 3f, 4f)));
        }
    }
}
