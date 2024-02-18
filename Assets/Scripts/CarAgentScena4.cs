﻿using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class CarAgentScena4 : Agent
{
    private Vector3 carInitialPosition;
    private Quaternion carInitialRotation;
    private List<Vector3> spawnList = new List<Vector3>();
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    //public Transform parkIndicator;

    private void Start()
    {
        carInitialPosition = transform.position;
        carInitialRotation = transform.rotation;

        spawnList.Add(new Vector3(-20.5f, 2.5f, 12f)); // lasciare come primo elemento della lista
        spawnList.Add(new Vector3(4f, 2.5f, 0f));
        spawnList.Add(new Vector3(-46f, 2.5f, -10.5f));
    }

    public override void OnEpisodeBegin()
    {
        transform.position = carInitialPosition;
        transform.rotation = carInitialRotation;

        int index = Random.Range(0, spawnList.Count);
        targetTransform.localPosition = spawnList[index];
        if (index == 0)
        {
            //parkIndicator.position = new Vector3(70f, 0.5f, 108f);
            targetTransform.rotation = Quaternion.Euler(0, 90, 90);
        }
        else if (index == 1)
        {
            //parkIndicator.position = new Vector3(91.5f, 0.5f, 98.2f);
            targetTransform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            //parkIndicator.position = new Vector3(50, 0.5f, 88.1f);
        }
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
