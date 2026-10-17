using UnityEngine;

// Cosmetic only: turns, leans and animates the character model from the
// Player's Rigidbody velocity. Movement stays in Lab03PlayerController.
public class Lab03CharacterVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody body;
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;

    [Header("Turning")]
    [SerializeField] private float turnSharpness = 12f;
    [SerializeField] private float maxLeanAngle = 8f;

    [Header("Walk Cycle")]
    [SerializeField] private float strideFrequency = 1.6f;
    [SerializeField] private float legSwingAngle = 35f;
    [SerializeField] private float armSwingAngle = 30f;
    [SerializeField] private float bobHeight = 0.05f;
    [SerializeField, Min(0.1f)] private float fullSwingSpeed = 6f;

    [Header("Jump Stretch")]
    [SerializeField] private float stretchPerVelocity = 0.02f;
    [SerializeField] private float maxStretch = 0.1f;

    private Vector3 basePosition;
    private Vector3 baseScale;
    private Quaternion leftArmRest;
    private Quaternion rightArmRest;
    private Quaternion leftLegRest;
    private Quaternion rightLegRest;
    private float facingYaw;
    private float cyclePhase;
    private float stretch;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponentInParent<Rigidbody>();
        }

        basePosition = transform.localPosition;
        baseScale = transform.localScale;
        facingYaw = transform.localEulerAngles.y;
        leftArmRest = GetRestRotation(leftArm);
        rightArmRest = GetRestRotation(rightArm);
        leftLegRest = GetRestRotation(leftLeg);
        rightLegRest = GetRestRotation(rightLeg);
    }

    private void LateUpdate()
    {
        if (body == null)
        {
            return;
        }

        Vector3 velocity = body.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        float moveAmount = Mathf.Clamp01(horizontal.magnitude / fullSwingSpeed);

        UpdateFacing(horizontal, moveAmount);
        UpdateWalkCycle(moveAmount);
        UpdateStretch(velocity.y);
    }

    private void UpdateFacing(Vector3 horizontal, float moveAmount)
    {
        if (horizontal.sqrMagnitude > 0.09f)
        {
            float targetYaw = Mathf.Atan2(horizontal.x, horizontal.z) * Mathf.Rad2Deg;
            facingYaw = Mathf.LerpAngle(facingYaw, targetYaw, 1f - Mathf.Exp(-turnSharpness * Time.deltaTime));
        }

        transform.localRotation =
            Quaternion.Euler(0f, facingYaw, 0f) *
            Quaternion.Euler(maxLeanAngle * moveAmount, 0f, 0f);
    }

    private void UpdateWalkCycle(float moveAmount)
    {
        cyclePhase += Time.deltaTime * strideFrequency * Mathf.PI * 2f * Mathf.Lerp(0.6f, 1.6f, moveAmount);
        float swing = Mathf.Sin(cyclePhase) * moveAmount;

        SetSwing(leftLeg, leftLegRest, swing * legSwingAngle);
        SetSwing(rightLeg, rightLegRest, -swing * legSwingAngle);
        SetSwing(leftArm, leftArmRest, -swing * armSwingAngle);
        SetSwing(rightArm, rightArmRest, swing * armSwingAngle);

        float bob = Mathf.Abs(Mathf.Sin(cyclePhase)) * bobHeight * moveAmount;
        float idleBreath = Mathf.Sin(Time.time * 2f) * 0.01f * (1f - moveAmount);
        transform.localPosition = basePosition + Vector3.up * (bob + idleBreath);
    }

    private void UpdateStretch(float verticalVelocity)
    {
        float targetStretch = Mathf.Clamp(verticalVelocity * stretchPerVelocity, -maxStretch, maxStretch);
        stretch = Mathf.Lerp(stretch, targetStretch, 1f - Mathf.Exp(-10f * Time.deltaTime));

        float height = 1f + stretch;
        float width = 1f / Mathf.Sqrt(height);
        transform.localScale = new Vector3(baseScale.x * width, baseScale.y * height, baseScale.z * width);
    }

    private static Quaternion GetRestRotation(Transform limb)
    {
        return limb != null ? limb.localRotation : Quaternion.identity;
    }

    private static void SetSwing(Transform limb, Quaternion rest, float angle)
    {
        if (limb != null)
        {
            limb.localRotation = rest * Quaternion.Euler(angle, 0f, 0f);
        }
    }
}
