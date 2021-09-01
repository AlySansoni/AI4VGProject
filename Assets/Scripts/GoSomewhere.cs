using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoSomewhere : MonoBehaviour {
	
	public Transform destination;
	public GameObject gameOverUI;
	
	// Needs to check water level to set destination again
	[SerializeField] private WaterFlow water;
	private bool underWater = false;
	
	private NavMeshAgent myAgent;
	
	// To check and increase agent velocity in time
	private const float SpeedIncreaseInterval = 30f; // how often do you want the speed to change
	private const float SpeedChange = 5; // how much do you want the speed to change
	private const float SpeedAtStart = 10;
	private float timeSinceStart;
	
	//private float maxY;

	void Start()
	{
		timeSinceStart = Time.realtimeSinceStartup; //To save time value when the scene is initialized 
		//maxY = Terrain.activeTerrain.SampleHeight(new Vector3(250, 0 ,250));
		myAgent = GetComponent <NavMeshAgent>();
		myAgent.speed = SpeedAtStart;
		myAgent.destination = destination.position;
	}

	private void FixedUpdate()
	{
		if (underWater && destination.position.y > water.transform.position.y)
		{
			myAgent.SetDestination(destination.position);
			underWater = false;
		}

		if (!underWater && destination.position.y <= water.transform.position.y)
		{ //an alternative would be to temporarily set as destination the highest point to run from water
			//myAgent.SetDestination(new Vector3(250, maxY, 250));
			underWater = true;
		}

		// Condition to reach the destination
		if (Vector3.Distance(destination.transform.position, this.transform.position) <= 1)
		{
			myAgent.speed = 0;
			myAgent.angularSpeed = 0;
			Invoke(nameof(GameOver), 1.5f);

		}

		if (myAgent.speed < 25 & myAgent.speed != 0) //speed == 0 when target is reached
		{
			var secondsSinceGameStart = (int) (Time.realtimeSinceStartup - timeSinceStart);
			// how many times has the speed changing interval passed
			var interval = (int) (secondsSinceGameStart / SpeedIncreaseInterval);

			// current speed = start speed + speed increase for every interval that has passed 
			myAgent.speed = SpeedAtStart + (interval * SpeedChange);
		}
	}

	private void GameOver()
	{
		gameOverUI.SetActive(true);
	}
}