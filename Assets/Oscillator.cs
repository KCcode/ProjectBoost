using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]  //allow to put only one of this script on a the object

public class Oscillator : MonoBehaviour
{
    //these two will help us notice visually that an oscillator script is in the object
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f; //time it takes to complete a full cycle

    //todo remove from inspector later
    //[Range(0, 1)] [SerializeField] 
    float movementFactor;

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {

        
        if (period <= Mathf.Epsilon) { return; } //protect against period = 0
        float cycles = Time.time / period; //grows continually from 0
        const float tau = Mathf.PI * 2;
        float rawSineWave = Mathf.Sin(cycles * tau);
        movementFactor = (rawSineWave / 2f) + 0.5f; //between [0-1]
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
