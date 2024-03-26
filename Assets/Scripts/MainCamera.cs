using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Riferimento all'oggetto auto
    public GameObject auto;
    // Distanza tra la camera e l'auto
    public Vector3 offset;

    void Start()
    {
        offset = new Vector3(0, 25, -25);
    }

    void LateUpdate()
    {
        // Imposta la posizione della camera in base alla posizione dell'auto
        transform.position = auto.transform.position + offset;
        // Fa in modo che la camera guardi verso l'auto
        transform.LookAt(auto.transform);
        // Ruota la camera lungo l'asse Y in base all'angolo di rotazione dell'auto
        transform.RotateAround(auto.transform.position, Vector3.up,
            auto.transform.rotation.eulerAngles.y);
    }
}
