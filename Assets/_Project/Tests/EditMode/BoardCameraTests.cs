using NUnit.Framework;
using StickerPuzzle.Presentation;
using UnityEngine;

namespace StickerPuzzle.Tests
{
    public sealed class BoardCameraTests
    {
        private GameObject cameraObject;

        [SetUp]
        public void SetUp()
        {
            cameraObject = new GameObject("TestCamera", typeof(Camera));
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(cameraObject);
        }

        // Tall phones, a tablet shape, non-square boards, a straight-down view and a wide screen.
        [TestCase(6f, 6f, 76f, 0.5625f)]
        [TestCase(6f, 6f, 76f, 0.4286f)]
        [TestCase(6f, 6f, 76f, 0.75f)]
        [TestCase(8f, 4f, 76f, 0.5625f)]
        [TestCase(4f, 8f, 76f, 0.5625f)]
        [TestCase(6f, 6f, 90f, 0.5625f)]
        [TestCase(6f, 6f, 76f, 1.5f)]
        public void OrthographicSizeToFit_AllCornersOnScreenWithNoWastedSpace(float width, float depth, float pitch, float aspect)
        {
            var testCamera = cameraObject.GetComponent<Camera>();
            testCamera.orthographic = true;
            testCamera.orthographicSize = BoardCamera.OrthographicSizeToFit(width, depth, pitch, aspect);
            testCamera.aspect = aspect;
            var rotation = Quaternion.Euler(pitch, 0f, 0f);
            cameraObject.transform.SetPositionAndRotation(rotation * Vector3.back * 30f, rotation);

            var furthestFromCenter = 0f;
            for (var xSign = -1; xSign <= 1; xSign += 2)
            {
                for (var zSign = -1; zSign <= 1; zSign += 2)
                {
                    var corner = new Vector3(xSign * width * 0.5f, 0f, zSign * depth * 0.5f);
                    var onScreen = testCamera.WorldToViewportPoint(corner);

                    Assert.That(onScreen.z, Is.GreaterThan(0f), "Corner is behind the camera");
                    furthestFromCenter = Mathf.Max(furthestFromCenter, Mathf.Abs(onScreen.x - 0.5f), Mathf.Abs(onScreen.y - 0.5f));
                }
            }

            // 0.5 is exactly the screen edge.
            Assert.That(furthestFromCenter, Is.LessThanOrEqualTo(0.501f), "A corner is off screen");
            Assert.That(furthestFromCenter, Is.GreaterThanOrEqualTo(0.49f), "The view is bigger than it needs to be");
        }

        [Test]
        public void OrthographicSizeToFit_BiggerBoard_ShowsMore()
        {
            var small = BoardCamera.OrthographicSizeToFit(4f, 4f, 76f, 0.5625f);
            var big = BoardCamera.OrthographicSizeToFit(8f, 8f, 76f, 0.5625f);

            Assert.That(big, Is.GreaterThan(small));
        }

        [Test]
        public void OrthographicSizeToFit_NarrowerScreen_ShowsMore()
        {
            var phone = BoardCamera.OrthographicSizeToFit(6f, 6f, 76f, 0.5625f);
            var tallPhone = BoardCamera.OrthographicSizeToFit(6f, 6f, 76f, 0.4286f);

            Assert.That(tallPhone, Is.GreaterThan(phone));
        }
    }
}
