using UnityEngine;

public class Carryable : MonoBehaviour
{
	public float liftForceMultiplier = 1f;

	public float forceHalfDistance = 1f;

	public float damping = 1f;

	public CarryableAiming aiming;

	public float aimSpring = 1000f;

	public float aimTorque = float.PositiveInfinity;

	public bool alwaysForward;

	public float aimAnglePower = 0.5f;

	public float aimDistPower = 1f;

	public bool limitAlignToHorizontal;

	public float handForceMultiplier = 1f;
}
