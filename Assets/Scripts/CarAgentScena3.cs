using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class CarAgentScena3 : Agent
{
    private Vector3 carInitialPosition;
    private Quaternion carInitialRotation;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    public bool randomPos = false;

    private void Start()
    {
        carInitialPosition = transform.position;
        carInitialRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        if (randomPos)
            transform.position = new Vector3(Random.Range(carInitialPosition.x - 5f, carInitialPosition.x + 7f),
            0, Random.Range(carInitialPosition.z - 30f, carInitialPosition.z));
        else
        transform.position = carInitialPosition;
        transform.rotation = carInitialRotation;
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
                //AddReward(+0.05f);
                rotation *= -1;
            } else
            {
                //translation = 0;
                //rotation = 0;
                AddReward(-0.2f);
            }

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

            /*
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            if (distance < 45)
                AddReward(0.002f);
            else if (distance < 20)
                AddReward(0.005f);

            Debug.Log("distanza: " + distance);
            */

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

        if (other.TryGetComponent<Goal>(out Goal goal))
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
