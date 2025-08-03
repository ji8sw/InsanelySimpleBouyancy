using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    public float DampingMultiplier = 10.0f;
    public float VelocityDividerWhenEntering = 2.0f;
    public Vector3 WaterAntiGravity = new Vector3(0, 2f, 0);

    public Dictionary<Component, Rigidbody> ContainedRBs = new Dictionary<Component, Rigidbody>();
    public Dictionary<Component, Camera> ContainedCams = new Dictionary<Component, Camera>();

    public void OnEnterWater(Rigidbody RB)
    {
        RB.angularDamping *= DampingMultiplier;
        RB.linearDamping *= DampingMultiplier;
        RB.linearVelocity /= VelocityDividerWhenEntering;
    }

    public void OnExitWater(Rigidbody RB)
    {
        RB.angularDamping /= DampingMultiplier;
        RB.linearDamping /= DampingMultiplier;
    }

    private void OnTriggerEnter(Collider Other)
    {
        var RB = Other.transform.GetComponent<Rigidbody>();
        if (RB)
        { // an object has been dropped in the water, we should apply physics changes
            if (ContainedRBs.ContainsKey(RB)) return;
            OnEnterWater(RB);
            ContainedRBs.Add(RB, RB);

            return;
        }

        var Cam = Other.transform.GetComponent<Camera>();
        if (Cam)
        { // its a camera under the water, so we should apply underwater effects to it
            if (ContainedCams.ContainsKey(Cam)) return;

            ContainedCams.Add(Cam, Cam);
            return;
        }
    }

    private void FixedUpdate()
    {
        foreach (var Data in ContainedRBs)
        {
            var RB = Data.Value;
            RB.AddForce(WaterAntiGravity * RB.mass);
        }

        foreach (var Data in ContainedCams)
        {
            
        }
    }

    private void OnTriggerExit(Collider Other)
    {
        var RB = Other.transform.GetComponent<Rigidbody>();
        if (RB)
        { // an object has left the water, we should revert physics changes
            if (!ContainedRBs.ContainsKey(RB)) return;
            OnExitWater(RB);
            ContainedRBs.Remove(RB);
            return;
        }

        var Cam = Other.transform.GetComponent<Camera>();
        if (Cam)
        { // its a camera exiting the water, so we should revert underwater effects
            if (!ContainedCams.ContainsKey(Cam)) return;

            ContainedCams.Remove(Cam);
            return;
        }
    }
}