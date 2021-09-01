using UnityEngine;
 
public class WaterFlow : MonoBehaviour 
{ 
    [SerializeField] private Terrain terrain; 
    [SerializeField] private Rigidbody water;
    public Vector3 velocity = Vector3.up; 
    private bool growing = true; 
    private float currentHeight; 
    public float maxWaterHeight; 
    public float minWaterHeight; 

    private void Reset()
    {
        terrain = FindObjectOfType<Terrain>();
        water = GetComponent<Rigidbody>();

        if (terrain == null) return;
        velocity.y = 1.2f;
        var terrainHeight = terrain.terrainData.heightmapScale.y; 
        maxWaterHeight = terrainHeight * 0.8f; 
        minWaterHeight = terrainHeight * 0.2f;
    }

    private void Start() 
    {
        water.position = new Vector3(250, 3, 250);
    }
        
    void FixedUpdate()
    {
        currentHeight = transform.position.y;
        if (currentHeight >= maxWaterHeight)
            growing = false;
        if (currentHeight <= minWaterHeight)
            growing = true;
        if (growing)
        {
            water.position += velocity * Time.deltaTime;
        }
        else
        {
            water.position -= velocity * Time.deltaTime;
        }
        
    }
}