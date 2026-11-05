using System.IO;
using StickerPuzzle.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace StickerPuzzle.EditorTools
{
    /// <summary>Generates the graybox build-spike scene. Change this, not the scene file.</summary>
    public static class SpikeSceneBuilder
    {
        public const string ScenePath = "Assets/_Project/Scenes/Spike.unity";
        private const string MaterialFolder = "Assets/_Project/Art/Materials/Spike";
        private const int BoardSize = 6;

        [MenuItem("StickerPuzzle/Rebuild Spike Scene")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            Directory.CreateDirectory(MaterialFolder);
            AssetDatabase.Refresh();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.6f);
            RenderSettings.skybox = null;

            AddCamera();
            AddLight();

            var boardMaterial = GetMaterial("Board", new Color(0.32f, 0.34f, 0.4f));
            var cellMaterial = GetMaterial("Cell", new Color(0.42f, 0.44f, 0.5f));
            var redMaterial = GetMaterial("Red", new Color(0.86f, 0.25f, 0.25f));
            var blueMaterial = GetMaterial("Blue", new Color(0.22f, 0.48f, 0.88f));
            var yellowMaterial = GetMaterial("Yellow", new Color(0.95f, 0.78f, 0.2f));

            AddBox("Board", new Vector3(0f, -0.3f, 0f), new Vector3(BoardSize + 0.6f, 0.4f, BoardSize + 0.6f), boardMaterial);
            for (var x = 0; x < BoardSize; x++)
            {
                for (var z = 0; z < BoardSize; z++)
                {
                    AddBox($"Cell_{x}_{z}", CellCenter(x, z, -0.05f), new Vector3(0.92f, 0.1f, 0.92f), cellMaterial);
                }
            }

            AddBox("Door_Red", new Vector3(BoardSize * 0.5f + 0.3f, 0.1f, 0.5f), new Vector3(0.2f, 0.4f, 2f), redMaterial);
            AddBox("Door_Blue", new Vector3(-0.5f, 0.1f, BoardSize * 0.5f + 0.3f), new Vector3(2f, 0.4f, 0.2f), blueMaterial);

            var spinner = AddBox("Block_Spinner", CellCenter(1, 1, 0.3f), new Vector3(0.9f, 0.6f, 0.9f), yellowMaterial);
            var slider = AddBox("Block_Slider", CellCenter(1, 4, 0.3f), new Vector3(1.9f, 0.6f, 0.9f), blueMaterial);
            AddBox("Block_Static", CellCenter(4, 2, 0.3f) + new Vector3(0f, 0f, 0.5f), new Vector3(0.9f, 0.6f, 1.9f), redMaterial);

            var view = new GameObject("SpikeView").AddComponent<SpikeView>();
            var serialized = new SerializedObject(view);
            serialized.FindProperty("spinner").objectReferenceValue = spinner.transform;
            serialized.FindProperty("slider").objectReferenceValue = slider.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[SCENE] Built {ScenePath}");
        }

        private static Vector3 CellCenter(int x, int z, float y)
        {
            var offset = (BoardSize - 1) * 0.5f;
            return new Vector3(x - offset, y, z - offset);
        }

        private static void AddCamera()
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            var camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.13f, 0.16f);
            camera.fieldOfView = 40f;
            go.transform.position = new Vector3(0f, 12f, -3f);
            go.transform.rotation = Quaternion.Euler(76f, 0f, 0f);
        }

        private static void AddLight()
        {
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            go.transform.rotation = Quaternion.Euler(55f, -35f, 0f);
        }

        private static GameObject AddBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.position = position;
            go.transform.localScale = scale;
            go.GetComponent<MeshRenderer>().sharedMaterial = material;
            return go;
        }

        private static Material GetMaterial(string name, Color color)
        {
            var path = $"{MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.SetColor("_BaseColor", color);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}
