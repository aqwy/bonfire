using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPlayerCar))]
	public class D2dPlayerCar_Editor : D2dEditor<D2dPlayerCar>
	{
		protected override void OnInspector()
		{
			DrawDefault("SteerWheels");
			DrawDefault("SteerAngleMax");
			DrawDefault("SteerAngleDampening");
			
			Separator();

			DrawDefault("DriveWheels");
			DrawDefault("DriveTorque");
		}
	}
}
#endif

namespace Destructible2D
{
	// This component allows you to control a car
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Player Car")]
	public class D2dPlayerCar : MonoBehaviour
	{
		[Tooltip("The wheels used to steer this car")]
		public D2dWheel[] SteerWheels;

		[Tooltip("The maximum +- angle of turning")]
		public float SteerAngleMax = 20.0f;

		[Tooltip("How quickly the steering wheels turn to their target angle")]
		public float SteerAngleDampening = 5.0f;
		
		[Tooltip("The wheels used to move this car")]
		public D2dWheel[] DriveWheels;

		[Tooltip("The maximum torque that can be applied to each drive wheel")]
		public float DriveTorque = 1.0f;
		
		// Current steering angle
		[SerializeField]
		private float currentAngle;
		
		protected virtual void Update()
		{
			var targetAngle = Input.GetAxisRaw("Horizontal") * SteerAngleMax;

			currentAngle = D2dHelper.Dampen(currentAngle, targetAngle, SteerAngleDampening, Time.deltaTime);
			
			for (var i = 0; i < SteerWheels.Length; i++)
			{
				SteerWheels[i].transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -currentAngle);
			}
		}
		
		protected virtual void FixedUpdate()
		{
			for (var i = 0; i < DriveWheels.Length; i++)
			{
				DriveWheels[i].AddTorque(Input.GetAxisRaw("Vertical") * DriveTorque * Time.fixedDeltaTime);
			}
		}
	}
}