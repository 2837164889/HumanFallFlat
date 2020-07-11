using I2.Loc;
using Multiplayer;
using UnityEngine;

public class FreeRoamCam : MonoBehaviour
{
	public struct CameraKeyFrame
	{
		public float frame;

		public bool targetFocus;

		public bool humanRelative;

		public Vector3 pos;

		public Vector3 targetPos;

		public Quaternion rot;
	}

	private Camera cam;

	public GameObject logoOverlay;

	private Vector3 pos1;

	private Vector3 pos2;

	private Quaternion rot1;

	private Quaternion rot2;

	private float duration = 10f;

	private float animationPhase = 1f;

	public static bool allowFreeRoam;

	private CameraKeyFrame[] keyframes = new CameraKeyFrame[9];

	private const float kFogSpeed = 10f;

	private const float kFogMin = 0f;

	private const float kFogMax = 60f;

	private bool coopCameraToggle;

	private void OnEnable()
	{
		ResetKeyFrames();
	}

	private void ResetKeyFrames()
	{
		for (int i = 0; i < keyframes.Length; i++)
		{
			keyframes[i].frame = -1f;
		}
		if (ReplayUI.instance != null)
		{
			ReplayUI.instance.HideCameras();
		}
	}

	public static void CleanUp()
	{
		allowFreeRoam = false;
		MenuCameraEffects.instance.RemoveOverride();
	}

	private void Update()
	{
		if (Game.instance == null)
		{
			return;
		}
		if (NetGame.isLocal && Game.GetKeyDown(KeyCode.F6))
		{
			if (Time.timeScale == 0f)
			{
				Time.timeScale = 1f;
			}
			else
			{
				Time.timeScale = 0f;
			}
		}
		if (Input.GetKeyDown(KeyCode.F7) && NetGame.instance.players.Count == 2)
		{
			MenuCameraEffects.instance.SetCOOPFullScreenViewport(coopCameraToggle);
			coopCameraToggle = !coopCameraToggle;
		}
		if (Game.GetKeyDown(KeyCode.F8))
		{
			if (!GameSave.ProgressMoreOrEqual(Game.instance.currentLevelNumber + 1, 0))
			{
				SubtitleManager.instance.SetProgress(ScriptLocalization.TUTORIAL.CAMERADISABLED, 5f, -1f);
				return;
			}
			ResetKeyFrames();
			if (!allowFreeRoam)
			{
				Camera gameCam = NetGame.instance.local.players[0].cameraController.gameCam;
				base.transform.position = gameCam.transform.position;
				base.transform.rotation = gameCam.transform.rotation;
				allowFreeRoam = true;
				cam = MenuCameraEffects.instance.OverrideCamera(base.transform, applyEffects: true);
			}
			else
			{
				CleanUp();
			}
		}
		if (NetGame.isLocal && Game.GetKeyDown(KeyCode.T) && Game.GetKey(KeyCode.LeftShift) && Game.GetKey(KeyCode.LeftControl))
		{
			if (Time.timeScale == 1f)
			{
				Time.timeScale = 0.5f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}
		if (Game.GetKey(KeyCode.LeftBracket))
		{
			MenuCameraEffects.instance.zoom.to *= Mathf.Pow(1.4f, Time.unscaledDeltaTime);
		}
		if (Game.GetKey(KeyCode.RightBracket))
		{
			MenuCameraEffects.instance.zoom.to /= Mathf.Pow(1.4f, Time.unscaledDeltaTime);
		}
		if (Game.GetKey(KeyCode.Comma))
		{
			CaveRender.fogDensityMultiplier -= 10f * Time.unscaledDeltaTime;
		}
		if (Game.GetKey(KeyCode.Period))
		{
			CaveRender.fogDensityMultiplier += 10f * Time.unscaledDeltaTime;
		}
		CaveRender.fogDensityMultiplier = Mathf.Clamp(CaveRender.fogDensityMultiplier, 0f, 60f);
		if (Game.GetKeyDown(KeyCode.F9))
		{
			Camera gameCam2 = cam;
			if (gameCam2 == null)
			{
				gameCam2 = NetGame.instance.local.players[0].cameraController.gameCam;
			}
			if (!gameCam2.isActiveAndEnabled)
			{
				MultiplayerLobbyController multiplayerLobbyController = Object.FindObjectOfType<MultiplayerLobbyController>();
				if (multiplayerLobbyController != null)
				{
					gameCam2 = multiplayerLobbyController.gameCamera.gameCam;
				}
			}
			if (gameCam2 != null)
			{
				RenderTexture renderTexture2 = gameCam2.targetTexture = new RenderTexture(1024, 576, 16);
				gameCam2.Render();
				RenderTexture.active = renderTexture2;
				Texture2D texture2D = new Texture2D(renderTexture2.width, renderTexture2.height, TextureFormat.RGB24, mipmap: false);
				texture2D.ReadPixels(new Rect(0f, 0f, renderTexture2.width, renderTexture2.height), 0, 0);
				RenderTexture.active = null;
				gameCam2.targetTexture = null;
				FileTools.WriteTexture(FileTools.Combine(Application.persistentDataPath, "thumbnail.png"), texture2D);
				Object.Destroy(renderTexture2);
				SubtitleManager.instance.SetProgress(ScriptLocalization.Get("WORKSHOP/ThumbnailCaptured"), 2f, 0.5f);
			}
		}
		if (!allowFreeRoam)
		{
			return;
		}
		if (Game.GetKeyDown(KeyCode.Alpha0))
		{
			ResetKeyFrames();
		}
		for (int i = 0; i < keyframes.Length; i++)
		{
			if (Game.GetKeyDown((KeyCode)(49 + i)))
			{
				Human human = Human.all[0];
				keyframes[i] = new CameraKeyFrame
				{
					pos = base.transform.position,
					targetPos = human.transform.position,
					rot = base.transform.rotation,
					targetFocus = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.Tab)),
					humanRelative = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.Tab)),
					frame = ReplayRecorder.instance.currentFrame
				};
				if (keyframes[i].humanRelative)
				{
					keyframes[i].pos -= human.transform.position;
				}
				ReplayUI.instance.SyncCameras(keyframes);
			}
		}
		if (ReplayRecorder.instance.state == ReplayRecorder.ReplayState.PlayForward || ReplayRecorder.instance.state == ReplayRecorder.ReplayState.PlayBackward)
		{
			for (int j = 0; j < keyframes.Length; j++)
			{
				CameraKeyFrame cameraKeyFrame = keyframes[j];
				if (cameraKeyFrame.frame < 0f)
				{
					break;
				}
				if (cameraKeyFrame.frame >= (float)ReplayRecorder.instance.currentFrame)
				{
					CameraKeyFrame prevKeyframe = (j <= 0) ? cameraKeyFrame : keyframes[j - 1];
					float t = Mathf.InverseLerp(prevKeyframe.frame, cameraKeyFrame.frame, ReplayRecorder.instance.currentFrame);
					SyncCamera(cameraKeyFrame, prevKeyframe, t);
					return;
				}
			}
		}
		else if (ReplayRecorder.instance.state == ReplayRecorder.ReplayState.None && keyframes[0].pos != Vector3.zero && keyframes[1].pos != Vector3.zero)
		{
			if (Game.GetKeyDown(KeyCode.Space))
			{
				if (animationPhase < 1f)
				{
					animationPhase = 1f;
				}
				else
				{
					animationPhase = 0f;
				}
			}
			if (animationPhase < 1f)
			{
				animationPhase += Time.unscaledDeltaTime / duration;
				SyncCamera(keyframes[1], keyframes[0], animationPhase);
				return;
			}
		}
		bool flag = Game.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		int num = flag ? 1 : 10;
		if (Game.GetKey(KeyCode.W))
		{
			base.transform.position += base.transform.forward * Time.unscaledDeltaTime * num;
		}
		if (Game.GetKey(KeyCode.S))
		{
			base.transform.position -= base.transform.forward * Time.unscaledDeltaTime * num;
		}
		if (Game.GetKey(KeyCode.A))
		{
			base.transform.position -= base.transform.right * Time.unscaledDeltaTime * num;
		}
		if (Game.GetKey(KeyCode.D))
		{
			base.transform.position += base.transform.right * Time.unscaledDeltaTime * num;
		}
		if (Game.GetKey(KeyCode.Q))
		{
			base.transform.position += base.transform.up * Time.unscaledDeltaTime * num;
		}
		if (Game.GetKey(KeyCode.Z))
		{
			base.transform.position -= base.transform.up * Time.unscaledDeltaTime * num;
		}
		Vector2 mouseScrollDelta = Input.mouseScrollDelta;
		float y = mouseScrollDelta.y;
		cam.fieldOfView *= Mathf.Pow(1.1f, 0f - y);
		cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 5f, 120f);
		cam.nearClipPlane = 0.05f * Mathf.Pow(1.1f, 0f - Mathf.Log(cam.fieldOfView / 60f, 1.1f));
		if (Input.GetMouseButton(2))
		{
			cam.fieldOfView = 60f;
			cam.nearClipPlane = 0.05f;
		}
		float num2 = (!flag) ? 0.2f : 0.1f;
		float axis = Input.GetAxis("mouse x");
		float axis2 = Input.GetAxis("mouse y");
		if (axis != 0f || axis2 != 0f)
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			if (eulerAngles.x > 180f)
			{
				eulerAngles.x -= 360f;
			}
			if (eulerAngles.x < -180f)
			{
				eulerAngles.x += 360f;
			}
			eulerAngles.x -= axis2 * num2;
			if (eulerAngles.x < -89f)
			{
				eulerAngles.x = -89f;
			}
			if (eulerAngles.x > 89f)
			{
				eulerAngles.x = 89f;
			}
			eulerAngles.y += axis * num2;
			base.transform.rotation = Quaternion.Euler(eulerAngles);
		}
	}

	private void SyncCamera(CameraKeyFrame keyframe, CameraKeyFrame prevKeyframe, float t)
	{
		Human human = Human.all[0];
		Vector3 pos = prevKeyframe.pos;
		Quaternion a = prevKeyframe.rot;
		Vector3 pos2 = keyframe.pos;
		Quaternion b = keyframe.rot;
		if (prevKeyframe.humanRelative)
		{
			pos += human.transform.position;
		}
		if (keyframe.humanRelative)
		{
			pos2 += human.transform.position;
		}
		Vector3 vector = Vector3.Lerp(pos, pos2, t);
		Vector3 a2 = Vector3.Lerp(prevKeyframe.targetPos, keyframe.targetPos, t);
		Quaternion quaternion = Quaternion.LookRotation(a2 - vector, Vector3.up);
		Quaternion quaternion2 = Quaternion.LookRotation(human.transform.position - vector, Vector3.up);
		if (prevKeyframe.targetFocus)
		{
			a = quaternion;
		}
		if (keyframe.targetFocus)
		{
			b = quaternion;
		}
		if (prevKeyframe.humanRelative)
		{
			a = quaternion2;
		}
		if (keyframe.humanRelative)
		{
			b = quaternion2;
		}
		Quaternion rotation = Quaternion.Lerp(a, b, t);
		base.transform.position = vector;
		base.transform.rotation = rotation;
	}
}
