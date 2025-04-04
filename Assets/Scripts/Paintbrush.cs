using UnityEngine;

public class Paintbrush : GridBrushBase
{
    [Header("Paintbrush Settings")]
    [SerializeField] public GameObject brushPrefab;
    [SerializeField] public float brushSize = 10.0f;
    [SerializeField] public float spawnInterval = 1f; //Time between spawning objects
    private float lastSpawnedTime;

    void MouseBrush()
    {
        //Get Mouse position
        Vector3 brushPosition = Input.mousePosition;

        //Convert screen coordinates to world coordinates
        Camera.main.ScreenPointToRay(brushPosition);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > lastSpawnedTime + spawnInterval)
        {
            // Get mouse position in world coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Spawn the brush prefab
                GameObject brush = Instantiate(brushPrefab, hit.point, Quaternion.identity);
                brush.transform.localScale = Vector3.one * brushSize; // Adjust scale
                lastSpawnedTime = Time.time;
            }
        }
    }
}
