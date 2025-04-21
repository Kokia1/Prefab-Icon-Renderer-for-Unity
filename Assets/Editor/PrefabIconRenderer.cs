using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PrefabIconRenderer : EditorWindow
{
    private GameObject prefab;
    private int resolution = 512;
    private bool transparentBackground = true;
    private Color backgroundColor = Color.white;

    private Vector3 objectRotation = new Vector3(25f, -30f, 0f);
    private float cameraZoom = 2f;

    private float prefabZoom = 1f;
    private Vector2 prefabOffset = Vector2.zero;

    private Sprite backgroundSprite;
    private float backgroundZoom = 1f;
    private Vector2 backgroundOffset = Vector2.zero;
    private bool tintBackground = false;
    private Color backgroundTintColor = Color.white;

    private Sprite frameSprite;
    private float frameZoom = 1f;
    private Vector2 frameOffset = Vector2.zero;
    private bool tintFrame = false;
    private Color frameTintColor = Color.white;

    private string fileName = "NewIcon";
    private bool showAdvanced = false;

    private Vector2 scrollPosition;

    private RenderTexture previewRT;
    private Texture2D previewTexture;

    private const string folderPath = "Assets/GeneratedSprites";
    private const int tempLayer = 31;

    [MenuItem("Tools/Render Prefab Icon")]
    public static void ShowWindow()
    {
        GetWindow<PrefabIconRenderer>("Render Prefab Icon");
    }

    private void OnEnable() => EditorApplication.update += Repaint;
    private void OnDisable()
    {
        CleanupPreviewObjects();
        EditorApplication.update -= Repaint;
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Prefab Icon Renderer", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        resolution = EditorGUILayout.IntSlider("Resolution", resolution, 128, 1024);
        fileName = EditorGUILayout.TextField("Icon Filename", fileName);

        transparentBackground = EditorGUILayout.Toggle("Transparent Background", transparentBackground);
        GUI.enabled = !transparentBackground;
        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        GUI.enabled = true;

        objectRotation = EditorGUILayout.Vector3Field("Rotation (Euler)", objectRotation);
        cameraZoom = EditorGUILayout.Slider("Camera Zoom (Orthographic Size)", cameraZoom, 0.1f, 10f);

        prefabZoom = EditorGUILayout.Slider("Prefab Zoom", prefabZoom, 0.1f, 20f);
        prefabOffset = EditorGUILayout.Vector2Field("Prefab Offset (X/Y)", prefabOffset);

        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Layer Settings");
        if (showAdvanced)
        {
            GUILayout.Label("Background", EditorStyles.boldLabel);
            backgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite", backgroundSprite, typeof(Sprite), false);
            backgroundZoom = EditorGUILayout.Slider("Background Zoom", backgroundZoom, 0.1f, 100f);
            backgroundOffset = EditorGUILayout.Vector2Field("Background Offset (X/Y)", backgroundOffset);
            tintBackground = EditorGUILayout.Toggle("Tint Background", tintBackground);
            if (tintBackground)
                backgroundTintColor = EditorGUILayout.ColorField("Background Tint Color", backgroundTintColor);

            GUILayout.Space(5);
            GUILayout.Label("Frame", EditorStyles.boldLabel);
            frameSprite = (Sprite)EditorGUILayout.ObjectField("Frame Sprite", frameSprite, typeof(Sprite), false);
            frameZoom = EditorGUILayout.Slider("Frame Zoom", frameZoom, 0.1f, 20f);
            frameOffset = EditorGUILayout.Vector2Field("Frame Offset (X/Y)", frameOffset);
            tintFrame = EditorGUILayout.Toggle("Tint Frame", tintFrame);
            if (tintFrame)
                frameTintColor = EditorGUILayout.ColorField("Frame Tint Color", frameTintColor);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Render & Save Icon"))
        {
            if (prefab != null)
                RenderAndSaveIcon();
            else
                Debug.LogWarning("[PrefabIconRenderer] Kein Prefab ausgewählt.");
        }

        if (prefab != null)
        {
            RenderPreviewToTexture();
            if (previewTexture != null)
            {
                Rect previewRect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(false));
                GUI.Box(previewRect, GUIContent.none);
                GUI.DrawTexture(previewRect, previewTexture, ScaleMode.ScaleToFit, true);
            }
            else
            {
                EditorGUILayout.HelpBox("Keine Vorschau generiert – überprüfe Prefab oder Renderer.", MessageType.Info);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Bitte wähle ein Prefab aus.", MessageType.Warning);
        }

        EditorGUILayout.EndScrollView();
    }

    private GameObject CreateRenderGroup(Camera cam, out Dictionary<GameObject, int> originalLayers, out GameObject prefabObject)
    {
        GameObject group = new GameObject("RenderGroup") { hideFlags = HideFlags.HideAndDontSave };
        originalLayers = new Dictionary<GameObject, int>();

        Vector3 center = cam.transform.position + cam.transform.forward * 5f;

        if (backgroundSprite != null)
        {
            GameObject bg = new GameObject("Background") { hideFlags = HideFlags.HideAndDontSave };
            bg.transform.SetParent(group.transform);
            var sr = bg.AddComponent<SpriteRenderer>();
            sr.sprite = backgroundSprite;
            sr.sortingOrder = -100;
            sr.color = tintBackground ? backgroundTintColor : Color.white;
            bg.transform.position = center + cam.transform.right * backgroundOffset.x + cam.transform.up * backgroundOffset.y;
            bg.transform.localScale = Vector3.one * backgroundZoom;
            bg.transform.rotation = Quaternion.identity;
        }

        prefabObject = Instantiate(prefab);
        prefabObject.hideFlags = HideFlags.HideAndDontSave;
        prefabObject.transform.SetParent(group.transform);
        prefabObject.transform.localPosition = Vector3.zero;
        prefabObject.transform.localRotation = Quaternion.Euler(objectRotation);
        prefabObject.transform.localScale = Vector3.one * prefabZoom;

        Bounds bounds = GetRenderableBounds(prefabObject);
        Vector3 visualCenterOffset = prefabObject.transform.position - bounds.center;
        prefabObject.transform.position += visualCenterOffset;

        prefabObject.transform.position += cam.transform.right * prefabOffset.x + cam.transform.up * prefabOffset.y;

        GameObject sortingHelper = new GameObject("SortingHelper") { hideFlags = HideFlags.HideAndDontSave };
        sortingHelper.transform.SetParent(group.transform);
        sortingHelper.transform.position = prefabObject.transform.position;
        var helperSR = sortingHelper.AddComponent<SpriteRenderer>();
        helperSR.enabled = false;
        helperSR.sortingOrder = 0;

        if (frameSprite != null)
        {
            GameObject frame = new GameObject("Frame") { hideFlags = HideFlags.HideAndDontSave };
            frame.transform.SetParent(group.transform);
            var sr = frame.AddComponent<SpriteRenderer>();
            sr.sprite = frameSprite;
            sr.sortingOrder = 100;
            sr.color = tintFrame ? frameTintColor : Color.white;
            frame.transform.position = center + cam.transform.right * frameOffset.x + cam.transform.up * frameOffset.y;
            frame.transform.localScale = Vector3.one * frameZoom;
            frame.transform.rotation = Quaternion.identity;
        }

        foreach (Transform child in group.GetComponentsInChildren<Transform>(true))
        {
            GameObject go = child.gameObject;
            originalLayers[go] = go.layer;
            go.layer = tempLayer;
        }

        return group;
    }

    private void RenderPreviewToTexture()
    {
        CleanupPreviewObjects();
        GameObject camObj = new GameObject("PreviewCam") { hideFlags = HideFlags.HideAndDontSave };
        Camera cam = camObj.AddComponent<Camera>();

        GameObject renderGroup = CreateRenderGroup(cam, out Dictionary<GameObject, int> originalLayers, out GameObject prefabObject);

        previewRT = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        previewRT.Create();

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        cam.targetTexture = previewRT;
        cam.orthographic = true;
        cam.orthographicSize = cameraZoom;
        cam.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.Default;
        cam.cullingMask = 1 << tempLayer;

        cam.transform.position = Vector3.forward * -10f;
        cam.transform.rotation = Quaternion.identity;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = previewRT;
        cam.Render();

        previewTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        previewTexture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        previewTexture.Apply();

        RenderTexture.active = currentRT;
        DestroyImmediate(camObj);
        RestoreLayers(originalLayers);
        DestroyImmediate(renderGroup);
    }

    private void RenderAndSaveIcon()
    {
        GameObject camObj = new GameObject("IconCam") { hideFlags = HideFlags.HideAndDontSave };
        Camera cam = camObj.AddComponent<Camera>();

        GameObject renderGroup = CreateRenderGroup(cam, out Dictionary<GameObject, int> originalLayers, out GameObject prefabObject);

        RenderTexture rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 8;
        rt.Create();

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        cam.targetTexture = rt;
        cam.orthographic = true;
        cam.orthographicSize = cameraZoom;
        cam.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.Default;
        cam.cullingMask = 1 << tempLayer;

        cam.transform.position = Vector3.forward * -10f;
        cam.transform.rotation = Quaternion.identity;

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;
        cam.Render();

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string safeFileName = string.IsNullOrWhiteSpace(fileName) ? prefab.name + "_Icon" : fileName;
        string filePath = $"{folderPath}/{safeFileName}.png";
        File.WriteAllBytes(filePath, tex.EncodeToPNG());

        AssetDatabase.Refresh();

        TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 100;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        RenderTexture.active = currentRT;
        cam.targetTexture = null;
        rt.Release();

        DestroyImmediate(tex);
        DestroyImmediate(camObj);
        RestoreLayers(originalLayers);
        DestroyImmediate(renderGroup);
    }

    private void RestoreLayers(Dictionary<GameObject, int> layerData)
    {
        foreach (var pair in layerData)
        {
            if (pair.Key != null)
                pair.Key.layer = pair.Value;
        }
    }

    private Bounds GetRenderableBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one * 0.5f);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }

    private void CleanupPreviewObjects()
    {
        if (previewRT != null)
        {
            previewRT.Release();
            previewRT = null;
        }

        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }
    }
}
