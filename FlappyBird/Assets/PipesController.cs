using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesController : MonoBehaviour
{
    public float Speed;
    public Transform DespawnPoint;
    public Transform SpawnPoint;

    public float YMin;
    public float YMax;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x,
            transform.position.y + Random.Range(YMin, YMax), transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + Speed * Time.deltaTime, 
            transform.position.y, transform.position.z);

        if(transform.position.x < DespawnPoint.position.x)
        {
            transform.position = new Vector3(SpawnPoint.position.x,
            transform.position.y + Random.Range(YMin, YMax), transform.position.z);
        }
    }
}
