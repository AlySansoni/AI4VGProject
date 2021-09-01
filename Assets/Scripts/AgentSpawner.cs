using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public class AgentSpawner : MonoBehaviour
{

    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject agent;
    [SerializeField] private GameObject destinationPrefab;
    [SerializeField] private WaterFlow water;
    void Awake()
    {
        Random.InitState((int)DateTime.Now.Ticks);
        Debug.Log("Destination");
        var xZone = Random.Range(1, 3);
        var zZone = Random.Range(1, 3);
        GameObject destination = Instantiate(destinationPrefab, GetRandomPosition(xZone, zZone, true), Quaternion.identity);
        Debug.Log("Agent");
        agent.transform.position = GetRandomPosition(xZone%2+1, zZone%2+1 , false); // Obtains always the opposite corner wrt the destination
        agent.GetComponent<GoSomewhere>().destination = destination.transform;
        
    }
    
    private Vector3 GetRandomPosition(int xZone, int zZone, bool destination, float offSetHeight = 1)
    {
        Vector3 randomPosition;
        bool maxLimit, minLimit;

        Random.InitState((int)DateTime.Now.Ticks);

        Vector3 dim = terrain.terrainData.size;
        Debug.Log("xZone " + xZone + " zZone " + zZone);
        do
        {   
            if (xZone == 1)       
                randomPosition.x = Random.Range(1, dim[0]*0.3f);
            else 
                randomPosition.x = Random.Range(dim[0]*0.6f, dim[0]);
            
            if (zZone == 1)
                randomPosition.z = Random.Range(1, (dim[0])*0.3f);
            else
                randomPosition.z = Random.Range(dim[0]*0.6f, dim[0]);
 
            Vector3 randomVector = new Vector3(randomPosition.x,terrain.transform.position.y, randomPosition.z);
            randomPosition.y = terrain.SampleHeight(randomVector)+offSetHeight;
      
            if (destination)
                minLimit = randomPosition.y <= water.minWaterHeight + 2;
            else
                minLimit = randomPosition.y <= water.minWaterHeight;
            maxLimit = randomPosition.y >= water.maxWaterHeight;
          
        } while ( minLimit || maxLimit);
        
        Debug.Log(randomPosition);
        return randomPosition;
        
    }


}
