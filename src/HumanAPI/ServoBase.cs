using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public abstract class ServoBase : Node, INetBehavior
	{
		[Tooltip("Position mode interpolates between min&max values based on signal (0-1), Speed mode - uses signed signal value to set speed up to maxSpeed")]
		public ServoMode mode;

		[Tooltip("Input: Source signal")]
		public NodeInput input;

		[Tooltip("Input: Power - multiplies the maxspeed")]
		public NodeInput power = new NodeInput
		{
			initialValue = 1f
		};

		[Tooltip("Output: 1 when motor is engaged, 0 when no motor is working")]
		public NodeOutput motor;

		[Tooltip("Output: 1 when moving at maxSpeed")]
		public NodeOutput velocity;

		public bool signedInput;

		[Tooltip("Value that matches model state")]
		public float initialValue;

		[Tooltip("Minimum range of servo action")]
		public float minValue = -1f;

		[Tooltip("Maximum range of servo action")]
		public float maxValue = 1f;

		[Tooltip("Maximum speed to move servo")]
		public float maxSpeed = 1f;

		[Tooltip("Maximum acceleration to move servo")]
		public float maxAcceleration = 10f;

		[Tooltip("The stiffness of spring driving the servo, smaller values - more stiff")]
		public float tensionDist = 0.1f;

		[Tooltip("Maximum force of the servo motor")]
		public float maxForce;

		[Tooltip("Delay before starting to move in opposite direction")]
		public float reverseDelay = 0.1f;

		protected bool isKinematic;

		private float speed;

		private float position = -10000f;

		private float reachedEnd;

		private float disableMotorTime;

		private float direction;

		public float actualPosition;

		private NetFloat encoder = new NetFloat(1f, 12, 3, 8);

		protected abstract float GetActualPosition();

		protected abstract void SetJoint(float pos, float spring, float damper);

		protected abstract void SetStatic(float pos);

		protected virtual void Awake()
		{
			tensionDist = Mathf.Max(tensionDist, maxSpeed * Time.fixedDeltaTime);
		}

		public override void Process()
		{
			base.Process();
			if (SignalManager.skipTransitions)
			{
				speed = 0f;
				if (mode == ServoMode.Position)
				{
					position = Mathf.Lerp(minValue, maxValue, (!signedInput) ? input.value : ((input.value + 1f) / 2f));
				}
				else
				{
					position = initialValue;
				}
				if (isKinematic)
				{
					SetStatic(position);
					return;
				}
				float spring = maxForce / tensionDist;
				float damper = maxForce / maxSpeed;
				SetJoint(position, spring, damper);
			}
		}

		private void FixedUpdate()
		{
			if (ReplayRecorder.isPlaying || NetGame.isClient)
			{
				return;
			}
			disableMotorTime -= Time.fixedDeltaTime;
			float num = position;
			float num2 = direction;
			if (mode == ServoMode.Position)
			{
				float num3 = Mathf.Lerp(minValue, maxValue, (!signedInput) ? input.value : ((input.value + 1f) / 2f));
				speed = Mathf.MoveTowards(speed, Mathf.Sign(num3 - num) * maxSpeed, maxAcceleration * Time.fixedDeltaTime);
				position += speed * power.value * Time.fixedDeltaTime;
				if (speed < 0f && position < num3 && num >= num3)
				{
					position = num3;
				}
				if (speed > 0f && position > num3 && num <= num3)
				{
					position = num3;
				}
				direction = Mathf.Sign(position - num);
				if (direction * num2 < 0f)
				{
					disableMotorTime = reverseDelay;
				}
				motor.SetValue((power.value > 0f && disableMotorTime <= 0f && Mathf.Abs(num - num3) >= tensionDist) ? 1 : 0);
			}
			else
			{
				if (speed * input.value < 0f)
				{
					speed = 0f;
				}
				speed = Mathf.MoveTowards(speed, input.value * maxSpeed, maxAcceleration * Time.fixedDeltaTime);
				position += speed * power.value * Time.fixedDeltaTime;
				direction = Mathf.Sign(position - num);
				if (direction * num2 < 0f)
				{
					disableMotorTime = reverseDelay;
				}
				if (mode == ServoMode.Speed)
				{
					if (reachedEnd == -1f && input.value > 0f)
					{
						reachedEnd = 0f;
					}
					else if (reachedEnd == 1f && input.value < 0f)
					{
						reachedEnd = 0f;
					}
					else if (position < minValue + tensionDist && input.value < 0f)
					{
						reachedEnd = -1f;
					}
					else if (position > maxValue - tensionDist && input.value > 0f)
					{
						reachedEnd = 1f;
					}
				}
				motor.SetValue((power.value > 0f && disableMotorTime <= 0f && reachedEnd == 0f && input.value != 0f) ? 1 : 0);
			}
			if (num != position)
			{
				velocity.SetValue(Mathf.Clamp01(Mathf.Abs(position - num) / Time.fixedDeltaTime / maxSpeed));
				if (isKinematic)
				{
					if (mode != ServoMode.SpeedIgnoreLimit)
					{
						position = Mathf.Clamp(position, minValue, maxValue);
					}
					SetStatic(position);
				}
				else
				{
					actualPosition = GetActualPosition();
					position = Mathf.Clamp(position, actualPosition - tensionDist, actualPosition + tensionDist);
					float spring = maxForce / tensionDist * power.value;
					float damper = maxForce / maxSpeed * power.value;
					SetJoint(position, spring, damper);
				}
			}
			else
			{
				velocity.SetValue(0f);
			}
		}

		public void StartNetwork(NetIdentity identity)
		{
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
		}

		public void SetMaster(bool isMaster)
		{
		}

		public void CollectState(NetStream stream)
		{
			encoder.CollectState(stream, Mathf.InverseLerp(minValue, maxValue, GetActualPosition()));
			NetSignal.encoder.CollectState(stream, motor.value);
			NetSignal.encoder.CollectState(stream, velocity.value);
		}

		public void ApplyState(NetStream state)
		{
			position = Mathf.Lerp(minValue, maxValue, encoder.ApplyState(state));
			speed = 0f;
			SetStatic(position);
			motor.SetValue(NetSignal.encoder.ApplyState(state));
			velocity.SetValue(NetSignal.encoder.ApplyState(state));
		}

		public void ApplyLerpedState(NetStream state0, NetStream state1, float mix)
		{
			position = Mathf.Lerp(minValue, maxValue, encoder.ApplyLerpedState(state0, state1, mix));
			speed = 0f;
			SetStatic(position);
			motor.SetValue(NetSignal.encoder.ApplyLerpedState(state0, state1, mix));
			velocity.SetValue(NetSignal.encoder.ApplyLerpedState(state0, state1, mix));
		}

		public void CalculateDelta(NetStream state0, NetStream state1, NetStream delta)
		{
			encoder.CalculateDelta(state0, state1, delta);
			NetSignal.encoder.CalculateDelta(state0, state1, delta);
			NetSignal.encoder.CalculateDelta(state0, state1, delta);
		}

		public void AddDelta(NetStream state0, NetStream delta, NetStream result)
		{
			encoder.AddDelta(state0, delta, result);
			NetSignal.encoder.AddDelta(state0, delta, result);
			NetSignal.encoder.AddDelta(state0, delta, result);
		}

		public int CalculateMaxDeltaSizeInBits()
		{
			return encoder.CalculateMaxDeltaSizeInBits() + 2 * NetSignal.encoder.CalculateMaxDeltaSizeInBits();
		}
	}
}
