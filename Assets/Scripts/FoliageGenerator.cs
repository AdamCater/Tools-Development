using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UIElements;

public class FoliageGenerator
{
    private FoliageDatabase foliageDatabase;
    private DropdownField foliageDropdown;
    private Paintbrush foliagePaintbrush;
    private List<GameObject> spawnedFoliageList = new List<GameObject>(); //Creates a list to keep track of spawned objects

    //Assign Dropdown and Database
    public void FoliageDropDown(DropdownField _dropDown)
    {
        foliageDropdown = _dropDown;
    }
    public void FoliageDataBase(FoliageDatabase _dataBase)
    {
        foliageDatabase = _dataBase;
    }
    public void GenerateFoliage(Vector3 spawnPosition, float spawnDensity, int spacing)
    {
        if (foliageDatabase == null)
        {
            Debug.LogError("foliageDatabase is null, make sure it's assigned");
        }
        if (foliageDropdown == null)
        {
            Debug.LogError("foliageDropdown is null, make sure it's assigned");
        }

        string selectedFoliage = foliageDropdown.value;
        FoliageType foliageType = foliageDatabase.foliageTypes.FirstOrDefault(f => f.foliageName == selectedFoliage);

        if (foliageType != null)
        {
            Debug.Log($"Spawning {foliageType.foliageName} with density {spawnDensity}");
            GeneratePrefab(foliageType, spawnPosition, spawnDensity, spacing);
        }

    }

    public void SelectFoliage(string foliageName)
    {
        if (foliageDropdown != null && foliageDropdown.choices.Contains(foliageName))
        {
            foliageDropdown.value = foliageName;
            Debug.Log($"Foliage Type '{foliageName}' selected");
        }
    }

    public void GeneratePrefab(FoliageType foliageType, Vector3 spawnPosition, float spawnDensity, int spacing)
    {
        string selectedFoliage = foliageDropdown.value;

        // Calculate how many prefabs to spawn based on density
        int numberOfFoliage = Mathf.RoundToInt(foliageType.density * spawnDensity);

        for (int i = 0; i < numberOfFoliage; i++)
        {
            // Calculate random offset for each spawned object
            float offsetX = Random.Range(-spacing, spacing); // Random offset on the X axis 
            float offsetZ = Random.Range(-spacing, spacing); // Random offset on the Z axis

            // Apply the offset to the spawn position
            Vector3 newSpawnPosition = spawnPosition + new Vector3(offsetX, 0 , offsetZ);

            // Instantiate the prefab
            GameObject spawnedFoliage = GameObject.Instantiate(foliageType.foliagePrefab, newSpawnPosition, Quaternion.identity);
            spawnedFoliage.name = foliageType.foliageName;

            // Add the spawned prefab to the list
            spawnedFoliageList.Add(spawnedFoliage);

            Debug.Log($"Successfully Spawned {foliageType.foliageName} at {newSpawnPosition}");
        }


    }

    public void UndoAction()
    {
        // Destroy all spawned foliage objects
        if (spawnedFoliageList.Count == 0)
        {
            Debug.Log("No spawned prefabs to clear.");
            return;
        }

        foreach (var spawnedFoliage in spawnedFoliageList)
        {
            if (spawnedFoliage != null)
            {
                //Destroy the Prefab
                UnityEngine.Object.DestroyImmediate(spawnedFoliage);

                Debug.Log($"Destroyed {spawnedFoliage.name} from scene.");
            }
            else
            {
                Debug.LogWarning("Found a null reference in the list of spawned prefabs.");
            }
        }

        // Clear the list after destruction
        spawnedFoliageList.Clear();
        Debug.Log("Cleared all spawned prefabs.");
    }

    public void Paintbrush()
    {
        Debug.Log("Paintbrush button clicked");

        if (foliageDropdown != null && foliageDropdown.value == "Tree")
        {
            Debug.Log($"You cannot use the Paintbrush whilst Prefab: {foliageDropdown.value} is selected!");
        }
        else if (foliageDropdown != null && foliageDropdown.value == "Grass")
        {
            Debug.Log($"You have used the Paintbrush for Prefab: {foliageDropdown.value}");
            foliagePaintbrush.brushPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
        }
    }
}