using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    private Vector3 initialPosition;
    private Quaternion initialRot;

    private void Start()
    {
        initialPosition = transform.position;
        initialRot = transform.rotation;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float rotation = actions.ContinuousActions[0];
        float translation = actions.ContinuousActions[1];

        if(translation != 0)
        {
            translation *= Time.deltaTime * 20f;
            rotation *= Time.deltaTime * 85f;

            if (translation < 0)
            {
                AddReward(0.0002f);
                rotation *= -1;
            } else
            {
                AddReward(0.0001f);
            }

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(5f);
        }
        if (other.TryGetComponent<Crash>(out Crash crash))
        {
            SetReward(-1f);
        }
        EndEpisode();
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
    }

    public override void OnEpisodeBegin()
    {
        transform.position = initialPosition;
        transform.rotation = initialRot;
    }
}
