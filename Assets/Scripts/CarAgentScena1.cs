using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgentScena1 : Agent
{
    private Vector3 carInitialPosition;
    private Quaternion carInitialRotation;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    public bool randomPos = false;
    public Animator policeAnimation;
    public AudioSource policeAudio;
    public AudioSource exp;
    public ParticleSystem explosionEffect;

    private void Start()
    {
        carInitialPosition = transform.position;
        carInitialRotation = transform.rotation;

        policeAnimation.enabled = false;
        policeAudio.enabled = false;
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
            rotation *= Time.deltaTime * 80f;

            if (translation < 0)
            {
                AddReward(+0.01f);
                rotation *= -1;
            }

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

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
            policeAnimation.enabled = false;
            policeAudio.enabled = false;
            EndEpisode();
        }
        if (other.TryGetComponent<Crash>(out Crash crash))
        {
            AddReward(-3f);
            policeAnimation.enabled = true;
            policeAudio.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("carCrash"))
        {
            explosionEffect.Play();
            exp.Play();
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
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
