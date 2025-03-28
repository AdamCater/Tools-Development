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
        //Create root
        VisualElement root = new VisualElement();

        //Load in UXML from path and attach to the root
        VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UXML/foliageGeneratorEditor.uxml");
        asset.CloneTree(root);

        StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/foliageTool.uss");
        root.styleSheets.Add(sheet);

        return root;

        //return null; //returns a generic container element
    }
}