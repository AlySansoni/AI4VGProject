using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    public float rightLeftSpeed = 0.5f;
    public Vector3 offset;
    private void Start()
    {
        transform.position = target.position + offset;
    }

    private void Update()
    {
        //clockwise or counterclockwise rotation based on the input around y-axes with a certain speed 
        offset = Quaternion.AngleAxis(-Input.GetAxis("Horizontal") * rightLeftSpeed, Vector3.up) * offset;
    }
    


    private void LateUpdate()
    {

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}


