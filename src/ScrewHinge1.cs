using UnityEngine;

public class ScrewHinge1 : MonoBehaviour
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

	[SerializeField]
	private float screwRatio;

	[SerializeField]
	private float heightMax;

	[SerializeField]
	private float heightMin;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		preAngle = base.transform.rotation;
		Vector3 position = base.transform.position;
		preHeight = position.y;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		preHeight = position.y;
		diffAngle = Quaternion.Angle(base.transform.rotation, preAngle);
		if (diffAngle > deadAngle)
		{
			Vector3 eulerAngles = preAngle.eulerAngles;
			float y = eulerAngles.y;
			Vector3 eulerAngles2 = base.transform.eulerAngles;
			if (y > eulerAngles2.y)
			{
				diffAngle = 0f - diffAngle;
			}
			Vector3 position2 = base.transform.position;
			float x = position2.x;
			Vector3 position3 = base.transform.position;
			float y2 = position3.y + diffAngle * screwRatio;
			Vector3 position4 = base.transform.position;
			Vector3 position5 = new Vector3(x, y2, position4.z);
			bool flag = false;
			if (position5.y > heightMax)
			{
				Vector3 eulerAngles3 = preAngle.eulerAngles;
				float y3 = eulerAngles3.y;
				Vector3 eulerAngles4 = base.transform.eulerAngles;
				if (y3 < eulerAngles4.y)
				{
					flag = true;
				}
			}
			if (position5.y < heightMin)
			{
				Vector3 eulerAngles5 = preAngle.eulerAngles;
				float y4 = eulerAngles5.y;
				Vector3 eulerAngles6 = base.transform.eulerAngles;
				if (y4 > eulerAngles6.y)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				rb.MovePosition(position5);
			}
			else
			{
				base.transform.rotation = preAngle;
			}
		}
		preAngle = base.transform.rotation;
	}
}
