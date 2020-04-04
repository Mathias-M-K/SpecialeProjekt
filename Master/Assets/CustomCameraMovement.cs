using System;
using UnityEngine;

public class CustomCameraMovement : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject chicken;

    public float offsetX;
    public float offsetY;


    public float smoothingFactor = 10;
    public float cameraHeight;
    public float cameraDistance;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float xPos = Mathf.Lerp(transform.position.x, chicken.transform.position.x+offsetX, Time.deltaTime * smoothingFactor);
        float zPos = Mathf.Lerp(transform.position.z, chicken.transform.position.z+offsetY, Time.deltaTime * smoothingFactor);

        mainCamera.transform.position = new Vector3(xPos, cameraHeight, zPos);
    }
    


}


