using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed; // 6
    public Vector3 camOffset; // 0, 19, -16

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + camOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        // transform.LookAt(target); // I don't like cause it jitters the player
    }
}
