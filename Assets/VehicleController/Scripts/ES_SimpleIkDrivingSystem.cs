using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ES_SimpleIkDrivingSystem : MonoBehaviour
{
    public enum SteerRotationAxis
    {
        XAxis,
        YAxis,
        ZAxis
    }
    public enum Sign
    {
        Negative,
        Positive
    }
    public Sign _sign = Sign.Negative;
    public SteerRotationAxis steerRotation = SteerRotationAxis.ZAxis;
    public bool UpdateAnimInEdit = true;
    public float IKWeight = 1;
    public float LookWeight = .5f;
    public float BodyWeight;
    public float HeadWeight;
    public float Eyeweight;
    public float ClampWeight;
    public Transform LookPos;
    public Transform LeftHandIkTarget, RightHandIKTarget;
    public Animator animator;
    public ESVehicleController vehicleController;
    public float SteerAngle = 25f;
    public Transform Steering;


    // Start is called before the first frame update

    // Update is called once per Animator Late Update
    private void OnAnimatorIK()
    {
        if (animator == null) return;
        if (LeftHandIkTarget == null) return;
        if (RightHandIKTarget == null) return;
        if (LookPos == null) return;
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IKWeight);
        //
        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);

        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, IKWeight);

        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKWeight);
        //
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIkTarget.position);

        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IKWeight);

        //
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIkTarget.rotation);

        //LookWeight
        animator.SetLookAtWeight(LookWeight, BodyWeight, HeadWeight, Eyeweight, ClampWeight);
        animator.SetLookAtPosition(LookPos.position);

    }
    //
    private void Update()
    {
        if (Application.isPlaying)
        {
            Vector3 steervec = Steering.localEulerAngles;
            if (steerRotation == SteerRotationAxis.ZAxis)
            {
                steervec.z = _sign == Sign.Negative ? vehicleController.Steer * -SteerAngle : vehicleController.Steer * SteerAngle;
            }
            else if (steerRotation == SteerRotationAxis.XAxis)
            {
                steervec.x = _sign == Sign.Negative ? vehicleController.Steer * -SteerAngle : vehicleController.Steer * SteerAngle;
            }
            else if (steerRotation == SteerRotationAxis.YAxis)
            {
                steervec.y = _sign == Sign.Negative ? vehicleController.Steer * -SteerAngle : vehicleController.Steer * SteerAngle;
            }
            Steering.localEulerAngles = steervec;
        }
        if (animator != null)
            if (UpdateAnimInEdit)
                animator.Update(0);
    }
}
