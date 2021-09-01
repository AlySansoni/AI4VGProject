using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class GoSomewhere : MonoBehaviour {
	
	public Transform destination;
	public GameObject gameOverUI;
	
	// Needs of checking water level to set destination again
	[SerializeField] private WaterFlow water;
	private bool underWater = false;
	
	private NavMeshAgent myAgent;
	
	// To check and increase agent velocity during time
	private const float speedIncreaseInterval = 30f; // how often do you want the speed to change
	private const float speedChange = 5; // how much do you want the speed to change
	private const float speedAtStart = 10;
	private float timeSinceStart;
	//private float maxY;

	void Start()
	{
		timeSinceStart = Time.realtimeSinceStartup; //To save time value when the scene is initialized 
		//maxY = Terrain.activeTerrain.SampleHeight(new Vector3(250, 0 ,250));
		myAgent = GetComponent <NavMeshAgent>();
		myAgent.speed = speedAtStart;
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
		{
			//myAgent.SetDestination(new Vector3(250, maxY, 250));
			underWater = true;
		}

		if (Vector3.Distance(destination.transform.position, this.transform.position) <= 1)
		{
			myAgent.speed = 0;
			myAgent.angularSpeed = 0;
			Invoke("GameOver", 1.5f);

		}

		if (myAgent.speed < 25 & myAgent.speed != 0) //speed == 0 when target is reached
		{
			var secondsSinceGameStart = (int) (Time.realtimeSinceStartup - timeSinceStart);
			// how many times has the speed changing interval passed
			var interval = (int) (secondsSinceGameStart / speedIncreaseInterval);

			// current speed = start speed + speed increase for every interval that has passed 
			myAgent.speed = speedAtStart + (interval * speedChange);
		}
	}

	private void GameOver()
	{
		gameOverUI.SetActive(true);
	}
}