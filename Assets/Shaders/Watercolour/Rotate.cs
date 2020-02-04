using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    float randomX, randomY, randomZ;
    public bool rotate = true;
    // Start is called before the first frame update
    void Start() => RandomInit();

    void RandomInit()
    {
        float Rand() => System.Math.Sign(Random.Range(-1, 1)) * Random.Range(.3f, .7f);
        (randomX, randomY, randomZ) = (Rand(), Rand(), Rand());
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
            transform.Rotate(randomX, randomY, randomZ);
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rotate = !rotate;
            RandomInit();
        }
    }
}
