using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    private Vector3 initialPosition;
    private Quaternion initialRot;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;


    private void Start()
    {
        initialPosition = transform.position;
        initialRot = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        //posizione statica dell'auto
        //transform.position = initialPosition;

        //Posizione randomica dell'auto
       transform.position = new Vector3(Random.Range(initialPosition.x -12f, initialPosition.x +8f),0, Random.Range(initialPosition.z -40f,initialPosition.z));

        transform.rotation = initialRot;
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
                //SetReward(0.0002f);
                rotation *= -1;
            } 

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

          /*  float distance = Vector3.Distance(transform.position, targetTransform.position);
            if (distance < 20)
                AddReward(0.05f); */

           // Debug.Log("distanza: " + distance);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Crash>(out Crash crash))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
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
    }

   
}
