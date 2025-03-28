using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

public class FoliageGeneratorEditorWindow : EditorWindow
{
    private FoliageDatabase foliageDatabase;
    private DropdownField foliageDropdown;

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
        //Load UMXL
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/foliageTool.uxml");
        if (asset != null )
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

        foliageDropdown = new DropdownField("Selected Foliage");
        foliageDropdown.choices = foliageDatabase.foliageTypes.Select(f => f.foliageName).ToList();
        foliageDropdown.index = 0; //Default Selection
        foliageDropdown.SetEnabled(false); //Disable user Input
        rootVisualElement.Add(foliageDropdown); //Add it to the UI

        //Create a dropdown menu (Dynamically)
        foreach (FoliageType foliage in foliageDatabase.foliageTypes) //Loops through foliage types and finds matching buttons
        {
            Button foliageButton = rootVisualElement.Q<Button>(foliage.foliageName); //Find button by name
            if (foliageButton != null)
            {
                foliageButton.clicked += () => SelectFoliage(foliage.foliageName);
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
            generateButton.clicked += GenerateFoliage;
        }
        else
        {
            Debug.LogError("Button: 'GenerateButton' was not found in the UXML!");
        }
    }

    private void GenerateFoliage()
    {
        string selectedFoliage = foliageDropdown.value;
        FoliageType foliageType = foliageDatabase.foliageTypes.FirstOrDefault(f => f.foliageName == selectedFoliage);

        if (foliageType != null)
        {
            Debug.Log($"Spawning {foliageType.foliageName} with density {foliageType.density}");
            GeneratePrefab(foliageType);
        }
    }

    private void SelectFoliage(string foliageName)
    {
        if (foliageDropdown != null && foliageDropdown.choices.Contains(foliageName))
        {
            foliageDropdown.value = foliageName;
            Debug.Log($"Foliage Type '{foliageName}' selected");
        }
    }

    void GeneratePrefab(FoliageType foliageType)
    {
        string selectedFoliage = foliageDropdown.value;

        if (foliageType != null && foliageType.foliagePrefab != null)
        {
            //Instantiate the prefab
            GameObject spawnedFoliage = Instantiate(foliageType.foliagePrefab, Vector3.zero, Quaternion.identity);
            spawnedFoliage.name = foliageType.foliageName;

            Debug.Log($"Successfully Spawned {foliageType.foliageName}");
        }
        else
        {
            Debug.LogError($"Prefab not found for {selectedFoliage}!");
        }

    }

}
