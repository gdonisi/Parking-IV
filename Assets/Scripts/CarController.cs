using UnityEngine;

public class CarController : MonoBehaviour
{
    public float movingSpeed;
    public float rotationSpeed;
    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        movingSpeed = 20f;
        rotationSpeed = 85f;

        originalPos = gameObject.transform.position;
        originalRot = gameObject.transform.rotation;
    }

    void Update()
    {
        /* Boh
        // pressione dei tasti W, S, o frecce avanti e dietro
        float translation = Input.GetAxis("Vertical") * movingSpeed;
        float rotation = 0f;

        // Evita la rotazione se l'auto sta ferma
        if (translation != 0)
        {
            // pressione dei tasti A, D, o frecce sinistra e destra
            rotation = Input.GetAxis("Horizontal") * rotationSpeed;

            // Se si sta facendo retromarcia, inverti i tasti A e D
            if (translation < 0)
            {
                rotation *= -1;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
        }

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.position = originalPos;
        transform.rotation = originalRot;
    }
}
