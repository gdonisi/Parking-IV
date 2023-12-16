using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float movingSpeed;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        movingSpeed = 20f;
        rotationSpeed = 85f;
    }

    // Update is called once per frame
    void Update()
    {
        // pressione dei tasti W, S, o frecce avanti e dietro
        float translation = Input.GetAxis("Vertical") * movingSpeed;
        // pressione dei tasti A, D, o frecce destra e sinistra
        float rotation = 0f;

        // Evita la rotazione se l'auto sta ferma
        if (translation != 0)
        {
            rotation = Input.GetAxis("Horizontal") * rotationSpeed;

            // Se si sta facendo retromarcia, inverti i tasti A e D
            if (translation < 0)
            {
                rotation *= -1;
            }
        }

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
