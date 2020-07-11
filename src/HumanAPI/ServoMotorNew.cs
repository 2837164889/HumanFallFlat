using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class ServoMotorNew : Node, INetBehavior
	{
		[Tooltip("The things that needs to move vertically when told")]
		public JointBase joint;

		[Tooltip("Position mode interpolates between min&max values based on signal (0-1), Speed mode - uses signed signal value to set speed up to maxSpeed")]
		public ServoMode mode;

		[Tooltip("Input: Source signal")]
		public NodeInput input;

		[Tooltip("Power: the power needed for this to work")]
		public NodeInput power = new NodeInput
		{
			initialValue = 0f
		};

		[Tooltip("Output: 1 when motor is engaged, 0 when no motor is working")]
		public NodeOutput motor;

		[Tooltip("Output: 1 when moving at maxSpeed")]
		public NodeOutput velocity;

		[Tooltip("Delay before starting to move in opposite direction")]
		public float reverseDelay = 0.1f;

		[Tooltip("Whether or not to use the normalised values for output")]
		public bool signedInput;

		private NetIdentity identity;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private float speed;

		private float position = -10000f;

		private float oldpower;

		private float reachedEnd;

		private float disableMotorTime;

		private float direction;

		[Tooltip(" The amount of power this thing needs in order to run ")]
		public float neededVoltage = 1f;

		[Tooltip("Whether or not the servo is powered without voltage")]
		public bool unpoweredWithoutVoltage;

		private float maxForce;

		public float actualPosition;

		public float Speed => speed;

		private void Awake()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Awake ");
			}
			if (joint == null)
			{
				joint = GetComponent<JointBase>();
			}
			if (joint != null)
			{
				joint.EnsureInitialized();
				position = joint.initialValue;
				maxForce = joint.maxForce;
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the joint var ");
			}
		}

		public override void Process()
		{
			base.Process();
			if (!SignalManager.skipTransitions)
			{
				return;
			}
			if (joint != null)
			{
				speed = 0f;
				if (mode == ServoMode.Position)
				{
					position = Mathf.Lerp(joint.minValue, joint.maxValue, (!signedInput) ? input.value : ((input.value + 1f) / 2f));
				}
				else if (joint != null)
				{
					position = joint.initialValue;
				}
				else if (showDebug)
				{
					Debug.Log(base.name + " There is nothing set within the joint var ");
				}
				if (joint != null)
				{
					if (joint.isKinematic)
					{
						SetPosition(position);
						return;
					}
					float spring = joint.maxForce / joint.tensionDist;
					float damper = joint.maxForce / joint.maxSpeed;
					SetJoint(position, spring, damper);
					SetPosition(position);
				}
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the joint var ");
			}
		}

		private void FixedUpdate()
		{
			if (identity != null && (ReplayRecorder.isPlaying || NetGame.isClient))
			{
				return;
			}
			disableMotorTime -= Time.fixedDeltaTime;
			float num = position;
			float num2 = direction;
			float num3 = power.value / neededVoltage;
			if (Mathf.Abs(num3) < 0.9f)
			{
				num3 = 0f;
			}
			if (joint != null)
			{
				if (unpoweredWithoutVoltage)
				{
					joint.maxForce = ((!(num3 > 0f)) ? 0f : maxForce);
				}
				if (mode == ServoMode.Position)
				{
					float num4 = Mathf.Lerp(joint.minValue, joint.maxValue, (!signedInput) ? input.value : ((input.value + 1f) / 2f));
					float num5 = num4 - num;
					float num6 = (num5 > 0f) ? 1 : ((num5 < 0f) ? (-1) : 0);
					speed = Mathf.MoveTowards(speed, num6 * joint.maxSpeed, joint.maxAcceleration * Time.fixedDeltaTime);
					position += speed * num3 * Time.fixedDeltaTime;
					if (speed < 0f && position < num4 && num > num4)
					{
						position = num4;
						speed = 0f;
					}
					if (speed > 0f && position > num4 && num < num4)
					{
						position = num4;
						speed = 0f;
					}
					direction = Mathf.Sign(position - num);
					if (direction * num2 < 0f)
					{
						disableMotorTime = reverseDelay;
					}
					motor.SetValue((num3 > 0f && disableMotorTime <= 0f && Mathf.Abs(num - num4) >= joint.tensionDist) ? 1 : 0);
				}
				else
				{
					if (joint != null)
					{
						if (speed * input.value < 0f)
						{
							speed = 0f;
						}
						speed = Mathf.MoveTowards(speed, input.value * joint.maxSpeed, joint.maxAcceleration * Time.fixedDeltaTime);
						position += speed * num3 * Time.fixedDeltaTime;
						direction = Mathf.Sign(position - num);
						if (direction * num2 < 0f)
						{
							disableMotorTime = reverseDelay;
						}
						if (mode == ServoMode.Speed)
						{
							if (reachedEnd == -1f && input.value * num3 > 0f)
							{
								reachedEnd = 0f;
							}
							else if (reachedEnd == 1f && input.value * num3 < 0f)
							{
								reachedEnd = 0f;
							}
							else if (position < joint.minValue + joint.tensionDist && input.value * num3 <= 0f)
							{
								reachedEnd = -1f;
							}
							else if (position > joint.maxValue - joint.tensionDist && input.value * num3 >= 0f)
							{
								reachedEnd = 1f;
							}
						}
					}
					motor.SetValue((Mathf.Abs(num3) > 0f && disableMotorTime <= 0f && reachedEnd == 0f && input.value != 0f) ? 1 : 0);
				}
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the joint var ");
			}
			if (num != position || oldpower != num3)
			{
				if (joint != null)
				{
					velocity.SetValue(Mathf.Clamp01(Mathf.Abs(position - num) / Time.fixedDeltaTime / joint.maxSpeed));
					if (joint.isKinematic || !joint.useSpring)
					{
						if (mode == ServoMode.Speed)
						{
							position = Mathf.Clamp(position, joint.minValue, joint.maxValue);
						}
						SetPosition(position);
					}
					else
					{
						actualPosition = GetActualPosition();
						position = Mathf.Clamp(position, actualPosition - joint.tensionDist, actualPosition + joint.tensionDist);
						float spring = joint.maxForce / joint.tensionDist * Mathf.Abs(num3);
						float damper = joint.maxForce / joint.maxSpeed * Mathf.Abs(num3);
						SetJoint(position, spring, damper);
					}
				}
				else if (showDebug)
				{
					Debug.Log(base.name + " There is nothing set within the joint var ");
				}
			}
			else
			{
				velocity.SetValue(0f);
			}
			oldpower = num3;
		}

		private float GetActualPosition()
		{
			return joint.GetValue();
		}

		private void SetPosition(float angle)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Set Position ");
			}
			position = angle;
			if (joint != null)
			{
				if (joint.GetValue() != angle)
				{
					joint.SetValue(angle);
				}
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the joint var ");
			}
		}

		private void SetJoint(float pos, float spring, float damper)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Set Joint ");
			}
			if (joint != null)
			{
				if (joint.GetTarget() != pos)
				{
					if (showDebug)
					{
						Debug.Log(base.name + " Setting the Target ");
					}
					joint.SetTarget(pos);
				}
			}
			else if (showDebug)
			{
				Debug.Log(base.name + " There is nothing set within the joint var ");
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Start Network ");
			}
			this.identity = identity;
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}

		public void SetMaster(bool isMaster)
		{
		}

		public void CollectState(NetStream stream)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Coolect State ");
			}
			NetSignal.encoder.CollectState(stream, motor.value);
			NetSignal.encoder.CollectState(stream, velocity.value);
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply Lerped State ");
			}
			speed = 0f;
			motor.SetValue(NetSignal.encoder.ApplyLerpedState(state0, state1, mix));
			velocity.SetValue(NetSignal.encoder.ApplyLerpedState(state0, state1, mix));
		}

		public void ApplyState(NetStream state)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Apply State ");
			}
			speed = 0f;
			motor.SetValue(NetSignal.encoder.ApplyState(state));
			velocity.SetValue(NetSignal.encoder.ApplyState(state));
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Calc Delta ");
			}
			NetSignal.encoder.CalculateDelta(state0, state1, delta);
			NetSignal.encoder.CalculateDelta(state0, state1, delta);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Add Delta ");
			}
			NetSignal.encoder.AddDelta(state0, delta, result);
			NetSignal.encoder.AddDelta(state0, delta, result);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Mx Delta ");
			}
			return 2 * NetSignal.encoder.CalculateMaxDeltaSizeInBits();
		}
	}
}
