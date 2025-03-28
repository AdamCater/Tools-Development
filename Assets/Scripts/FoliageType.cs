using UnityEngine;

[CreateAssetMenu(fileName = "NewFoliageType", menuName = "Foliage/Foliage Type")]
public class FoliageType : ScriptableObject
{
    public string foliageName;
    public GameObject foliagePrefab;
    public float density;
    public float minScale = 1.0f;
    public float maxScale = 2.0f;
    public Color colourVariation = Color.green;
}
