using Multiplayer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController3 : MonoBehaviour
{
	public CameraMode mode;

	public LayerMask wallLayers;

	public Vector3 farTargetOffset;

	public float farRange;

	public float farRangeAction;

	public Vector3 closeTargetOffset;

	public float closeRange;

	public float closeRangeLookUp;

	public Vector3 fpsTargetOffset;

	private float nearClip = 0.05f;

	public bool ignoreWalls;

	public Human human;

	public Camera gameCam;

	public WaterSensor waterSensor;

	private Ragdoll ragdoll;

	public static float fovAdjust;

	public static float smoothingAmount = 0.5f;

	private List<float> offsetSmoothingFast = new List<float>();

	private List<float> offsetSmoothingSlow = new List<float>();

	private float wallHold = 1000f;

	private float wallHoldTime;

	[NonSerialized]
	public float offset = 4f;

	private Vector3 oldTarget;

	private Vector3 smoothTarget;

	private int oldFrame;

	private Vector3 fixedupdateSmooth;

	private float offsetSpeed;

	public float headPivotAhead;

	private float pitchRange = 30f;

	private float oldPitchSign;

	private float pitchExpandTimer;

	private float cameraPitchAngle;

	private float standPhase;

	private float holdPhase;

	private Vector3[] rayStarts = new Vector3[4];

	private float cameraTransitionSpeed = 1f;

	private float cameraTransitionPhase;

	private float startFov;

	private Vector3 startOffset;

	private Quaternion startRotation = Quaternion.identity;

	private Vector3 startHumanCameraPos;

	[CompilerGenerated]
	private static Action<string> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<string> _003C_003Ef__mg_0024cache1;

	private void Start()
	{
		ragdoll = human.ragdoll;
		waterSensor = base.gameObject.AddComponent<WaterSensor>();
		Shell.RegisterCommand("smooth", OnCameraSmooth, "smooth <smoothing>\r\nSet camera smoothing amount (0-none, 0.5-default, 1-max)");
		Shell.RegisterCommand("fov", OnFOV, "fov <fov_adjust>\r\nSet FOV adjust (-10 - narrow, 0 - default, 30 - wide)");
		Shell.RegisterCommand("hdr", OnHDR, "hdr\r\nToggle high dynamic range");
	}

	private void OnHDR(string param)
	{
		if (!string.IsNullOrEmpty(param))
		{
			param = param.ToLowerInvariant();
			if ("off".Equals(param))
			{
				Options.advancedVideoHDR = 0;
			}
			else if ("on".Equals(param))
			{
				Options.advancedVideoHDR = 1;
			}
		}
		else
		{
			Options.advancedVideoHDR = ((Options.advancedVideoHDR <= 0) ? 1 : 0);
		}
		Options.ApplyAdvancedVideo();
		Shell.Print("HDR " + ((Options.advancedVideoHDR <= 0) ? "off" : "on"));
	}

	private static void OnCameraSmooth(string param)
	{
		if (!string.IsNullOrEmpty(param))
		{
			param = param.ToLowerInvariant();
			if (float.TryParse(param, out float result))
			{
				Options.cameraSmoothing = Mathf.Clamp((int)(result * 20f), 0, 40);
			}
			else if ("off".Equals(param))
			{
				Options.cameraSmoothing = 0;
			}
			else if ("on".Equals(param))
			{
				Options.cameraSmoothing = 20;
			}
		}
		Shell.Print("Camera smoothing " + smoothingAmount);
	}

	private static void OnFOV(string param)
	{
		if (!string.IsNullOrEmpty(param) && float.TryParse(param, out float result))
		{
			Options.cameraFov = Mathf.Clamp((int)(result / 2f + 5f), 0, 20);
		}
		Shell.Print("FOV adjust " + fovAdjust);
	}

	public void Scroll(Vector3 offset)
	{
		oldTarget += offset;
		smoothTarget += offset;
		base.transform.position += offset;
		fixedupdateSmooth += offset;
	}

	private Vector3 SmoothCamera(Vector3 target, float deltaTime)
	{
		if (ReplayRecorder.isPlaying)
		{
			deltaTime = (float)Mathf.Abs(ReplayRecorder.instance.currentFrame - oldFrame) * Time.fixedDeltaTime;
			oldFrame = ReplayRecorder.instance.currentFrame;
			if (deltaTime == 0f)
			{
				return smoothTarget;
			}
		}
		if (NetGame.isClient)
		{
			deltaTime = (float)Mathf.Abs(human.player.renderedFrame - oldFrame) * Time.fixedDeltaTime;
			oldFrame = human.player.renderedFrame;
			if (deltaTime == 0f)
			{
				return smoothTarget;
			}
		}
		int num = Mathf.RoundToInt(deltaTime / (Time.fixedDeltaTime / 10f));
		if (num < 1)
		{
			num = 1;
		}
		if (smoothingAmount == 0f || deltaTime == 0f || num > 1000 || (target - oldTarget).magnitude > 10f)
		{
			deltaTime = 0f;
			smoothTarget = (oldTarget = target);
			return target;
		}
		float num2 = deltaTime / (float)num;
		float num3 = offset * 0.1f * smoothingAmount;
		Vector3 a = (target - oldTarget) / deltaTime;
		for (int i = 0; i < num; i++)
		{
			oldTarget += a * num2;
			Vector3 vector = oldTarget + Vector3.ClampMagnitude(smoothTarget - oldTarget, num3);
			float num4 = Mathf.SmoothStep(0.05f, 2f, (vector - oldTarget).magnitude / num3);
			smoothTarget = Vector3.MoveTowards(vector, oldTarget, num2 * num4);
		}
		deltaTime = 0f;
		return smoothTarget;
	}

	public void PostSimulate()
	{
		if (base.enabled && !NetGame.isClient && !ReplayRecorder.isPlaying)
		{
			fixedupdateSmooth = SmoothCamera(ragdoll.partHead.transform.position, Time.fixedDeltaTime);
		}
	}

	public void LateUpdate()
	{
		bool flag = !NetGame.isClient && !ReplayRecorder.isPlaying;
		Vector3 targetOffset;
		float yaw;
		float pitch;
		float minDist;
		float fov;
		float camDist;
		float num;
		switch (mode)
		{
		case CameraMode.Far:
			CalculateFarCam(out targetOffset, out yaw, out pitch, out camDist, out minDist, out fov, out num);
			break;
		case CameraMode.Close:
			CalculateCloseCam(out targetOffset, out yaw, out pitch, out camDist, out minDist, out fov, out num);
			break;
		case CameraMode.FirstPerson:
			CalculateFirstPersonCam(out targetOffset, out yaw, out pitch, out camDist, out minDist, out fov, out num);
			break;
		default:
			CalculateCloseCam(out targetOffset, out yaw, out pitch, out camDist, out minDist, out fov, out num);
			break;
		}
		if (fovAdjust != 0f)
		{
			camDist *= Mathf.Tan((float)Math.PI / 180f * fov / 2f) / Mathf.Tan((float)Math.PI / 180f * (fov + fovAdjust) / 2f);
			fov += fovAdjust;
		}
		camDist /= MenuCameraEffects.instance.cameraZoom;
		camDist = Mathf.Max(camDist, minDist);
		if (MenuCameraEffects.instance.creditsAdjust > 0f)
		{
			pitch = Mathf.Lerp(pitch, 90f, MenuCameraEffects.instance.creditsAdjust * 0.7f);
			camDist += MenuCameraEffects.instance.creditsAdjust * 20f;
			fov = Mathf.Lerp(fov, 40f, MenuCameraEffects.instance.creditsAdjust);
		}
		Quaternion quaternion = Quaternion.Euler(pitch, yaw, 0f);
		Vector3 vector = quaternion * Vector3.forward;
		Vector3 vector2 = ((!flag) ? SmoothCamera(ragdoll.partHead.transform.position, Time.unscaledDeltaTime) : fixedupdateSmooth) + targetOffset;
		num *= Mathf.Clamp(gameCam.transform.position.magnitude / 500f, 1f, 2f);
		gameCam.nearClipPlane = num;
		gameCam.fieldOfView = fov;
		float num2 = (!ignoreWalls) ? CompensateForWallsNearPlane(vector2, quaternion, farRange * 1.2f, minDist) : 10000f;
		offset = SpringArm(offset, camDist, num2, Time.unscaledDeltaTime);
		if (num2 < offset && !Physics.SphereCast(vector2 - vector * offset, num * 2f, vector, out RaycastHit _, offset - num2, wallLayers, QueryTriggerInteraction.Ignore))
		{
			offset = num2;
			offsetSpeed = 0f;
		}
		ApplyCamera(vector2, vector2 - vector * offset, quaternion, fov);
	}

	private float SpringArm(float current, float target, float limit, float deltaTime)
	{
		int num = Mathf.RoundToInt(deltaTime / (Time.fixedDeltaTime / 10f));
		if (num < 1)
		{
			num = 1;
		}
		if (deltaTime == 0f || num > 1000)
		{
			return target;
		}
		float stepTime = deltaTime / (float)num;
		if (limit < target)
		{
			target = limit;
			if (target < current)
			{
				offsetSpeed = 0f;
				IntegrateDirect(target, ref current, stepTime, num, 5f, 10f);
				return current;
			}
		}
		if (target < current)
		{
			IntegrateSpring(target, ref current, ref offsetSpeed, stepTime, num, 100f, 10f, 500f);
		}
		else
		{
			IntegrateSpring(target, ref current, ref offsetSpeed, stepTime, num, 2f, 1f, 6f);
		}
		return current;
	}

	public static void SetSmoothing(float v)
	{
		smoothingAmount = v;
	}

	public static void SetFov(float v)
	{
		fovAdjust = (v * 20f - 5f) * 2f;
	}

	private void IntegrateDirect(float target, ref float pos, float stepTime, int steps, float minSpeed, float spring)
	{
		for (int i = 0; i < steps; i++)
		{
			pos = Mathf.MoveTowards(pos, target, (minSpeed + Mathf.Abs(spring * (target - pos))) * stepTime);
		}
	}

	private void IntegrateSpring(float target, ref float pos, ref float speed, float stepTime, int steps, float spring, float damper, float maxForce)
	{
		for (int i = 0; i < steps; i++)
		{
			if (speed * (target - pos) <= 0f)
			{
				speed = 0f;
			}
			speed += Mathf.Clamp(spring * (target - pos), 0f - maxForce, maxForce) * stepTime;
			speed = Mathf.MoveTowards(speed, 0f, Mathf.Abs(speed * damper * stepTime));
			pos = Mathf.MoveTowards(pos, target, Mathf.Abs(speed * stepTime));
		}
	}

	private void CalculateFarCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
	{
		fov = 70f;
		nearClip = this.nearClip;
		yaw = human.controls.cameraYawAngle;
		cameraPitchAngle = human.controls.cameraPitchAngle;
		float num = 0.5f + 0.5f * Mathf.InverseLerp(0f, 80f, Mathf.Abs(cameraPitchAngle));
		if (human.controls.holding)
		{
			holdPhase = Mathf.MoveTowards(holdPhase, 1f, (human.state != HumanState.Climb) ? (Time.fixedDeltaTime * Mathf.InverseLerp(0.5f, 0f, human.controls.walkSpeed)) : 0f);
		}
		else
		{
			holdPhase = Mathf.MoveTowards(holdPhase, 0f, Time.fixedDeltaTime / 0.5f);
		}
		num *= Mathf.Lerp(Mathf.InverseLerp(0.5f, 0f, human.controls.walkSpeed), 1f, holdPhase);
		standPhase = Mathf.MoveTowards(standPhase, num, Time.fixedDeltaTime / 1f);
		pitchRange = Mathf.Lerp(30f, 60f, standPhase);
		int num2 = -50;
		int num3 = 80;
		float b = cameraPitchAngle / 80f * 60f + 10f;
		float a = cameraPitchAngle / 80f * 20f + 10f;
		pitch = Mathf.Lerp(a, b, standPhase);
		Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
		targetOffset = rotation * farTargetOffset;
		if (human.controls.leftGrab || human.controls.rightGrab)
		{
			camDist = Options.LogMap(pitch, num2, 0f, num3, farRangeAction * 0.6f, farRangeAction, (farRange + farRangeAction) / 2f);
		}
		else
		{
			camDist = Options.LogMap(pitch, num2, 0f, num3, farRangeAction * 0.6f, farRangeAction, farRange);
		}
		fov = Mathf.Lerp(70f, 80f, Mathf.InverseLerp(0f, num2, pitch));
		minDist = 0.025f;
	}

	private void CalculateCloseCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
	{
		fov = 70f;
		nearClip = this.nearClip;
		yaw = human.controls.cameraYawAngle;
		pitch = human.controls.cameraPitchAngle + 10f;
		if (pitch < 0f)
		{
			pitch *= 0.8f;
		}
		Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
		targetOffset = rotation * closeTargetOffset;
		Vector3 vector = Quaternion.Euler(human.controls.cameraPitchAngle, human.controls.cameraYawAngle, 0f) * Vector3.forward;
		camDist = Mathf.Lerp(closeRange, closeRangeLookUp, vector.y);
		minDist = 0.025f;
	}

	private void CalculateFirstPersonCam(out Vector3 targetOffset, out float yaw, out float pitch, out float camDist, out float minDist, out float fov, out float nearClip)
	{
		fov = 70f;
		nearClip = this.nearClip;
		yaw = human.controls.cameraYawAngle;
		pitch = human.controls.cameraPitchAngle + 10f;
		if (pitch < 0f)
		{
			pitch *= 0.8f;
		}
		Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
		targetOffset = rotation * fpsTargetOffset;
		camDist = 0f;
		minDist = 0f;
	}

	private float CompensateForWallsNearPlane(Vector3 targetPos, Quaternion lookRot, float desiredDist, float minDist)
	{
		Vector3 a = lookRot * Vector3.forward;
		float nearClipPlane = gameCam.nearClipPlane;
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = lookRot;
		base.transform.position = targetPos - a * (gameCam.nearClipPlane + minDist);
		rayStarts[0] = gameCam.ViewportToWorldPoint(new Vector3(0f, 0f, nearClipPlane));
		rayStarts[1] = gameCam.ViewportToWorldPoint(new Vector3(0f, 1f, nearClipPlane));
		rayStarts[2] = gameCam.ViewportToWorldPoint(new Vector3(1f, 0f, nearClipPlane));
		rayStarts[3] = gameCam.ViewportToWorldPoint(new Vector3(1f, 1f, nearClipPlane));
		float num = desiredDist - nearClipPlane;
		for (int i = 0; i < rayStarts.Length; i++)
		{
			Ray ray = new Ray(rayStarts[i], -a);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, num, wallLayers) && hitInfo.distance < num)
			{
				num = hitInfo.distance;
			}
		}
		num += nearClipPlane + minDist;
		if (num < minDist * 2f)
		{
			num = minDist * 2f;
		}
		return num;
	}

	public void TransitionFromCurrent(float duration)
	{
		if (duration == 0f)
		{
			throw new ArgumentException("duration can't be 0", "duration");
		}
		cameraTransitionPhase = 0f;
		cameraTransitionSpeed = 1f / duration;
		startFov = gameCam.fieldOfView;
		startOffset = base.transform.position - human.transform.position;
		startRotation = base.transform.rotation;
		startHumanCameraPos = gameCam.WorldToViewportPoint(human.transform.position);
	}

	public void TransitionFrom(GameCamera gameCamera, float focusDist, float duration)
	{
		if (duration == 0f)
		{
			throw new ArgumentException("duration can't be 0", "duration");
		}
		cameraTransitionPhase = 0f;
		cameraTransitionSpeed = 1f / duration;
		startFov = gameCam.fieldOfView;
		startOffset = gameCamera.transform.position - human.transform.position;
		startRotation = gameCamera.transform.rotation;
		startHumanCameraPos = gameCamera.gameCam.WorldToViewportPoint(human.transform.position);
	}

	public void ApplyCamera(Vector3 target, Vector3 position, Quaternion rotation, float fov)
	{
		cameraTransitionPhase += cameraTransitionSpeed * Time.deltaTime;
		if (cameraTransitionPhase >= 1f)
		{
			base.transform.position = position;
			base.transform.rotation = rotation;
			gameCam.fieldOfView = fov;
		}
		else
		{
			float num = Ease.easeInOutSine(0f, 1f, Mathf.Clamp01(cameraTransitionPhase));
			base.transform.rotation = rotation;
			base.transform.position = position;
			gameCam.fieldOfView = fov;
			Vector3 b = gameCam.WorldToViewportPoint(human.transform.position);
			Vector3 position2 = Vector3.Lerp(startHumanCameraPos, b, cameraTransitionPhase);
			base.transform.rotation = Quaternion.Lerp(startRotation, rotation, cameraTransitionPhase);
			gameCam.fieldOfView = Mathf.Lerp(startFov, fov, cameraTransitionPhase);
			Vector3 b2 = gameCam.ViewportToWorldPoint(position2);
			base.transform.position += human.transform.position - b2;
		}
		float num2 = (!human.controls.leftGrab && !human.controls.rightGrab) ? Mathf.Clamp((target - position).magnitude, 6f, 6f) : Mathf.Clamp((target - position).magnitude, 4f, 5f);
	}
}
