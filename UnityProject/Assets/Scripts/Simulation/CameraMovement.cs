/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
#endregion


/// <summary>
/// Component for simple camera target following.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    #region Members
    // Distance of Camera to target in Z direction, to be set in Unity Editor.
    [SerializeField]
    private int CamZ = -10;
    // The initial Camera target, to be set in Unity Editor.
    [SerializeField]
    private GameObject Target;
    // The speed of camera movement, to be set in Unity Editor.
    [SerializeField]
    private float CamSpeed = 5f;
    // The speed of camera movement when reacting to user input, to be set in Unity Editor.
    [SerializeField]
    private float UserInputSpeed = 50f;
    // Whether the camera can be controlled by user input, to be set in Unity Editor.
    [SerializeField]
    private bool AllowUserInput;

    /// <summary>
    /// The bounds the camera may move in.
    /// </summary>
    public RectTransform MovementBounds
    {
        get;
        set;
    }

    private Vector3 targetCamPos;
    #endregion

    #region Methods
    /// <summary>
    /// Sets the target to follow.
    /// </summary>
    /// <param name="target">The target to follow.</param>
    public void SetTarget(GameObject target)
    {
        //Set position instantly if previous target was null
        if (Target == null && !AllowUserInput && target != null)
            SetCamPosInstant(target.transform.position);

        this.Target = target;
    }

    // Unity method for updating the simulation
	void FixedUpdate ()
    {
        //Check movement direction
        if (AllowUserInput)
            CheckUserInput();
        else if (Target != null)
            targetCamPos = Target.transform.position;

        targetCamPos.z = CamZ; //Always set z to cam distance
        this.transform.position = Vector3.Lerp(this.transform.position, targetCamPos, CamSpeed * Time.deltaTime); //Move camera with interpolation

        //Check if out of bounds
        if (MovementBounds != null)
        {
            float vertExtent = Camera.main.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            float rightDiff = (this.transform.position.x + horzExtent) - (MovementBounds.position.x + MovementBounds.rect.width / 2);
            float leftDiff = (this.transform.position.x - horzExtent) - (MovementBounds.position.x - MovementBounds.rect.width / 2);
            float upDiff = (this.transform.position.y + vertExtent) - (MovementBounds.position.y + MovementBounds.rect.height / 2);
            float downDiff = (this.transform.position.y - vertExtent) - (MovementBounds.position.y - MovementBounds.rect.height / 2);

            if (rightDiff > 0)
            {
                this.transform.position = new Vector3(this.transform.position.x - rightDiff, this.transform.position.y, this.transform.position.z);
                targetCamPos.x = this.transform.position.x;
            }
            else if (leftDiff < 0)
            {
                this.transform.position = new Vector3(this.transform.position.x - leftDiff, this.transform.position.y, this.transform.position.z);
                targetCamPos.x = this.transform.position.x;
            }

            if (upDiff > 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - upDiff, this.transform.position.z);
                targetCamPos.y = this.transform.position.y;
            }
            else if (downDiff < 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - downDiff, this.transform.position.z);
                targetCamPos.y = this.transform.position.y;
            }
        }
    }

    /// <summary>
    /// Instantly sets the camera position to the given position, without interpolation.
    /// </summary>
    /// <param name="camPos">The position to set the camera to.</param>
    public void SetCamPosInstant(Vector3 camPos)
    {
        camPos.z = CamZ;
        this.transform.position = camPos;
        targetCamPos = this.transform.position;
    }

    private void CheckUserInput()
    {
        float horizontalInput, verticalInput;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        targetCamPos += new Vector3(horizontalInput * UserInputSpeed * Time.deltaTime, verticalInput * UserInputSpeed * Time.deltaTime);
    }
    #endregion
}
