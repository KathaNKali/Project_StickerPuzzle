using System.Collections;
using NUnit.Framework;
using StickerPuzzle.Presentation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace StickerPuzzle.Tests
{
    public sealed class SmokeTests
    {
        [UnityTest]
        public IEnumerator SpikeScene_LoadsAndRunsFrames()
        {
            yield return SceneManager.LoadSceneAsync("Spike");

            var view = Object.FindAnyObjectByType<SpikeView>();
            Assert.That(view, Is.Not.Null);
            Assert.That(Camera.main, Is.Not.Null);

            for (var i = 0; i < 10; i++)
            {
                yield return null;
            }

            // Any error logged during these frames also fails the test automatically.
            Assert.That(view.FramesRun, Is.GreaterThan(0));
        }
    }
}
