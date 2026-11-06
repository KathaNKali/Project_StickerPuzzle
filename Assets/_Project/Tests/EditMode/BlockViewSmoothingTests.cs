using NUnit.Framework;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class BlockViewSmoothingTests
    {
        private const float FrameSeconds = 0.016f;

        private static readonly Vector3 Start = Vector3.zero;
        private static readonly Vector3 Target = new Vector3(10f, 0f, 0f);

        private BlockView view;

        [SetUp]
        public void SetUp()
        {
            view = new GameObject("TestBlock").AddComponent<BlockView>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(view.gameObject);
        }

        [Test]
        public void TravelStep_MovesExactlySpeedTimesTime()
        {
            var next = BlockView.TravelStep(Start, Target, 5f, 0.1f);

            Assert.That(next.x, Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void TravelStep_NearTheTarget_NeverOvershoots()
        {
            Assert.That(BlockView.TravelStep(Start, Target, 1000f, 1f), Is.EqualTo(Target));
        }

        [Test]
        public void TravelStep_NoSpeedLimit_ArrivesInstantly()
        {
            Assert.That(BlockView.TravelStep(Start, Target, 0f, FrameSeconds), Is.EqualTo(Target));
        }

        [Test]
        public void TravelStep_HigherSpeed_GoesFurtherEachFrame()
        {
            var slow = BlockView.TravelStep(Start, Target, 5f, FrameSeconds);
            var fast = BlockView.TravelStep(Start, Target, 20f, FrameSeconds);

            Assert.That(fast.x, Is.GreaterThan(slow.x));
        }

        // This is the reason for having two sliders: smoothness must not change the travel speed.
        [TestCase(0f)]
        [TestCase(0.05f)]
        [TestCase(0.2f)]
        [TestCase(0.4f)]
        public void TravelSpeed_IsTheSameAtAnySmoothness(float smoothness)
        {
            const float speed = 10f;
            var farAway = new Vector3(1000f, 0f, 0f);
            var guide = Vector3.zero;
            var block = Vector3.zero;

            void RunFrames(int frames)
            {
                for (var i = 0; i < frames; i++)
                {
                    guide = BlockView.TravelStep(guide, farAway, speed, FrameSeconds);
                    block = BlockView.SmoothStep(block, guide, smoothness, FrameSeconds);
                }
            }

            RunFrames(300);
            var before = block.x;
            RunFrames(100);

            Assert.That(block.x - before, Is.EqualTo(speed * FrameSeconds * 100f).Within(0.05f));
        }

        [Test]
        public void SmoothStep_ZeroSmoothness_ArrivesInstantly()
        {
            Assert.That(BlockView.SmoothStep(Start, Target, 0f, FrameSeconds), Is.EqualTo(Target));
        }

        [Test]
        public void SmoothStep_WithSmoothness_MovesPartWay()
        {
            var next = BlockView.SmoothStep(Start, Target, 0.1f, FrameSeconds);

            Assert.That(next.x, Is.GreaterThan(0f));
            Assert.That(next.x, Is.LessThan(10f));
        }

        [Test]
        public void SmoothStep_SlowAndFastFrames_EndUpInTheSamePlace()
        {
            var oneLongFrame = BlockView.SmoothStep(Start, Target, 0.1f, 0.1f);

            var twoShortFrames = BlockView.SmoothStep(Start, Target, 0.1f, 0.05f);
            twoShortFrames = BlockView.SmoothStep(twoShortFrames, Target, 0.1f, 0.05f);

            Assert.That(twoShortFrames.x, Is.EqualTo(oneLongFrame.x).Within(0.001f));
        }

        [Test]
        public void SmoothStep_VeryLongFrame_NeverOvershoots()
        {
            var next = BlockView.SmoothStep(Start, Target, 0.01f, 10f);

            Assert.That(next.x, Is.LessThanOrEqualTo(10f));
        }

        [Test]
        public void MoveTo_WithMotion_WaitsForFramesInsteadOfJumping()
        {
            view.SetMotion(12f, 0.1f);

            view.MoveTo(Target);

            Assert.That(view.transform.position, Is.EqualTo(Start));
            Assert.That(view.enabled, Is.True);
        }

        [Test]
        public void SetMotion_BothZero_FinishesAnyMoveAtOnce()
        {
            view.SetMotion(12f, 0.1f);
            view.MoveTo(Target);

            view.SetMotion(0f, 0f);

            Assert.That(view.transform.position, Is.EqualTo(Target));
            Assert.That(view.enabled, Is.False);
        }

        [Test]
        public void PlaceAt_NoAnimationAndNoPerFrameCost()
        {
            view.SetMotion(12f, 0.1f);

            view.PlaceAt(Target);

            Assert.That(view.transform.position, Is.EqualTo(Target));
            Assert.That(view.enabled, Is.False);
        }
    }
}
