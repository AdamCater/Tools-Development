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

    //References for the Vector3 UI Fields
    private FloatField xField;
    private FloatField yField;
    private FloatField zField;

    //Reference for the Density slider
    private Slider densitySlider;

    //Reference for the Spacing IntegerField
    private IntegerField spacingField;

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
            generateButton.clicked += GenerateFoliageWithPosition;
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

        //Find the Undo button and attach the UndoAction Function
        Button undoPrefabButton = rootVisualElement.Q<Button>("undoButton");

        if (undoPrefabButton != null)
        {
            undoPrefabButton.clicked += foliageGenerator.UndoAction;
        }
        else
        {
            Debug.Log($"Button: 'Undo' was not found in the UXML");
        }

        //Add UI references for X, Y, Z positions
        xField = rootVisualElement.Q<FloatField>("unity-x-input");
        yField = rootVisualElement.Q<FloatField>("unity-y-input");
        zField = rootVisualElement.Q<FloatField>("unity-z-input");

        //Add Slider References for Density slider
        densitySlider = rootVisualElement.Q<Slider>("densitySlider");

        //Add Spacing Referneces for Spacing IntegerField
        spacingField = rootVisualElement.Q<IntegerField>("spacingField");
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

    private void GenerateFoliageWithPosition()
    {
        //Get Selected Foliage
        string selectedFoliage = foliageDropdown.value;
        FoliageType foliageType = foliageDatabase.foliageTypes.FirstOrDefault(f => f.foliageName == selectedFoliage);


        if (foliageType != null)
        {
            //Get the spawn position from the UI fields
            float x = xField.value;
            float y = yField.value;
            float z = zField.value;

            //Create a Vector3 for the spawn location
            Vector3 spawnPosition = new Vector3(x, y, z);

            //Get the Spawn Density from the slider
            float spawnDensity = densitySlider.value;

            //Get the Spacing from the IntegerField
            int spacing = spacingField.value;

            //Call the GenerateFoliage method with the specified position and density
            foliageGenerator.GenerateFoliage(spawnPosition, spawnDensity, spacing);
        }

    }
}

