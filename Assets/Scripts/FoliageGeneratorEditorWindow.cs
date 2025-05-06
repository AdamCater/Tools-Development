using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

public class FoliageGeneratorEditorWindow : EditorWindow
{
    private FoliageDatabase foliageDatabase;
    private DropdownField foliageDropdown;
    private FoliageGenerator foliageGenerator;
    private VisualElement previewElement;

    [MenuItem("Tools/Foliage Generator")]
    public static void ShowWindow()
    {
        FoliageGeneratorEditorWindow window = GetWindow<FoliageGeneratorEditorWindow>();
        window.titleContent = new GUIContent("Foliage Generator at your Disposal!"); //Sets the title of the Tool

        //Set default window size
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(500, 600);

        window.Show(); //opens in last docked position instead of being a floating window
    }

    private void CreateGUI()
    {
        foliageGenerator = new FoliageGenerator();

        //Load UMXL
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/foliageTool.uxml");
        if (asset != null)
        {
            asset.CloneTree(rootVisualElement);
        }
        else
        {
            Debug.LogError("UXML File not found: Assets/UXML/foliageGeneratorEditor.uxml");
        }

        //Load and apply stylesheet
        StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/foliageTool.uss");
        if (sheet != null)
        {
            rootVisualElement.styleSheets.Add(sheet);
        }
        else
        {
            Debug.LogError("USS File not found: Assets/UI/foliageTool.uss");
        }

        //Load the foliage database
        foliageDatabase = AssetDatabase.LoadAssetAtPath<FoliageDatabase>("Assets/Prefabs/Data/FoliageDatabase.asset");
        if (foliageDatabase == null)
        {
            Debug.LogError("Foliage Database not found, make sure it is in the correct file location: 'Assets/Prefabs/Data/FoliageDatabase.asset'");
            return;
        }

        foliageGenerator.FoliageDataBase(foliageDatabase);

        foliageDropdown = new DropdownField("Selected Foliage");
        foliageDropdown.choices = foliageDatabase.foliageTypes.Select(f => f.foliageName).ToList();
        foliageDropdown.index = 0; //Default Selection
        foliageDropdown.SetEnabled(false); //Disable user Input
        rootVisualElement.Add(foliageDropdown); //Add it to the UI

        foliageGenerator.FoliageDropDown(foliageDropdown);

        //Setup preview Image
        previewElement = rootVisualElement.Q<VisualElement>("previewContainer");
        UpdatePrefabPreview();

        if (previewElement == null)
        {
            Debug.LogError("Could not find 'previewContainer' VisualElement in UXML!");
        }

        foliageDropdown.RegisterValueChangedCallback(evt =>
        {
            UpdatePrefabPreview();
        });

        //Create a dropdown menu (Dynamically)
        foreach (FoliageType foliage in foliageDatabase.foliageTypes) //Loops through foliage types and finds matching buttons
        {
            Button foliageButton = rootVisualElement.Q<Button>(foliage.foliageName); //Find button by name
            if (foliageButton != null)
            {
                foliageButton.clicked += () => foliageGenerator.SelectFoliage(foliage.foliageName);
                UpdatePrefabPreview();
            }
            else
            {
                Debug.LogWarning($"No Button found for Foliage Type: {foliage.foliageName}");
            }

        }

        //Find and Generate button and attach the GenerateFoliage Function
        Button generateButton = rootVisualElement.Q<Button>("generateButton");

        if (generateButton != null)
        {
            generateButton.clicked += foliageGenerator.GenerateFoliage;
        }
        else
        {
            Debug.LogError("Button: 'GenerateButton' was not found in the UXML!");
        }

        //Find and generate button and attach the Paintbrush Function
        Button paintbrushButton = rootVisualElement.Q<Button>("Paintbrush");

        if (paintbrushButton != null)
        {
            paintbrushButton.clicked += foliageGenerator.Paintbrush;
        }
        else
        {
            Debug.Log("Button: 'Paintbrush' was not found in the UXML");
        }
    }

    private void UpdatePrefabPreview()
    {
        string selectedFoliage = foliageDropdown.value;
        FoliageType foliageType = foliageDatabase.foliageTypes.FirstOrDefault(f => f.foliageName == selectedFoliage);

        if (foliageType != null && foliageType.foliagePrefab != null)
        {
            Texture2D previewTexture = AssetPreview.GetAssetPreview(foliageType.foliagePrefab);

            if (previewTexture != null)
            {
                previewElement.style.backgroundImage = new StyleBackground(previewTexture);
            }
            else
            {
                Debug.LogWarning($"No preview available for: {foliageType.foliageName}");
                previewElement.style.backgroundImage = null;
            }
        }
        else
        {
            previewElement.style.backgroundImage = null;
        }
    }
}
