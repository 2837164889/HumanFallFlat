using UnityEngine;

public class ScrewHinge : MonoBehaviour
{
	private Quaternion preAngle;

	private float preHeight;

	[SerializeField]
	private float deadAngle;

	[SerializeField]
	private float diffAngle;

	[SerializeField]
	private float diffHeight;

	private Rigidbody rb;

	private Rigidbody vrb;

	[SerializeField]
	private float screwRatio;

	[SerializeField]
	private GameObject vertical;

	[SerializeField]
	private float heightMax;

	[SerializeField]
	private float heightMin;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		vrb = vertical.GetComponent<Rigidbody>();
		preAngle = base.transform.rotation;
		Vector3 position = base.transform.position;
		preHeight = position.y;
	}

	private void Update()
	{
		diffAngle = Quaternion.Angle(base.transform.rotation, preAngle);
		if (diffAngle > deadAngle)
		{
			vrb.isKinematic = true;
			Vector3 eulerAngles = preAngle.eulerAngles;
			float y = eulerAngles.y;
			Vector3 eulerAngles2 = base.transform.eulerAngles;
			if (y > eulerAngles2.y)
			{
				diffAngle = 0f - diffAngle;
			}
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			float y2 = position2.y + diffAngle * screwRatio;
			Vector3 position3 = base.transform.position;
			Vector3 position4 = new Vector3(x, y2, position3.z);
			vrb.MovePosition(position4);
		}
		else
		{
			vrb.isKinematic = false;
			Vector3 position5 = vertical.transform.position;
			diffHeight = position5.y - preHeight;
			Vector3 eulerAngles3 = base.transform.eulerAngles;
			float x2 = eulerAngles3.x;
			Vector3 eulerAngles4 = base.transform.eulerAngles;
			float y3 = eulerAngles4.y + diffHeight / screwRatio;
			Vector3 eulerAngles5 = base.transform.eulerAngles;
			Vector3 euler = new Vector3(x2, y3, eulerAngles5.z);
			vrb.MoveRotation(Quaternion.Euler(euler));
		}
		Vector3 position6 = vertical.transform.position;
		preHeight = position6.y;
		preAngle = base.transform.rotation;
	}
}
