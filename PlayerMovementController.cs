namespace NH
{
	using ClassExtensions;
	using UnityEngine;
	using System.Collections;
	using MT = MovementType;
	using LT = AnimatorLinkType;

	public enum MovementType
	{
		Acceleration,
		Movement
	}
	;

	public enum AnimatorLinkType
	{
		Bool,
		Int,
		Float
	}
	;

	[System.Serializable]
	public class AnimatorLink
	{
		public AnimatorLinkType type;
		public string name;
		public string keyPath;

		public void PerformLink (object target, Animator animator)
		{

			switch (this.type) {
			case LT.Float:
				animator.SetFloat (this.name, target.ValueAtPath<float> (this.keyPath));
				break;
			case LT.Int:
				animator.SetInteger (this.name, target.ValueAtPath<int> (this.keyPath));
				break;
			case LT.Bool:
				animator.SetBool (this.name, target.ValueAtPath<bool> (this.keyPath));
				break;
			default:
				break;
			}
		}
	}

	[System.Serializable]
	public class TouchInfo
	{
		public int id = -1;
		public Vector2 initialPosition;
		public Vector2 currentPosition;
		public float time = 0;
	}

	[RequireComponent(typeof(MovementScript))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Animator))]
	public class PlayerMovementController : MonoBehaviour
	{
		public MovementScript movementScript;
		public Animator animator;
		public float horizontalMovement;
		public MovementType movementType = MT.Acceleration;
		public bool isJumping = false;
		public bool isGrounded = false;
		public Transform groundCheckObject;
		public float groundCheckRadius;
		public LayerMask groundCheckLayerMask = 1;
		public LayerMask ignoreCollisionLayerMask = 0;
		public AnimatorLink[] animatorLinks;
		public TouchInfo movementTouch;
		public float touchMovementPoints = 50;

		void Start ()
		{
			this.tag = "Player";

			this.movementScript = this.GetComponent<MovementScript> ();
			this.animator = this.GetComponent<Animator> ();

			this.ChangeLayer (LayerMask.NameToLayer ("Player"));


			this.ignoreCollisionLayerMask.LayerAction ((layer) => {
				Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer ("Player"), layer, true);
			});
		}

		void FixedUpdate ()
		{
			if (this.groundCheckObject) {
				switch (this.groundCheckLayerMask.value) {
				case 1:
					this.isGrounded = Physics2D.OverlapCircle (this.groundCheckObject.position, this.groundCheckRadius);
					break;
				default:
					this.isGrounded = Physics2D.OverlapCircle (this.groundCheckObject.position, this.groundCheckRadius, this.groundCheckLayerMask);
					break;
				}
			} else {
				this.isGrounded = false;
			}

			if (this.movementScript) {
				switch (this.movementType) {
				case MT.Movement:
					this.movementScript.MoveHorizontal (moveValue: this.horizontalMovement);
					break;
				case MT.Acceleration:
					this.movementScript.AccelerateHorizontal (moveValue: this.horizontalMovement);
					break;
				default:
					break;
				}


				if (this.isJumping
					&& this.isGrounded) {
					this.isGrounded = false;

					switch (this.movementType) {
					case MT.Movement:
						this.movementScript.MoveVertical(1);
						break;
					case MT.Acceleration:
						this.movementScript.AccelerateVertical(1);
						break;
					default:
						break;
					}

				}
			}

			this.isJumping = false;

			this.changeFaceDirection ();
		}

		void Update ()
		{
			this.horizontalMovement = Input.GetAxis ("Horizontal");

			this.isJumping = Input.GetAxis ("Vertical") > 0;

			if (Input.GetButton ("Jump")) {
				this.isJumping = true;
			}

//
//			if (Input.touchCount > 0) {
//				foreach (var touch in Input.touches) {
//					var touchViewportPosition = Camera.main.ScreenToViewportPoint(touch.position);
//						switch (touch.phase) {
//						case TouchPhase.Began:
//							if (this.movementTouch.id == -1 
//						    && touchViewportPosition.x < 0.5) {
//								this.movementTouch = new TouchInfo();
//								this.movementTouch.id = touch.fingerId;
//								this.movementTouch.currentPosition = this.movementTouch.initialPosition = touch.position;
//							}
//						else if (touchViewportPosition.y < 0.5) {
//								this.isJumping = true;
//							}
//							break;
//						case TouchPhase.Stationary:
//						case TouchPhase.Moved:
//
////							if (this.movementTouch.id == -1) {
////								this.movementTouch = new TouchInfo();
////								this.movementTouch.id = touch.fingerId;
////								this.movementTouch.currentPosition = this.movementTouch.initialPosition = touch.position;
////							}
//
//							    if (this.movementTouch.id == touch.fingerId) {
//							this.horizontalMovement = Mathf.Clamp((this.movementTouch.currentPosition.x - this.movementTouch.initialPosition.x) / this.touchMovementPoints, -1, 1);
//
//								this.movementTouch.currentPosition = touch.position;
//							}
//							break;
//						case TouchPhase.Ended:
//						case TouchPhase.Canceled:
//							if (this.movementTouch.id == touch.fingerId) {
//								if (this.movementTouch.currentPosition == this.movementTouch.initialPosition
//								    && this.movementTouch.time < 1) {
//									this.isJumping = true;
//								}
//
//								this.movementTouch = new TouchInfo();
//							}
//							break;
//						default:
//							break;
//						}
////					}
//
//					this.movementTouch.time += Time.deltaTime;
//				}
//
//			}
//			else {
//
//				if (this.movementTouch.id != -1
//				    && this.movementTouch.currentPosition == this.movementTouch.initialPosition
//				    && this.movementTouch.time < 1) {
//					this.isJumping = true;
//				}
//				this.movementTouch = new TouchInfo();
//			}
//
////			#if UNITY_IPHONE || UNITY_ANDROID
////			#endif
//
			if (this.animatorLinks != null) {
				foreach (var link in this.animatorLinks) {
					link.PerformLink (this, this.animator);
				}
			}
		}

		void changeFaceDirection ()
		{
			if (this.GetComponent<Rigidbody2D> ().velocity.x != 0) {
				this.transform.localScale = new Vector3 (
					x: Mathf.Sign (this.GetComponent<Rigidbody2D> ().velocity.x) * Mathf.Abs (this.transform.localScale.x), 
					y: this.transform.localScale.y,
					z: this.transform.localScale.z);
			}
		}
	}
}