using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEffect : MonoBehaviour
{
    public GameObject rainParticlePrefab;
    private GameObject rainParticleInstance;

    void Start()
    {
        CreateRainEffect();
    }

    void CreateRainEffect()
    {
        Vector3 rainPosition = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        rainParticleInstance = Instantiate(rainParticlePrefab, rainPosition, Quaternion.identity);
        rainParticleInstance.transform.SetParent(transform);
    }

    void Update()
    {
        if (rainParticleInstance != null)
        {
            rainParticleInstance.transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        }
    }
}
