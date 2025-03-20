using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class FoliageGenerator : MonoBehaviour
{
    [SerializeField] private int genWidth;
    [SerializeField] private int genHeight;
}

[CustomEditor(typeof(FoliageGenerator)), CanEditMultipleObjects]
public class FoliageGeneratorEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        return null; //returns a generic container element
    }
}