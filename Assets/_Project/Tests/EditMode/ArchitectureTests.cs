using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StickerPuzzle.Core;
using StickerPuzzle.Game;
using StickerPuzzle.Presentation;

namespace StickerPuzzle.Tests
{
    public sealed class ArchitectureTests
    {
        [Test]
        public void CoreAssembly_HasExpectedName()
        {
            Assert.That(typeof(CoreInfo).Assembly.GetName().Name, Is.EqualTo(CoreInfo.AssemblyName));
        }

        [Test]
        public void Core_ReferencesNoUnityAssemblies()
        {
            var unityReferences = ReferenceNames(typeof(CoreInfo).Assembly)
                .Where(name => name.StartsWith("Unity"))
                .ToArray();

            Assert.That(unityReferences, Is.Empty);
        }

        [Test]
        public void Core_DoesNotReferenceGameOrPresentation()
        {
            var references = ReferenceNames(typeof(CoreInfo).Assembly);

            Assert.That(references, Has.None.EqualTo(typeof(GameInfo).Assembly.GetName().Name));
            Assert.That(references, Has.None.EqualTo(typeof(SpikeView).Assembly.GetName().Name));
        }

        [Test]
        public void Game_DoesNotReferencePresentation()
        {
            var references = ReferenceNames(typeof(GameInfo).Assembly);

            Assert.That(references, Has.None.EqualTo(typeof(SpikeView).Assembly.GetName().Name));
        }

        private static string[] ReferenceNames(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies().Select(reference => reference.Name).ToArray();
        }
    }
}
