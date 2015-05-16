namespace NH
{
	using UnityEngine;
	using ClassExtensions;
	using System.Collections;

	[RequireComponent(typeof(Rigidbody2D))]
	public class MovementScript : MonoBehaviour
	{
		public float maxHorizontalSpeed = 5;
		public float maxVerticalSpeed = 7;
		public float horizontalForce = 150;
		public float verticalForce = 300;

		public void AccelerateHorizontal(float moveValue) 
		{
			if (moveValue * this.GetComponent<Rigidbody2D>().velocity.x < this.maxHorizontalSpeed) 
			{
				this.GetComponent<Rigidbody2D>().AddForce(Vector2.right * moveValue * this.horizontalForce);
			}

			if (Mathf.Abs(this.GetComponent<Rigidbody2D>().velocity.x) > Mathf.Abs(moveValue) * this.maxHorizontalSpeed)
			{
				this.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(this.GetComponent<Rigidbody2D>().velocity.x) * Mathf.Abs(moveValue) * this.maxHorizontalSpeed, this.GetComponent<Rigidbody2D>().velocity.y);
			}
		}

		public void AccelerateVertical(float moveValue)
		{
			if (moveValue * this.GetComponent<Rigidbody2D>().velocity.y < this.maxVerticalSpeed) 
			{
				this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * moveValue * this.verticalForce);
			}
			
			if (Mathf.Abs(this.GetComponent<Rigidbody2D>().velocity.y) > Mathf.Abs(moveValue) * this.maxVerticalSpeed)
			{
				this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, Mathf.Abs(moveValue * this.maxVerticalSpeed));
			}
		}

		public void MoveHorizontal(float moveValue) 
		{
			this.GetComponent<Rigidbody2D>().velocity = new Vector2(moveValue * this.maxHorizontalSpeed, this.GetComponent<Rigidbody2D>().velocity.y);
		}

		public void MoveVertical(float moveValue)
		{
			this.GetComponent<Rigidbody2D>().velocity = new Vector2(this.GetComponent<Rigidbody2D>().velocity.x, Mathf.Abs(moveValue * this.maxVerticalSpeed));
		}
	}
}