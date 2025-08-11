using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ScriptToTextExporter : EditorWindow
{
    // Root folder path to scan (always inside Assets)
    private string rootFolder = "Assets";

    // Node class representing folders and scripts
    private class Node
    {
        public string name;
        public string path; // Asset path like "Assets/Scripts/MyScript.cs"
        public bool isFolder;
        public List<Node> children = new List<Node>();
        public bool isExpanded = true;
        public bool isSelected = false; // Added selection state for scripts
    }

    private Node rootNode;
    private Vector2 scrollPos;
    private List<Node> selectedNodes = new List<Node>(); // Track multiple selected nodes
    private bool exportAsSeparateFiles = true; // Export preference
    
    // GUI Styles for theme-aware rendering with clear visual states
    private GUIStyle headerStyle;
    private GUIStyle folderStyle;
    private GUIStyle scriptStyle;
    private GUIStyle selectedScriptStyle;
    private GUIStyle buttonStyle;
    private GUIStyle toolbarStyle;
    private GUIStyle separatorStyle;
    private GUIStyle folderButtonStyle;

    [MenuItem("Tools/Script to text Exporter")]
    public static void ShowWindow()
    {
        ScriptToTextExporter window = GetWindow<ScriptToTextExporter>("Script Hierarchy");
        window.minSize = new Vector2(300, 400);
    }

    private void OnEnable()
    {
        BuildTree();
        InitializeStyles();
    }
    
    private void InitializeStyles()
    {
        // Initialize styles with clear, solid colors based on Unity's current theme
        bool isDarkTheme = EditorGUIUtility.isProSkin;
        
        // Header style for section titles
        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            normal = { textColor = isDarkTheme ? Color.white : Color.black }
        };
        
        // Folder style with clear colors
        folderStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = isDarkTheme ? new Color(0.7f, 0.9f, 1f) : new Color(0.2f, 0.4f, 0.8f) }
        };
        
        // Script style for normal (unselected) scripts - solid background
        scriptStyle = new GUIStyle("Button")
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(8, 8, 4, 4),
            normal = { 
                textColor = isDarkTheme ? Color.white : Color.black,
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.35f, 0.35f, 0.35f) : new Color(0.9f, 0.9f, 0.9f))
            },
            hover = {
                textColor = isDarkTheme ? Color.white : Color.black,
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.45f, 0.45f, 0.45f) : new Color(0.8f, 0.8f, 0.8f))
            }
        };
        
        // Selected script style with clear, high-contrast highlighting - solid background
        selectedScriptStyle = new GUIStyle("Button")
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(8, 8, 4, 4),
            fontStyle = FontStyle.Bold,
            normal = {
                textColor = isDarkTheme ? Color.black : Color.white,
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.3f, 0.7f, 1f) : new Color(0.2f, 0.5f, 0.9f))
            },
            hover = {
                textColor = isDarkTheme ? Color.black : Color.white,
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.4f, 0.8f, 1f) : new Color(0.1f, 0.4f, 0.8f))
            }
        };
        
        // Enhanced button style
        buttonStyle = new GUIStyle("Button")
        {
            fontStyle = FontStyle.Bold,
            normal = {
                textColor = isDarkTheme ? Color.white : Color.black
            }
        };
        
        // Folder button style for compact buttons
        folderButtonStyle = new GUIStyle("Button")
        {
            fontSize = 10,
            padding = new RectOffset(4, 4, 2, 2),
            margin = new RectOffset(2, 2, 1, 1),
            normal = {
                textColor = isDarkTheme ? Color.white : Color.black,
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.7f, 0.7f, 0.7f))
            },
            hover = {
                textColor = isDarkTheme ? Color.white : Color.black,
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.6f, 0.6f, 0.6f))
            }
        };
        
        // Toolbar style for top section
        toolbarStyle = new GUIStyle()
        {
            normal = {
                background = MakeTexture(1, 1, isDarkTheme ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.85f, 0.85f, 0.85f))
            },
            padding = new RectOffset(8, 8, 4, 4)
        };
        
        // Separator style
        separatorStyle = new GUIStyle()
        {
            normal = {
                background = MakeTexture(1, 2, isDarkTheme ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.6f, 0.6f, 0.6f))
            },
            margin = new RectOffset(0, 0, 4, 4)
        };
    }
    
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
            
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void OnGUI()
    {
        // Initialize styles if not already done (handles theme changes)
        if (headerStyle == null)
            InitializeStyles();
            
        // Main container with theme-aware background
        EditorGUILayout.BeginVertical();
        
        // Top toolbar section
        EditorGUILayout.BeginVertical(toolbarStyle);
        
        // Root folder controls with improved styling
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Root Folder:", headerStyle, GUILayout.Width(80));
        rootFolder = EditorGUILayout.TextField(rootFolder);
        if (GUILayout.Button("Browse", buttonStyle, GUILayout.Width(60)))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Root Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(folder))
            {
                if (folder.StartsWith(Application.dataPath))
                {
                    rootFolder = "Assets" + folder.Substring(Application.dataPath.Length).Replace('\\', '/');
                    BuildTree();
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Folder", "Please select a folder inside the Assets folder.", "OK");
                }
            }
        }
        if (GUILayout.Button("Refresh", buttonStyle, GUILayout.Width(60)))
        {
            BuildTree();
        }
        EditorGUILayout.EndHorizontal();

        // Selection controls with visual separation
        GUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();

        // Visual separator
        GUILayout.Space(2);
        EditorGUILayout.LabelField("", separatorStyle, GUILayout.Height(2));
        GUILayout.Space(4);

        // Tree view with enhanced scrolling
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        if (rootNode != null)
        {
            DrawNode(rootNode, 0);
        }
        else
        {
            EditorGUILayout.LabelField("No scripts found or invalid folder selected", 
                EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandHeight(true));
        }
        EditorGUILayout.EndScrollView();

        // Visual separator before selection panel
        GUILayout.Space(4);
        EditorGUILayout.LabelField("", separatorStyle, GUILayout.Height(2));

        // Selection info and actions with enhanced styling
        EditorGUILayout.BeginVertical(toolbarStyle);
        
        if (selectedNodes.Count > 0)
        {
            EditorGUILayout.LabelField($"Selected Scripts ({selectedNodes.Count}):", headerStyle);
            
            // Show selected script names with improved formatting
            int displayCount = Mathf.Min(selectedNodes.Count, 3);
            for (int i = 0; i < displayCount; i++)
            {
                EditorGUILayout.LabelField("  • " + selectedNodes[i].name, EditorStyles.miniLabel);
            }
            
            if (selectedNodes.Count > 3)
            {
                EditorGUILayout.LabelField($"  • ... and {selectedNodes.Count - 3} more", 
                    EditorStyles.miniLabel);
            }

            GUILayout.Space(8);

            // Export options with enhanced presentation
            EditorGUILayout.LabelField("Export Options:", headerStyle);
            exportAsSeparateFiles = EditorGUILayout.Toggle("Export as separate files", exportAsSeparateFiles);
            
            GUILayout.Space(8);

            // Action buttons with improved layout
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("📍 Ping All in Project", buttonStyle))
            {
                foreach (var node in selectedNodes)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(node.path);
                    EditorGUIUtility.PingObject(asset);
                }
            }

            string exportButtonText = exportAsSeparateFiles ? 
                "📄 Export to Separate Files" : "📋 Export to Single File";
            if (GUILayout.Button(exportButtonText, buttonStyle))
            {
                ExportSelectedScripts();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("No scripts selected", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(20);
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawNode(Node node, int indent)
    {
        EditorGUI.indentLevel = indent;

        if (node.isFolder)
        {
            // Enhanced folder rendering with selection buttons
            EditorGUILayout.BeginHorizontal();
            
            // Folder foldout with reduced width to make room for buttons
            bool wasExpanded = node.isExpanded;
            node.isExpanded = EditorGUILayout.Foldout(node.isExpanded, 
                $"📁 {node.name}", folderStyle);
                
            // Add selection buttons for folders with scripts
            int scriptCount = GetScriptCountInFolder(node);
            if (scriptCount > 0)
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button($"Select ({scriptCount})", folderButtonStyle, GUILayout.Width(70)))
                {
                    SelectScriptsInFolder(node);
                }
                
                if (GUILayout.Button("Deselect", folderButtonStyle, GUILayout.Width(60)))
                {
                    DeselectScriptsInFolder(node);
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Add subtle visual feedback for folder state changes
            if (wasExpanded != node.isExpanded)
                Repaint();
                
            if (node.isExpanded)
            {
                // Add slight indentation for visual hierarchy
                EditorGUI.indentLevel++;
                foreach (var child in node.children)
                {
                    DrawNode(child, indent + 1);
                }
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            // Enhanced script rendering with clear visual distinction between selected and unselected
            bool wasSelected = node.isSelected;
            GUIStyle currentStyle = wasSelected ? selectedScriptStyle : scriptStyle;
            
            // Use clear visual indicators for script state
            string displayName = wasSelected ? $"✓ 📄 {node.name}" : $"📄 {node.name}";
            bool selected = GUILayout.Toggle(wasSelected, displayName, currentStyle);

            if (selected != wasSelected)
            {
                node.isSelected = selected;
                
                if (selected)
                {
                    if (!selectedNodes.Contains(node))
                    {
                        selectedNodes.Add(node);
                    }
                }
                else
                {
                    selectedNodes.Remove(node);
                }
                
                Repaint();
            }
        }
    }

    private int GetScriptCountInFolder(Node folder)
    {
        int count = 0;
        CountScriptsRecursive(folder, ref count);
        return count;
    }

    private void CountScriptsRecursive(Node node, ref int count)
    {
        if (node == null) return;

        if (!node.isFolder)
        {
            count++;
        }
        else
        {
            foreach (var child in node.children)
            {
                CountScriptsRecursive(child, ref count);
            }
        }
    }

    private void SelectScriptsInFolder(Node folder)
    {
        SelectScriptsInFolderRecursive(folder);
        Repaint();
    }

    private void SelectScriptsInFolderRecursive(Node node)
    {
        if (node == null) return;

        if (!node.isFolder)
        {
            if (!node.isSelected)
            {
                node.isSelected = true;
                if (!selectedNodes.Contains(node))
                {
                    selectedNodes.Add(node);
                }
            }
        }
        else
        {
            foreach (var child in node.children)
            {
                SelectScriptsInFolderRecursive(child);
            }
        }
    }

    private void DeselectScriptsInFolder(Node folder)
    {
        DeselectScriptsInFolderRecursive(folder);
        Repaint();
    }

    private void DeselectScriptsInFolderRecursive(Node node)
    {
        if (node == null) return;

        if (!node.isFolder)
        {
            if (node.isSelected)
            {
                node.isSelected = false;
                selectedNodes.Remove(node);
            }
        }
        else
        {
            foreach (var child in node.children)
            {
                DeselectScriptsInFolderRecursive(child);
            }
        }
    }

    private void SelectAllScriptsRecursive(Node node)
    {
        if (node == null) return;

        if (!node.isFolder)
        {
            node.isSelected = true;
            if (!selectedNodes.Contains(node))
            {
                selectedNodes.Add(node);
            }
        }
        else
        {
            foreach (var child in node.children)
            {
                SelectAllScriptsRecursive(child);
            }
        }
    }

    private void ExportSelectedScripts()
    {
        if (selectedNodes.Count == 0) return;

        if (exportAsSeparateFiles)
        {
            ExportAsSeparateFiles();
        }
        else
        {
            ExportAsSingleFile();
        }
    }

    private void ExportAsSeparateFiles()
    {
        int successCount = 0;
        int failCount = 0;

        foreach (var node in selectedNodes)
        {
            string absPath = Path.Combine(Application.dataPath, node.path.Substring("Assets".Length).TrimStart('/', '\\'));

            if (File.Exists(absPath))
            {
                string exportPath = Path.Combine(Application.dataPath, node.name + "_export.txt");
                try
                {
                    string content = File.ReadAllText(absPath);
                    File.WriteAllText(exportPath, content);
                    successCount++;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to export {node.name}: {e.Message}");
                    failCount++;
                }
            }
            else
            {
                Debug.LogWarning("Script file not found at path: " + absPath);
                failCount++;
            }
        }

        AssetDatabase.Refresh();
        
        if (successCount > 0)
        {
            Debug.Log($"Successfully exported {successCount} script(s) to separate TXT files.");
        }
        
        if (failCount > 0)
        {
            Debug.LogWarning($"Failed to export {failCount} script(s).");
        }
    }

    private void ExportAsSingleFile()
    {
        System.Text.StringBuilder combinedContent = new System.Text.StringBuilder();
        int successCount = 0;
        int failCount = 0;

        // Add header with timestamp and file count
        combinedContent.AppendLine("// Combined Script Export");
        combinedContent.AppendLine($"// Generated on: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        combinedContent.AppendLine($"// Total files: {selectedNodes.Count}");
        combinedContent.AppendLine("// " + new string('=', 60));
        combinedContent.AppendLine();

        foreach (var node in selectedNodes)
        {
            string absPath = Path.Combine(Application.dataPath, node.path.Substring("Assets".Length).TrimStart('/', '\\'));

            if (File.Exists(absPath))
            {
                try
                {
                    string content = File.ReadAllText(absPath);
                    
                    // Add file separator and header
                    combinedContent.AppendLine($"// FILE: {node.name}");
                    combinedContent.AppendLine($"// PATH: {node.path}");
                    combinedContent.AppendLine("// " + new string('-', 40));
                    combinedContent.AppendLine();
                    combinedContent.AppendLine(content);
                    combinedContent.AppendLine();
                    combinedContent.AppendLine("// " + new string('-', 40));
                    combinedContent.AppendLine();
                    
                    successCount++;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to read {node.name}: {e.Message}");
                    failCount++;
                    
                    // Add error info to combined file
                    combinedContent.AppendLine($"// FILE: {node.name} (ERROR)");
                    combinedContent.AppendLine($"// ERROR: {e.Message}");
                    combinedContent.AppendLine("// " + new string('-', 40));
                    combinedContent.AppendLine();
                }
            }
            else
            {
                Debug.LogWarning("Script file not found at path: " + absPath);
                failCount++;
                
                // Add missing file info to combined file
                combinedContent.AppendLine($"// FILE: {node.name} (NOT FOUND)");
                combinedContent.AppendLine($"// PATH: {node.path}");
                combinedContent.AppendLine("// " + new string('-', 40));
                combinedContent.AppendLine();
            }
        }

        // Save the combined file
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string exportPath = Path.Combine(Application.dataPath, $"CombinedScripts_{timestamp}.txt");
        
        try
        {
            File.WriteAllText(exportPath, combinedContent.ToString());
            AssetDatabase.Refresh();
            
            Debug.Log($"Successfully exported {successCount} script(s) to combined file: {exportPath}");
            
            if (failCount > 0)
            {
                Debug.LogWarning($"Encountered issues with {failCount} script(s). Check the combined file for details.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save combined export file: {e.Message}");
        }
    }

    private void BuildTree()
    {
        // Clear previous selections when rebuilding tree
        selectedNodes.Clear();
        
        if (string.IsNullOrEmpty(rootFolder))
        {
            Debug.LogWarning("Root folder is empty");
            rootNode = null;
            return;
        }

        // Normalize rootFolder path
        string relativePath = rootFolder.Trim();

        if (!relativePath.StartsWith("Assets"))
        {
            Debug.LogWarning("Root folder must be inside the Assets folder");
            rootNode = null;
            return;
        }

        // Remove "Assets" and the slash after it
        relativePath = relativePath.Substring("Assets".Length);
        if (relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
        {
            relativePath = relativePath.Substring(1);
        }

        string absPath = Path.Combine(Application.dataPath, relativePath);

        if (!Directory.Exists(absPath))
        {
            Debug.LogWarning("Folder does not exist: " + absPath);
            rootNode = null;
            return;
        }

        rootNode = new Node
        {
            name = Path.GetFileName(rootFolder),
            path = rootFolder,
            isFolder = true
        };

        BuildTreeRecursive(rootNode);
    }

    private void BuildTreeRecursive(Node parent)
    {
        string absPath = Path.Combine(Application.dataPath, parent.path.Substring("Assets".Length).TrimStart('/', '\\'));

        // Add folders
        foreach (var dir in Directory.GetDirectories(absPath))
        {
            string folderName = Path.GetFileName(dir);
            string folderAssetPath = Path.Combine(parent.path, folderName).Replace('\\', '/');

            Node folderNode = new Node
            {
                name = folderName,
                path = folderAssetPath,
                isFolder = true
            };

            BuildTreeRecursive(folderNode);
            parent.children.Add(folderNode);
        }

        // Add scripts (.cs files)
        foreach (var file in Directory.GetFiles(absPath, "*.cs"))
        {
            string fileName = Path.GetFileName(file);
            string fileAssetPath = Path.Combine(parent.path, fileName).Replace('\\', '/');

            Node scriptNode = new Node
            {
                name = fileName,
                path = fileAssetPath,
                isFolder = false
            };

            parent.children.Add(scriptNode);
        }
    }
}