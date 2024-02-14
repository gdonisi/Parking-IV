using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class CarAgent : Agent
{
    private Vector3 carInitialPosition;
    private Quaternion carInitialRotation;
    private List<Vector3> spawnList = new List<Vector3>();
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private void Start()
    {
        carInitialPosition = transform.position;
        carInitialRotation = transform.rotation;

        spawnList.Add(new Vector3(-20.5f, 2.5f, 12f)); // lasciare come primo elemento della lista
        spawnList.Add(new Vector3(2.5f, 2.5f, 0.5f));
        spawnList.Add(new Vector3(-45f, 2.5f, -10f));
    }

   
    public override void OnEpisodeBegin()
    {
        //posizione statica dell'auto
        transform.position = carInitialPosition;

        // Posizione casuale di auto e parcheggio
        /* transform.position = new Vector3(Random.Range(carInitialPosition.x -12f, carInitialPosition.x +8f),
            0, Random.Range(carInitialPosition.z -40f,carInitialPosition.z));*/
        transform.rotation = carInitialRotation;

        int index = Random.Range(0, spawnList.Count);
        if (index == 0)
            targetTransform.rotation = Quaternion.Euler(0, 90, 90);
        else
            targetTransform.rotation = Quaternion.Euler(0, 0, 90);
        targetTransform.localPosition = spawnList[index];
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotation = actions.ContinuousActions[0];
        float translation = actions.ContinuousActions[1];

        if (transform.position.y < -4.99)
            EndEpisode();

        if (translation != 0)
        {
            translation *= Time.deltaTime * 20f;
            rotation *= Time.deltaTime * 180f;

            if (translation < 0)
            {
                AddReward(+0.01f);
                rotation *= -1;
            }

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

            float distance = Vector3.Distance(transform.position, targetTransform.position);

            /*if (distance < 45)
                AddReward(0.002f);
            else if (distance < 20)
                AddReward(0.005f); */

            //Debug.Log("distanza: " + distance);
            if (this.StepCount == 999)
                AddReward(-11f);

            else if (this.StepCount == 600)
                AddReward(-5f);
        }
        else
            AddReward(-0.01f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(+3f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Crash>(out Crash crash))
        {
            AddReward(-3f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("carCrash"))
        {
            AddReward(-2f);
            floorMeshRenderer.material = loseMaterial;
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
        //sensor.AddObservation(Vector3.Distance(transform.localPosition, targetTransform.localPosition));
    }

    
}
