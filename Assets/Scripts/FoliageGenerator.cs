using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class FoliageGenerator
{
    private FoliageDatabase foliageDatabase;
    private DropdownField foliageDropdown;

    //Assign Dropdown and Database
    public void FoliageDropDown(DropdownField _dropDown)
    {
        foliageDropdown = _dropDown;
    }
    public void FoliageDataBase(FoliageDatabase _dataBase)
    {
        foliageDatabase = _dataBase;
    }
    public void GenerateFoliage()
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
            Debug.Log($"Spawning {foliageType.foliageName} with density {foliageType.density}");
            GeneratePrefab(foliageType);
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

    public void GeneratePrefab(FoliageType foliageType)
    {
        string selectedFoliage = foliageDropdown.value;

        if (foliageType != null && foliageType.foliagePrefab != null)
        {
            //Instantiate the prefab
            GameObject spawnedFoliage = GameObject.Instantiate(foliageType.foliagePrefab, Vector3.zero, Quaternion.identity);
            spawnedFoliage.name = foliageType.foliageName;

            Debug.Log($"Successfully Spawned {foliageType.foliageName}");
        }
        else
        {
            Debug.LogError($"Prefab not found for {selectedFoliage}!");
        }

    }
}