// Assets/Editor/TexturePatternGenerator.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

public class TexturePatternGenerator : EditorWindow
{
    private Texture2D sourceTexture;
    private int horizontalRepeats = 2;
    private int verticalRepeats = 2;
    private Texture2D previewTexture;
    private Vector2 scrollPosition = Vector2.zero;
    private string outputFileName = "GeneratedPattern";

    // Background settings
    private enum BackgroundType { None, SolidColor, Texture }
    private BackgroundType backgroundType = BackgroundType.None;
    private Color backgroundColor = Color.white;
    private Texture2D backgroundTexture;
    private bool useTransparency = false;
    private float backgroundAlpha = 1.0f;

    // Randomization settings
    private bool usePositionRandomization = false;
    private float positionRandomnessX = 0f;
    private float positionRandomnessY = 0f;

    private bool useScaleRandomization = false;
    private float minScale = 0.8f;
    private float maxScale = 1.2f;

    private bool useRotationRandomization = false;
    private float rotationRandomness = 0f;

    private int randomSeed = 42;

    // NEW FEATURES - Pattern Offset Controls
    private int patternOffsetX = 0;
    private int patternOffsetY = 0;

    // NEW FEATURES - Seamless Tiling Preview
    private bool seamlessTilingPreview = false;
    private int tilingPreviewRepeats = 3;

    // NEW FEATURES - Pattern Symmetry/Mirroring
    private bool flipAlternateHorizontal = false;
    private bool flipAlternateVertical = false;

    // NEW FEATURES - Color Tint/Hue Shift
    private Color globalTint = Color.white;
    private bool useRandomHueShift = false;
    private float hueShiftRange = 0f;

    // NEW FEATURES - Alpha Mask Support
    private Texture2D alphaMask;

    // NEW FEATURES - Multiple Resolutions
    private bool saveAt1x = true;
    private bool saveAt2x = false;
    private bool saveAt4x = false;

    // NEW FEATURES - GPU Acceleration
    private bool useGPUAcceleration = false;
    // GPU path is stubbed for now; material/shader creation kept minimal for future use
    private Material tilingMaterial;
    private Shader tilingShader;

    // NEW FEATURES - Auto Make Readable
    private bool autoMakeReadable = true;

    private const int MIN_REPEATS = 1;
    private const int MAX_REPEATS = 10;
    private const int PREVIEW_MAX_SIZE = 400;

    // Background processing variables
    private bool isProcessing = false;
    private float processingProgress = 0f;
    private string processingStatus = "";
    private CancellationTokenSource cancellationTokenSource;

    // Performance thresholds
    private const int BACKGROUND_THRESHOLD_PIXELS = 2048 * 2048; // 4MP threshold

    // UI state variables for responsiveness
    private Vector2 mainScrollPosition = Vector2.zero;
    private bool showAdvancedSettings = false;
    private bool showRandomizationSettings = false;
    private bool showAdvancedPatternControls = false;
    private bool showColorEffects = false;
    private bool showExportOptions = false;

    [MenuItem("Tools/Texture Pattern Generator")]
    public static void ShowWindow()
    {
        TexturePatternGenerator window = GetWindow<TexturePatternGenerator>();
        window.titleContent = new GUIContent("Pattern Generator");
        window.minSize = new Vector2(450, 900);
        window.Show();
    }

    private void OnEnable()
    {
        CreateTilingShader();
    }

    private void OnGUI()
    {
        // Main scroll view for window responsiveness
        mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Texture Pattern Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        DrawSourceTextureSection();
        EditorGUILayout.Space(10);

        DrawPatternSettingsSection();
        EditorGUILayout.Space(10);

        DrawAdvancedPatternControlsSection();
        EditorGUILayout.Space(10);

        DrawRandomizationSection();
        EditorGUILayout.Space(10);

        DrawColorEffectsSection();
        EditorGUILayout.Space(10);

        DrawBackgroundSection();
        EditorGUILayout.Space(10);

        DrawExportOptionsSection();
        EditorGUILayout.Space(10);

        DrawOutputSettingsSection();
        EditorGUILayout.Space(10);

        DrawPreviewSection();
        EditorGUILayout.Space(10);

        DrawGenerateSection();

        EditorGUILayout.EndScrollView();
    }

    #region UI Sections
    private void DrawSourceTextureSection()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Source Texture", EditorStyles.boldLabel);

        Texture2D newSourceTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Texture2D", sourceTexture, typeof(Texture2D), false);

        if (newSourceTexture != sourceTexture)
        {
            sourceTexture = newSourceTexture;
            UpdatePreview();
        }

        if (sourceTexture != null)
        {
            EditorGUILayout.LabelField($"Size: {sourceTexture.width} x {sourceTexture.height}");
            EditorGUILayout.LabelField($"Format: {sourceTexture.format}");

            if (!sourceTexture.isReadable)
            {
                EditorGUILayout.HelpBox("Texture is not readable. Enable 'Auto Make Readable' below or manually enable Read/Write in import settings.", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a source texture to begin.", MessageType.Info);
        }

        bool newAutoMakeReadable = EditorGUILayout.Toggle("Auto Make Readable", autoMakeReadable);
        if (newAutoMakeReadable != autoMakeReadable)
        {
            autoMakeReadable = newAutoMakeReadable;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawPatternSettingsSection()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Pattern Settings", EditorStyles.boldLabel);

        int newHorizontalRepeats = EditorGUILayout.IntSlider(
            "Horizontal Repeats", horizontalRepeats, MIN_REPEATS, MAX_REPEATS);

        int newVerticalRepeats = EditorGUILayout.IntSlider(
            "Vertical Repeats", verticalRepeats, MIN_REPEATS, MAX_REPEATS);

        if (newHorizontalRepeats != horizontalRepeats || newVerticalRepeats != verticalRepeats)
        {
            horizontalRepeats = newHorizontalRepeats;
            verticalRepeats = newVerticalRepeats;
            UpdatePreview();
        }

        if (sourceTexture != null)
        {
            int finalWidth = sourceTexture.width * horizontalRepeats;
            int finalHeight = sourceTexture.height * verticalRepeats;
            EditorGUILayout.LabelField($"Output Size: {finalWidth} x {finalHeight}");
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawAdvancedPatternControlsSection()
    {
        EditorGUILayout.BeginVertical("Box");
        showAdvancedPatternControls = EditorGUILayout.Foldout(showAdvancedPatternControls, "Advanced Pattern Controls");

        if (showAdvancedPatternControls)
        {
            EditorGUI.indentLevel++;

            // Pattern Offset Controls
            EditorGUILayout.LabelField("Pattern Offset", EditorStyles.boldLabel);
            int newPatternOffsetX = EditorGUILayout.IntField("X Offset (pixels)", patternOffsetX);
            int newPatternOffsetY = EditorGUILayout.IntField("Y Offset (pixels)", patternOffsetY);

            if (newPatternOffsetX != patternOffsetX || newPatternOffsetY != patternOffsetY)
            {
                patternOffsetX = newPatternOffsetX;
                patternOffsetY = newPatternOffsetY;
                UpdatePreview();
            }

            EditorGUILayout.Space(5);

            // Pattern Symmetry/Mirroring
            EditorGUILayout.LabelField("Pattern Symmetry", EditorStyles.boldLabel);
            bool newFlipAlternateHorizontal = EditorGUILayout.Toggle("Flip Alternate Horizontal", flipAlternateHorizontal);
            bool newFlipAlternateVertical = EditorGUILayout.Toggle("Flip Alternate Vertical", flipAlternateVertical);

            if (newFlipAlternateHorizontal != flipAlternateHorizontal || newFlipAlternateVertical != flipAlternateVertical)
            {
                flipAlternateHorizontal = newFlipAlternateHorizontal;
                flipAlternateVertical = newFlipAlternateVertical;
                UpdatePreview();
            }

            EditorGUILayout.Space(5);

            // Alpha Mask Support
            EditorGUILayout.LabelField("Alpha Mask", EditorStyles.boldLabel);
            Texture2D newAlphaMask = (Texture2D)EditorGUILayout.ObjectField(
                "Alpha Mask Texture", alphaMask, typeof(Texture2D), false);

            if (newAlphaMask != alphaMask)
            {
                alphaMask = newAlphaMask;
                UpdatePreview();
            }

            if (alphaMask != null && !alphaMask.isReadable)
            {
                EditorGUILayout.HelpBox("Alpha mask texture is not readable. Enable 'Auto Make Readable' above or manually enable Read/Write in import settings.", MessageType.Warning);
            }

            EditorGUILayout.Space(5);

            // GPU Acceleration
            bool newUseGPUAcceleration = EditorGUILayout.Toggle("Use GPU Acceleration", useGPUAcceleration);
            if (newUseGPUAcceleration != useGPUAcceleration)
            {
                useGPUAcceleration = newUseGPUAcceleration;
                UpdatePreview();
            }

            if (useGPUAcceleration)
            {
                EditorGUILayout.HelpBox("GPU acceleration may provide faster generation for large textures but may not support all features yet (falls back to CPU).", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawColorEffectsSection()
    {
        EditorGUILayout.BeginVertical("Box");
        showColorEffects = EditorGUILayout.Foldout(showColorEffects, "Color & Effects");

        if (showColorEffects)
        {
            EditorGUI.indentLevel++;

            // Global Tint
            Color newGlobalTint = EditorGUILayout.ColorField("Global Tint", globalTint);
            if (newGlobalTint != globalTint)
            {
                globalTint = newGlobalTint;
                UpdatePreview();
            }

            EditorGUILayout.Space(5);

            // Random Hue Shift
            bool newUseRandomHueShift = EditorGUILayout.Toggle("Random Hue Shift", useRandomHueShift);
            if (newUseRandomHueShift != useRandomHueShift)
            {
                useRandomHueShift = newUseRandomHueShift;
                UpdatePreview();
            }

            if (useRandomHueShift)
            {
                EditorGUI.indentLevel++;
                float newHueShiftRange = EditorGUILayout.Slider("Hue Shift Range", hueShiftRange, 0f, 1f);
                if (Mathf.Abs(newHueShiftRange - hueShiftRange) > 0.001f)
                {
                    hueShiftRange = newHueShiftRange;
                    UpdatePreview();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawExportOptionsSection()
    {
        EditorGUILayout.BeginVertical("Box");
        showExportOptions = EditorGUILayout.Foldout(showExportOptions, "Export Options");

        if (showExportOptions)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Save at Multiple Resolutions", EditorStyles.boldLabel);

            bool newSaveAt1x = EditorGUILayout.Toggle("Save at 1x", saveAt1x);
            bool newSaveAt2x = EditorGUILayout.Toggle("Save at 2x", saveAt2x);
            bool newSaveAt4x = EditorGUILayout.Toggle("Save at 4x", saveAt4x);

            if (newSaveAt1x != saveAt1x || newSaveAt2x != saveAt2x || newSaveAt4x != saveAt4x)
            {
                saveAt1x = newSaveAt1x;
                saveAt2x = newSaveAt2x;
                saveAt4x = newSaveAt4x;
            }

            if (!saveAt1x && !saveAt2x && !saveAt4x)
            {
                EditorGUILayout.HelpBox("Please select at least one resolution to export.", MessageType.Warning);
                saveAt1x = true;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawRandomizationSection()
    {
        EditorGUILayout.BeginVertical("Box");
        showRandomizationSettings = EditorGUILayout.Foldout(showRandomizationSettings, "Randomization Settings");

        if (showRandomizationSettings)
        {
            EditorGUI.indentLevel++;

            // Random Seed
            int newRandomSeed = EditorGUILayout.IntField("Random Seed", randomSeed);
            if (newRandomSeed != randomSeed)
            {
                randomSeed = newRandomSeed;
                UpdatePreview();
            }

            EditorGUILayout.Space(5);

            // Position Randomization
            bool newUsePositionRandomization = EditorGUILayout.Toggle("Position Randomization", usePositionRandomization);
            if (newUsePositionRandomization != usePositionRandomization)
            {
                usePositionRandomization = newUsePositionRandomization;
                UpdatePreview();
            }

            if (usePositionRandomization)
            {
                EditorGUI.indentLevel++;

                float newPositionRandomnessX = EditorGUILayout.Slider("X Offset Range", positionRandomnessX, 0f, 1f);
                float newPositionRandomnessY = EditorGUILayout.Slider("Y Offset Range", positionRandomnessY, 0f, 1f);

                if (Mathf.Abs(newPositionRandomnessX - positionRandomnessX) > 0.001f ||
                    Mathf.Abs(newPositionRandomnessY - positionRandomnessY) > 0.001f)
                {
                    positionRandomnessX = newPositionRandomnessX;
                    positionRandomnessY = newPositionRandomnessY;
                    UpdatePreview();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Scale Randomization
            bool newUseScaleRandomization = EditorGUILayout.Toggle("Scale Randomization", useScaleRandomization);
            if (newUseScaleRandomization != useScaleRandomization)
            {
                useScaleRandomization = newUseScaleRandomization;
                UpdatePreview();
            }

            if (useScaleRandomization)
            {
                EditorGUI.indentLevel++;

                float newMinScale = EditorGUILayout.Slider("Min Scale", minScale, 0.1f, 2f);
                float newMaxScale = EditorGUILayout.Slider("Max Scale", maxScale, 0.1f, 2f);

                if (newMaxScale < newMinScale) newMaxScale = newMinScale;

                if (Mathf.Abs(newMinScale - minScale) > 0.001f || Mathf.Abs(newMaxScale - maxScale) > 0.001f)
                {
                    minScale = newMinScale;
                    maxScale = newMaxScale;
                    UpdatePreview();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Rotation Randomization
            bool newUseRotationRandomization = EditorGUILayout.Toggle("Rotation Randomization", useRotationRandomization);
            if (newUseRotationRandomization != useRotationRandomization)
            {
                useRotationRandomization = newUseRotationRandomization;
                UpdatePreview();
            }

            if (useRotationRandomization)
            {
                EditorGUI.indentLevel++;

                float newRotationRandomness = EditorGUILayout.Slider("Rotation Range (°)", rotationRandomness, 0f, 360f);

                if (Mathf.Abs(newRotationRandomness - rotationRandomness) > 0.001f)
                {
                    rotationRandomness = newRotationRandomness;
                    UpdatePreview();
                }

                EditorGUI.indentLevel--;
            }

            if (usePositionRandomization || useScaleRandomization || useRotationRandomization)
            {
                EditorGUILayout.HelpBox("Randomization adds variety to each repeated instance of the texture.", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawBackgroundSection()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Background Settings", EditorStyles.boldLabel);

        BackgroundType newBackgroundType = (BackgroundType)EditorGUILayout.EnumPopup("Background Type", backgroundType);
        if (newBackgroundType != backgroundType)
        {
            backgroundType = newBackgroundType;
            UpdatePreview();
        }

        switch (backgroundType)
        {
            case BackgroundType.SolidColor:
                Color newBackgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
                if (newBackgroundColor != backgroundColor)
                {
                    backgroundColor = newBackgroundColor;
                    UpdatePreview();
                }
                break;

            case BackgroundType.Texture:
                Texture2D newBackgroundTexture = (Texture2D)EditorGUILayout.ObjectField(
                    "Background Texture", backgroundTexture, typeof(Texture2D), false);
                if (newBackgroundTexture != backgroundTexture)
                {
                    backgroundTexture = newBackgroundTexture;
                    UpdatePreview();
                }

                if (backgroundTexture != null)
                {
                    EditorGUILayout.LabelField($"Background Size: {backgroundTexture.width} x {backgroundTexture.height}");
                    if (!backgroundTexture.isReadable)
                    {
                        EditorGUILayout.HelpBox("Background texture is not readable. Enable 'Auto Make Readable' above or manually enable Read/Write in import settings.", MessageType.Warning);
                    }
                }
                break;
        }

        if (backgroundType != BackgroundType.None)
        {
            bool newUseTransparency = EditorGUILayout.Toggle("Enable Transparency", useTransparency);
            if (newUseTransparency != useTransparency)
            {
                useTransparency = newUseTransparency;
                UpdatePreview();
            }

            if (useTransparency)
            {
                float newBackgroundAlpha = EditorGUILayout.Slider("Background Opacity", backgroundAlpha, 0f, 1f);
                if (Mathf.Abs(newBackgroundAlpha - backgroundAlpha) > 0.001f)
                {
                    backgroundAlpha = newBackgroundAlpha;
                    UpdatePreview();
                }
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawOutputSettingsSection()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);

        outputFileName = EditorGUILayout.TextField("File Name", outputFileName);

        if (string.IsNullOrEmpty(outputFileName.Trim()))
        {
            EditorGUILayout.HelpBox("Please enter a valid file name.", MessageType.Warning);
        }

        // Advanced settings toggle
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;

            if (sourceTexture != null && backgroundType != BackgroundType.None)
            {
                EditorGUILayout.HelpBox("Background will be applied behind the pattern.", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        // Seamless Tiling Preview Toggle
        bool newSeamlessTilingPreview = EditorGUILayout.Toggle("Seamless Tiling Preview", seamlessTilingPreview);
        if (newSeamlessTilingPreview != seamlessTilingPreview)
        {
            seamlessTilingPreview = newSeamlessTilingPreview;
            UpdatePreview();
        }

        if (seamlessTilingPreview)
        {
            EditorGUI.indentLevel++;
            int newTilingPreviewRepeats = EditorGUILayout.IntSlider("Tiling Repeats", tilingPreviewRepeats, 2, 6);
            if (newTilingPreviewRepeats != tilingPreviewRepeats)
            {
                tilingPreviewRepeats = newTilingPreviewRepeats;
                UpdatePreview();
            }
            EditorGUI.indentLevel--;
        }

        if (previewTexture != null)
        {
            float aspectRatio = (float)previewTexture.width / previewTexture.height;
            float previewWidth = Mathf.Min(PREVIEW_MAX_SIZE, previewTexture.width);
            float previewHeight = previewWidth / aspectRatio;

            if (previewHeight > PREVIEW_MAX_SIZE)
            {
                previewHeight = PREVIEW_MAX_SIZE;
                previewWidth = previewHeight * aspectRatio;
            }

            // Responsive preview with proper scrolling
            float availableWidth = EditorGUIUtility.currentViewWidth - 40; // Account for padding
            if (previewWidth > availableWidth)
            {
                float scale = availableWidth / previewWidth;
                previewWidth = availableWidth;
                previewHeight *= scale;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Height(Mathf.Min(previewHeight + 40, PREVIEW_MAX_SIZE + 20)),
                GUILayout.ExpandWidth(true));

            Rect previewRect = GUILayoutUtility.GetRect(
                previewWidth,
                previewHeight,
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false));

            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawPreviewTexture(previewRect, previewTexture);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField($"Preview Size: {previewTexture.width} x {previewTexture.height}");
        }
        else if (sourceTexture != null)
        {
            EditorGUILayout.HelpBox("Generating preview...", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("No preview available. Please select a source texture.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawGenerateSection()
    {
        EditorGUILayout.BeginVertical("Box");

        // Show progress bar when processing
        if (isProcessing)
        {
            EditorGUILayout.LabelField("Processing...", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            Rect progressRect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(progressRect, processingProgress, processingStatus);

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Cancel", GUILayout.Height(25)))
            {
                CancelProcessing();
            }
        }
        else
        {
            GUI.enabled = sourceTexture != null && !string.IsNullOrEmpty(outputFileName.Trim());

            if (GUILayout.Button("Generate and Save Pattern", GUILayout.Height(30)))
            {
                GenerateAndSavePattern();
            }

            GUI.enabled = true;

            if (sourceTexture == null)
            {
                EditorGUILayout.HelpBox("Please select a source texture before generating.", MessageType.Warning);
            }
            else if (string.IsNullOrEmpty(outputFileName.Trim()))
            {
                EditorGUILayout.HelpBox("Please enter a valid file name before generating.", MessageType.Warning);
            }
            else if (ShouldUseBackgroundProcessing())
            {
                EditorGUILayout.HelpBox("Large texture detected. Processing will run in background to prevent Editor freezing.", MessageType.Info);
            }
        }

        EditorGUILayout.EndVertical();
    }
    #endregion

    #region Preview / Generation Control
    private void UpdatePreview()
    {
        if (sourceTexture == null)
        {
            if (previewTexture != null)
            {
                DestroyImmediate(previewTexture);
                previewTexture = null;
            }
            return;
        }

        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
        }

        // Ensure textures are readable for preview
        if (autoMakeReadable)
        {
            MakeTextureReadable(sourceTexture);
            if (backgroundTexture != null) MakeTextureReadable(backgroundTexture);
            if (alphaMask != null) MakeTextureReadable(alphaMask);
        }

        // Generate preview with all new features
        int previewHRepeats = seamlessTilingPreview ? tilingPreviewRepeats : horizontalRepeats;
        int previewVRepeats = seamlessTilingPreview ? tilingPreviewRepeats : verticalRepeats;

        // If GPU mode requested, currently fall back to CPU and inform user
        if (useGPUAcceleration)
        {
            // TODO: implement GPU generation path (shader + RenderTexture)
            // For now fallback to CPU generation and notify user once
            // (so it doesn't silently ignore the flag)
            // We do not spam this message on every preview update.
            if (!EditorPrefs.HasKey("TPG_GPUFallbackShown"))
            {
                EditorUtility.DisplayDialog("GPU Acceleration", "GPU acceleration is not implemented in this build. Falling back to CPU path. (You can implement the shader path in CreateTilingShader + GPU Blit.)", "OK");
                EditorPrefs.SetBool("TPG_GPUFallbackShown", true);
            }
        }

        previewTexture = GeneratePatternTexture(sourceTexture, previewHRepeats, previewVRepeats, true);
        Repaint();
    }

    private bool ShouldUseBackgroundProcessing()
    {
        if (sourceTexture == null) return false;

        long totalPixels = (long)sourceTexture.width * horizontalRepeats * (long)sourceTexture.height * verticalRepeats;
        return totalPixels > BACKGROUND_THRESHOLD_PIXELS;
    }

    private void CancelProcessing()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
        }

        isProcessing = false;
        processingProgress = 0f;
        processingStatus = "";
        Repaint();
    }
    #endregion

    #region Importer Helpers
    private void MakeTextureReadable(Texture2D texture)
    {
        if (texture == null) return;
        if (texture.isReadable) return;

        try
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(assetPath)) return;

            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.SaveAndReimport();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to make texture readable: {e.Message}");
        }
    }
    #endregion

    #region Core Generation (CPU)
    private Texture2D GeneratePatternTexture(Texture2D source, int hRepeats, int vRepeats, bool isPreview = false)
    {
        if (source == null || !source.isReadable)
        {
            Debug.LogError("Source texture is null or not readable. Please ensure the texture import settings allow 'Read/Write'.");
            return null;
        }

        int outputWidth = source.width * hRepeats;
        int outputHeight = source.height * vRepeats;

        // Use RGBA32 format to ensure SetPixels compatibility and transparency support
        Texture2D outputTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.RGBA32, false);

        try
        {
            Color[] sourcePixels = source.GetPixels();
            Color[] outputPixels = new Color[outputWidth * outputHeight];

            // Initialize with transparent background
            for (int i = 0; i < outputPixels.Length; i++) outputPixels[i] = Color.clear;

            // First, fill with background if specified
            if (backgroundType != BackgroundType.None)
            {
                FillBackgroundPixels(outputPixels, outputWidth, outputHeight);
            }

            // Get alpha mask pixels if available
            Color[] maskPixels = null;
            int maskWidth = 0, maskHeight = 0;
            if (alphaMask != null)
            {
                if (!alphaMask.isReadable)
                {
                    if (autoMakeReadable) MakeTextureReadable(alphaMask);
                }

                if (alphaMask.isReadable)
                {
                    maskPixels = alphaMask.GetPixels();
                    maskWidth = alphaMask.width;
                    maskHeight = alphaMask.height;
                }
            }

            // Set up random state
            UnityEngine.Random.State originalState = UnityEngine.Random.state;
            UnityEngine.Random.InitState(randomSeed);

            int totalRepeats = hRepeats * vRepeats;
            int processedRepeats = 0;

            for (int repeatY = 0; repeatY < vRepeats; repeatY++)
            {
                for (int repeatX = 0; repeatX < hRepeats; repeatX++)
                {
                    // Optional: allow cancellation while generating huge previews
                    if (isPreview == false && cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                        throw new OperationCanceledException();

                    bool flipX = flipAlternateHorizontal && (repeatX % 2 == 1);
                    bool flipY = flipAlternateVertical && (repeatY % 2 == 1);

                    // Calculate base position with offsets
                    int baseX = repeatX * source.width + patternOffsetX;
                    int baseY = repeatY * source.height + patternOffsetY;

                    // Randomization offsets (per instance)
                    float posOffsetX = 0f;
                    float posOffsetY = 0f;
                    if (usePositionRandomization)
                    {
                        posOffsetX = UnityEngine.Random.Range(-positionRandomnessX, positionRandomnessX) * source.width;
                        posOffsetY = UnityEngine.Random.Range(-positionRandomnessY, positionRandomnessY) * source.height;
                    }

                    float scale = 1f;
                    if (useScaleRandomization) scale = UnityEngine.Random.Range(minScale, maxScale);

                    float rotationDeg = 0f;
                    if (useRotationRandomization) rotationDeg = UnityEngine.Random.Range(-rotationRandomness * 0.5f, rotationRandomness * 0.5f);
                    float rotationRad = rotationDeg * Mathf.Deg2Rad;
                    float cos = Mathf.Cos(rotationRad);
                    float sin = Mathf.Sin(rotationRad);

                    Vector2 centerOffset = new Vector2(source.width * 0.5f, source.height * 0.5f);

                    for (int y = 0; y < source.height; y++)
                    {
                        for (int x = 0; x < source.width; x++)
                        {
                            int srcX = flipX ? (source.width - 1 - x) : x;
                            int srcY = flipY ? (source.height - 1 - y) : y;

                            Color srcColor = sourcePixels[srcY * source.width + srcX];
                            if (srcColor.a <= 0f) continue; // skip fully transparent pixels

                            // Apply global tint
                            Color pixel = new Color(srcColor.r * globalTint.r, srcColor.g * globalTint.g, srcColor.b * globalTint.b, srcColor.a * globalTint.a);

                            // Random hue shift per tile if enabled
                            if (useRandomHueShift && hueShiftRange > 0f)
                            {
                                float h, s, v;
                                Color.RGBToHSV(pixel, out h, out s, out v);
                                h = (h + UnityEngine.Random.Range(-hueShiftRange, hueShiftRange));
                                // wrap H into 0..1
                                h = h - Mathf.Floor(h);
                                Color rgb = Color.HSVToRGB(h, s, v);
                                pixel.r = rgb.r;
                                pixel.g = rgb.g;
                                pixel.b = rgb.b;
                                // Preserve alpha from original tinted pixel
                            }

                            // Alpha mask test (mask repeats)
                            if (maskPixels != null && maskWidth > 0 && maskHeight > 0)
                            {
                                int maskX = x % maskWidth;
                                int maskY = y % maskHeight;
                                float maskAlpha = maskPixels[maskY * maskWidth + maskX].a;
                                if (maskAlpha < 0.5f) continue;
                            }

                            // Apply scale & rotation around tile center
                            Vector2 local = new Vector2(x, y) - centerOffset;
                            local *= scale;

                            if (useRotationRandomization && Mathf.Abs(rotationDeg) > 0.001f)
                            {
                                float newX = local.x * cos - local.y * sin;
                                float newY = local.x * sin + local.y * cos;
                                local = new Vector2(newX, newY);
                            }

                            // Final destination
                            int dstX = Mathf.RoundToInt(baseX + posOffsetX + centerOffset.x + local.x);
                            int dstY = Mathf.RoundToInt(baseY + posOffsetY + centerOffset.y + local.y);

                            // If preview seamless mode is used, wrap coordinates
                            if (seamlessTilingPreview)
                            {
                                dstX = ((dstX % outputWidth) + outputWidth) % outputWidth;
                                dstY = ((dstY % outputHeight) + outputHeight) % outputHeight;
                            }

                            if (dstX < 0 || dstX >= outputWidth || dstY < 0 || dstY >= outputHeight) continue;

                            int dstIndex = dstY * outputWidth + dstX;

                            // Alpha blending
                            Color dest = outputPixels[dstIndex];
                            outputPixels[dstIndex] = BlendColors(dest, pixel);
                        }
                    }

                    // progress (for long synchronous runs; preview skips precise progress)
                    processedRepeats++;
                    if (!isPreview)
                    {
                        processingProgress = (float)processedRepeats / (float)totalRepeats * 0.9f;
                        processingStatus = $"Compositing tile {processedRepeats}/{totalRepeats}";
                        Repaint();
                    }
                }
            }

            // Restore random state
            UnityEngine.Random.state = originalState;

            outputTexture.SetPixels(outputPixels);
            outputTexture.Apply();
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("Pattern generation cancelled.");
            DestroyImmediate(outputTexture);
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error generating pattern: {ex}");
            DestroyImmediate(outputTexture);
            return null;
        }

        return outputTexture;
    }

    private void FillBackgroundPixels(Color[] pixels, int width, int height)
    {
        switch (backgroundType)
        {
            case BackgroundType.SolidColor:
                Color bgColor = backgroundColor;
                if (useTransparency) bgColor.a = backgroundAlpha;
                for (int i = 0; i < pixels.Length; i++) pixels[i] = bgColor;
                break;

            case BackgroundType.Texture:
                if (backgroundTexture == null) return;
                if (!backgroundTexture.isReadable)
                {
                    if (autoMakeReadable) MakeTextureReadable(backgroundTexture);
                }

                if (!backgroundTexture.isReadable) return;

                Color[] bgPixels = backgroundTexture.GetPixels();
                int bgW = backgroundTexture.width;
                int bgH = backgroundTexture.height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int bgX = x % bgW;
                        int bgY = y % bgH;
                        Color c = bgPixels[bgY * bgW + bgX];
                        if (useTransparency) c.a *= backgroundAlpha;
                        pixels[y * width + x] = c;
                    }
                }
                break;
        }
    }

    private Color BlendColors(Color background, Color foreground)
    {
        // Standard alpha compositing - foreground over background
        float fa = foreground.a;
        float ba = background.a * (1f - fa);
        float outA = fa + background.a * (1f - fa);

        if (outA <= 0f) return Color.clear;

        float r = (foreground.r * fa + background.r * background.a * (1f - fa)) / outA;
        float g = (foreground.g * fa + background.g * background.a * (1f - fa)) / outA;
        float b = (foreground.b * fa + background.b * background.a * (1f - fa)) / outA;

        return new Color(r, g, b, outA);
    }
    #endregion

    #region Async / Save
    private async void GenerateAndSavePattern()
    {
        if (sourceTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a source texture first.", "OK");
            return;
        }

        if (!sourceTexture.isReadable)
        {
            if (autoMakeReadable) MakeTextureReadable(sourceTexture);
            if (!sourceTexture.isReadable)
            {
                EditorUtility.DisplayDialog("Error", "The selected texture is not readable. Please enable 'Read/Write' in the texture import settings and try again.", "OK");
                return;
            }
        }

        if (backgroundType == BackgroundType.Texture && backgroundTexture != null && !backgroundTexture.isReadable)
        {
            if (autoMakeReadable) MakeTextureReadable(backgroundTexture);
            if (!backgroundTexture.isReadable)
            {
                EditorUtility.DisplayDialog("Error", "The selected background texture is not readable. Please enable 'Read/Write' in the texture import settings and try again.", "OK");
                return;
            }
        }

        if (alphaMask != null && !alphaMask.isReadable)
        {
            if (autoMakeReadable) MakeTextureReadable(alphaMask);
            if (!alphaMask.isReadable)
            {
                EditorUtility.DisplayDialog("Error", "The alpha mask texture is not readable. Please enable 'Read/Write' in the texture import settings and try again.", "OK");
                return;
            }
        }

        string fileName = outputFileName.Trim();
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a valid file name.", "OK");
            return;
        }

        // Decide whether to run in background
        bool useBackground = ShouldUseBackgroundProcessing();

        // If GPU requested but not implemented, inform user (we fallback to CPU path)
        if (useGPUAcceleration)
        {
            if (!EditorPrefs.HasKey("TPG_GPUFallbackShown_Save"))
            {
                EditorUtility.DisplayDialog("GPU Acceleration", "GPU acceleration not implemented yet; falling back to CPU generation for save.", "OK");
                EditorPrefs.SetBool("TPG_GPUFallbackShown_Save", true);
            }
        }

        if (useBackground)
        {
            await GenerateAndSavePatternAsync(fileName);
        }
        else
        {
            GenerateAndSavePatternSync(fileName);
        }
    }

    private void GenerateAndSavePatternSync(string fileName)
    {
        isProcessing = true;
        processingProgress = 0f;
        processingStatus = "Generating (sync)...";
        Repaint();

        Texture2D generatedTexture = GeneratePatternTexture(sourceTexture, horizontalRepeats, verticalRepeats, false);

        if (generatedTexture == null)
        {
            isProcessing = false;
            processingStatus = "";
            EditorUtility.DisplayDialog("Error", "Failed to generate pattern texture.", "OK");
            return;
        }

        processingProgress = 0.95f;
        processingStatus = "Saving...";
        Repaint();

        // Save at selected scales
        SaveAtSelectedScales(generatedTexture, fileName);

        DestroyImmediate(generatedTexture);

        processingProgress = 1f;
        processingStatus = "Complete";
        isProcessing = false;
        Repaint();
    }

    private async Task GenerateAndSavePatternAsync(string fileName)
    {
        isProcessing = true;
        processingProgress = 0f;
        processingStatus = "Initializing background generation...";
        cancellationTokenSource = new CancellationTokenSource();
        Repaint();

        try
        {
            // prepare source pixels on main thread
            if (!sourceTexture.isReadable && autoMakeReadable) MakeTextureReadable(sourceTexture);
            Color[] sourcePixels = sourceTexture.GetPixels();
            int sW = sourceTexture.width;
            int sH = sourceTexture.height;

            // Background & mask reading
            Color[] bgPixels = null; int bgW = 0, bgH = 0;
            if (backgroundType == BackgroundType.Texture && backgroundTexture != null)
            {
                if (!backgroundTexture.isReadable && autoMakeReadable) MakeTextureReadable(backgroundTexture);
            }

            if (alphaMask != null)
            {
                if (!alphaMask.isReadable && autoMakeReadable) MakeTextureReadable(alphaMask);
            }

            int outputW = sW * horizontalRepeats;
            int outputH = sH * verticalRepeats;

            processingStatus = "Compositing pixels in background...";
            Repaint();

            // Run the heavy pixel compositing on a background thread
            Color[] outPixels = await Task.Run(() =>
            {
                // We will reuse some of the CPU compositing logic but inside Task
                Color[] result = new Color[outputW * outputH];
                for (int i = 0; i < result.Length; i++) result[i] = Color.clear;

                // Fill background if needed (note: using main thread asset access isn't allowed—so avoid reading textures here)
                // For safety, we read background and mask from main thread earlier, but to keep this simple we do only core tiling on background thread.
                // Therefore backgroundTexture/mask effects are applied after Task completes on main thread.
                // We'll composite tiles into result here without background/mask; then apply mask/bg on main thread.
                UnityEngine.Random.State orig = UnityEngine.Random.state;
                UnityEngine.Random.InitState(randomSeed);

                int total = horizontalRepeats * verticalRepeats;
                int count = 0;

                for (int ry = 0; ry < verticalRepeats; ry++)
                {
                    for (int rx = 0; rx < horizontalRepeats; rx++)
                    {
                        // Using same tile logic as CPU method, but without mask/bg
                        bool fx = flipAlternateHorizontal && (rx % 2 == 1);
                        bool fy = flipAlternateVertical && (ry % 2 == 1);

                        int baseX = rx * sW + patternOffsetX;
                        int baseY = ry * sH + patternOffsetY;

                        float posOffsetX = 0f, posOffsetY = 0f;
                        if (usePositionRandomization)
                        {
                            posOffsetX = UnityEngine.Random.Range(-positionRandomnessX, positionRandomnessX) * sW;
                            posOffsetY = UnityEngine.Random.Range(-positionRandomnessY, positionRandomnessY) * sH;
                        }

                        float scale = 1f;
                        if (useScaleRandomization) scale = UnityEngine.Random.Range(minScale, maxScale);

                        float rot = 0f;
                        if (useRotationRandomization) rot = UnityEngine.Random.Range(-rotationRandomness * 0.5f, rotationRandomness * 0.5f);
                        float rotR = rot * Mathf.Deg2Rad;
                        float c = Mathf.Cos(rotR), s = Mathf.Sin(rotR);

                        Vector2 center = new Vector2(sW * 0.5f, sH * 0.5f);

                        for (int y = 0; y < sH; y++)
                        {
                            for (int x = 0; x < sW; x++)
                            {
                                int sx = fx ? (sW - 1 - x) : x;
                                int sy = fy ? (sH - 1 - y) : y;
                                Color sp = sourcePixels[sy * sW + sx];
                                if (sp.a <= 0f) continue;

                                Color pixel = new Color(sp.r * globalTint.r, sp.g * globalTint.g, sp.b * globalTint.b, sp.a * globalTint.a);

                                if (useRandomHueShift && hueShiftRange > 0f)
                                {
                                    float hh, ss, vv;
                                    Color.RGBToHSV(pixel, out hh, out ss, out vv);
                                    hh = (hh + UnityEngine.Random.Range(-hueShiftRange, hueShiftRange));
                                    hh = hh - Mathf.Floor(hh);
                                    Color col = Color.HSVToRGB(hh, ss, vv);
                                    pixel.r = col.r; pixel.g = col.g; pixel.b = col.b;
                                }

                                // transform
                                Vector2 local = new Vector2(x, y) - center;
                                local *= scale;
                                if (Mathf.Abs(rot) > 0.001f)
                                {
                                    float nx = local.x * c - local.y * s;
                                    float ny = local.x * s + local.y * c;
                                    local = new Vector2(nx, ny);
                                }

                                int dx = Mathf.RoundToInt(baseX + posOffsetX + center.x + local.x);
                                int dy = Mathf.RoundToInt(baseY + posOffsetY + center.y + local.y);

                                if (seamlessTilingPreview)
                                {
                                    dx = ((dx % outputW) + outputW) % outputW;
                                    dy = ((dy % outputH) + outputH) % outputH;
                                }

                                if (dx < 0 || dx >= outputW || dy < 0 || dy >= outputH) continue;

                                int di = dy * outputW + dx;
                                Color dst = result[di];
                                result[di] = BlendColors(dst, pixel);
                            }
                        }

                        count++;
                        if (count % 4 == 0)
                        {
                            // coarse progress update via captured variable (can't call Repaint directly here)
                            processingProgress = 0.2f + (float)count / total * 0.6f;
                        }

                        if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                        {
                            UnityEngine.Random.state = orig;
                            throw new OperationCanceledException();
                        }
                    }
                }

                UnityEngine.Random.state = orig;
                return result;

            }, cancellationTokenSource.Token);

            // Apply background and mask on main thread now (safe to access textures)
            if (backgroundType != BackgroundType.None)
            {
                // Reconstruct final pixels by blending background first, then overlaying 'outPixels'
                Color[] finalPixels = outPixels;
                FillBackgroundPixels(finalPixels, outputW, outputH);

                // Now composite outPixels over background
                for (int i = 0; i < finalPixels.Length; i++)
                {
                    finalPixels[i] = BlendColors(finalPixels[i], outPixels[i]);
                }

                // Apply alpha mask if present (mask repeats)
                if (alphaMask != null && alphaMask.isReadable)
                {
                    Color[] maskP = alphaMask.GetPixels();
                    int mw = alphaMask.width;
                    int mh = alphaMask.height;

                    for (int y = 0; y < outputH; y++)
                    {
                        for (int x = 0; x < outputW; x++)
                        {
                            Color m = maskP[(y % mh) * mw + (x % mw)];
                            if (m.a < 0.5f)
                            {
                                finalPixels[y * outputW + x].a = 0f;
                            }
                        }
                    }
                }

                // Create texture and save
                Texture2D generatedTexture = new Texture2D(outputW, outputH, TextureFormat.RGBA32, false);
                generatedTexture.SetPixels(finalPixels);
                generatedTexture.Apply();

                // Save at selected scales
                SaveAtSelectedScales(generatedTexture, fileName);

                DestroyImmediate(generatedTexture);
            }
            else
            {
                // No background: outPixels already have composited tiles over clear bg
                Texture2D generatedTexture = new Texture2D(outputW, outputH, TextureFormat.RGBA32, false);
                generatedTexture.SetPixels(outPixels);
                generatedTexture.Apply();

                SaveAtSelectedScales(generatedTexture, fileName);
                DestroyImmediate(generatedTexture);
            }

            processingProgress = 1f;
            processingStatus = "Complete";
            Repaint();
        }
        catch (OperationCanceledException)
        {
            EditorUtility.DisplayDialog("Cancelled", "Pattern generation was cancelled.", "OK");
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to generate pattern: {e.Message}", "OK");
        }
        finally
        {
            isProcessing = false;
            processingProgress = 0;
            processingStatus = "";
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            Repaint();
        }
    }

    private void SaveAtSelectedScales(Texture2D tex, string fileNameBase)
    {
        if (saveAt1x) SaveTextureToFile(tex, fileNameBase + (saveAt1x && (saveAt2x || saveAt4x) ? "_1x" : ""));
        if (saveAt2x) SaveTextureToFile(ScaleTexture(tex, 2), fileNameBase + "_2x");
        if (saveAt4x) SaveTextureToFile(ScaleTexture(tex, 4), fileNameBase + "_4x");
    }

    private Texture2D ScaleTexture(Texture2D source, int scale)
    {
        if (scale <= 1) return source;

        int w = source.width * scale;
        int h = source.height * scale;
        Texture2D scaled = new Texture2D(w, h, source.format, false);
        Color[] src = source.GetPixels();
        Color[] dst = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float u = (float)x / (float)w;
                float v = (float)y / (float)h;
                int sx = Mathf.Clamp(Mathf.FloorToInt(u * source.width), 0, source.width - 1);
                int sy = Mathf.Clamp(Mathf.FloorToInt(v * source.height), 0, source.height - 1);
                dst[y * w + x] = src[sy * source.width + sx];
            }
        }

        scaled.SetPixels(dst);
        scaled.Apply();
        return scaled;
    }

    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        try
        {
            string safeName = fileName.Trim();
            if (string.IsNullOrEmpty(safeName)) safeName = "GeneratedPattern";

            string assetsPath = Path.Combine(Application.dataPath, safeName + ".png");
            string projectPath = "Assets/" + safeName + ".png";

            // Ensure unique filename
            int counter = 1;
            while (File.Exists(assetsPath))
            {
                string newName = $"{safeName}_{counter}";
                assetsPath = Path.Combine(Application.dataPath, newName + ".png");
                projectPath = "Assets/" + newName + ".png";
                counter++;
            }

            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(assetsPath, pngData);
            AssetDatabase.Refresh();

            UnityEngine.Object createdAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(projectPath);
            if (createdAsset != null)
            {
                Selection.activeObject = createdAsset;
                EditorGUIUtility.PingObject(createdAsset);
            }

            Debug.Log($"Saved pattern to {assetsPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save texture: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Failed to save texture: {e.Message}", "OK");
        }
    }
    #endregion

    #region Utilities & Cleanup
    private void CreateTilingShader()
    {
        // Minimal placeholder material/shader for future GPU implementation.
        // This creates a simple unlit shader if none exists.
        if (tilingShader == null)
        {
            tilingShader = Shader.Find("Unlit/Texture");
        }
        if (tilingMaterial == null && tilingShader != null)
        {
            tilingMaterial = new Material(tilingShader);
        }
    }

    private void OnDisable()
    {
        CancelProcessing();
        if (previewTexture != null)
        {
            DestroyImmediate(previewTexture);
            previewTexture = null;
        }
        if (tilingMaterial != null)
        {
            DestroyImmediate(tilingMaterial);
            tilingMaterial = null;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
    #endregion
}
