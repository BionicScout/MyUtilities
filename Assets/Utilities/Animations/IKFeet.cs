using UnityEngine;

[RequireComponent (typeof(Animator))]
public class IKFeet : MonoBehaviour {
    //A reference to the animator
    Animator animator;

    [SerializeField] private Transform leftKneeHint;
    [SerializeField] private Transform rightKneeHint;

    //The vertical distance between the ankle and the sole
    public float ankleOffset = 1f;
    //The length of the raycast cast from the sole
    public float rayLength = 1f;

    //At start we get the animator to make sure it's there
    private void Start() {
        animator = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex) {
        FootIK(AvatarIKGoal.LeftFoot, animator.GetFloat("LeftFootIKWeight"));
        FootIK(AvatarIKGoal.RightFoot, animator.GetFloat("RightFootIKWeight"));

        //animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1f);
        //animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1f);

        //animator.SetIKHintPosition(AvatarIKHint.LeftKnee, GetDynamicHint(AvatarIKGoal.LeftFoot));
        //animator.SetIKHintPosition(AvatarIKHint.RightKnee, GetDynamicHint(AvatarIKGoal.RightFoot));
    }

    private Vector3 GetDynamicHint(AvatarIKGoal goal) {
        Transform upperLeg = (goal == AvatarIKGoal.LeftFoot)
            ? animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg)
            : animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);

        Transform lowerLeg = (goal == AvatarIKGoal.LeftFoot)
            ? animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg)
            : animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);

        Vector3 legDir = lowerLeg.position - upperLeg.position;
        Vector3 bendDir = transform.right; // Use local right for stable direction

        // Mirror the direction for left vs right leg
        float mirror = (goal == AvatarIKGoal.LeftFoot) ? -1f : 1f;

        // Final hint position slightly in front and to the side
        return upperLeg.position + legDir.normalized * 0.6f + bendDir.normalized * 0.1f * mirror;
    }

    private void FootIK(AvatarIKGoal goal, float weight) {
        //We override the animation for this body part, based on the weight passed down to this function
        animator.SetIKPositionWeight(goal, weight);
        animator.SetIKRotationWeight(goal, weight);

        //The ray starts at the ankle, pointing down
        Ray ray = new Ray(animator.GetIKPosition(goal) + Vector3.down*0.1f, Vector3.down);

        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * (ankleOffset + rayLength + 0.1f));

        //If the ray hits something
        if(Physics.Raycast(ray, out hit, ankleOffset + rayLength)) {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.blue);

            //The sole is set to match the hit point
            Vector3 footPos = hit.point;
            footPos.y += ankleOffset;

            //The IK position and rotation are updated
            animator.SetIKPosition(goal, footPos);
            //We maintain the forward vector but make the up vector match the normal of the surface
            animator.SetIKRotation(goal, Quaternion.LookRotation(transform.forward, hit.normal));

        }
    }
}
