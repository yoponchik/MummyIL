using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MummyAgent : Agent
{

    private Transform tr;
    private Rigidbody rb;

    public float moveSpeed = 1.5f;
    public float turnSpeed = 200.0f;

    private Renderer floorRd;
    private StageManager stageManager;
    private Material originMt;

    public Material goodMt, badMt;

    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        stageManager = tr.parent.GetComponent<StageManager>();

        floorRd = tr.parent.Find("Floor").GetComponent<Renderer>();
        originMt = floorRd.material;

        MaxStep = 2000;
    }

    IEnumerator RevertMaterial(Material changedMt) {
        floorRd.material = changedMt;
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }

    public override void OnEpisodeBegin()
    {
        stageManager.InitStage(); //reset stage

        rb.velocity = rb.angularVelocity = Vector3.zero;

        tr.localPosition = new Vector3(0, 0.05f, -5.0f);

        tr.localRotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    //get commands from the Learning from external training (python), or the Academy
    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;
        //Debug.Log($"[0]={action[0]}, [1]={action[1]}");
        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        // Branch 0
        switch (action[0])
        {
            case 1: dir = tr.forward; break;
            case 2: dir = -tr.forward; break;
        }
        // Branch 1
        switch (action[1])
        {
            case 1: rot = -tr.up; break; //turn left
            case 2: rot = tr.up; break;  //turn right
        }

        tr.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rb.AddForce(dir * moveSpeed, ForceMode.VelocityChange);

        // minus penalty
        AddReward(-1 / (float)MaxStep); // 5000 -> 0.005 
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions.Clear();

        // Branch 0 - move (stop/go forward/go backwards) 0, 1, 2 : Size 3
        if (Input.GetKey(KeyCode.W))
        {
            actions[0] = 1; //go forward
        }
        if (Input.GetKey(KeyCode.S))
        {
            actions[0] = 2; //go backwards
        }
        // Branch 1 - È¸Àü (stop/turn left/turn right) 0, 1, 2 : Size 3
        if (Input.GetKey(KeyCode.A))
        {
            actions[1] = 1; //turn left
        }
        if (Input.GetKey(KeyCode.D))
        {
            actions[1] = 2; //turn right
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Floor") return;
        if (other.collider.tag == stageManager.hintColor.ToString())
        {
            SetReward(+1.0f);
            EndEpisode(); //end episode bcuz got the correct block
            StartCoroutine(RevertMaterial(goodMt));
        }
        else {
            if (other.collider.CompareTag("WALL") || other.gameObject.name == "Hint")
            {
                SetReward(-0.05f);
            }
            else {
                SetReward(-1.0f);
                EndEpisode(); //end episode because got the wrong block
                StartCoroutine(RevertMaterial(badMt));
            }
        }

    }

}
