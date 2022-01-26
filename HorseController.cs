using System.Collections;
using UnityEngine;

// HorseController requires the GameObject to have a Rigidbody and Animator component
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class HorseController : MonoBehaviour
{
    #region DataFields
    
    [Header("References")]
    public Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator riderAnimator;

    [Header("Variables")]
    [SerializeField] private float runSpeed = 500f;
    [SerializeField] private float sprintSpeed = 10f;
    [Tooltip("Sprint Animation Clip Speed")]
    [SerializeField] private float sprintAnimClipSpeed = 0.7f;
    private bool startRace = false;
    private bool sprint = false;
    [Tooltip("Tilt/Device Acceleration")]
    [SerializeField] private bool isDevAccel;
    [Tooltip("Rotation Lerp Speed")]
    [SerializeField] private float rotatLerpSpeed = 0.5f;
    [Tooltip("Rotation Angel Value")]
    [SerializeField] private float rotatAngelValue = 5f;
    private bool isRotationLerp = false;
    private Vector3 rotatAngel;
    private float rotatVal;


    float dirX;
    float moveSpeed = 30f;
    bool rightTilt = true;
    bool leftTilt = true;
    #endregion

    #region Methods

    private void Start()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (isDevAccel)
        {

            dirX = Input.acceleration.x * moveSpeed;
            Debug.Log("device accel = " + dirX);

            
            if (dirX > 2 && rightTilt)
            {
                StartCoroutine(RightTilt());
            }
            if (dirX < -2 && leftTilt)
            {
                StartCoroutine(LeftTilt());
            }
        }

        if (startRace && isRotationLerp)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotatAngel), Time.deltaTime * rotatLerpSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (startRace && !sprint)                     // start apply force on horse if startRace bool is true
        {
            rb.AddForce(transform.forward * runSpeed, ForceMode.Impulse);
        }
        else if(startRace && sprint)                // increase speed of horse if sprint
        {
            rb.AddForce(transform.forward * sprintSpeed, ForceMode.Impulse);
        }
    }


    // start race method
    public void StartRace()
    {
        if (rb)
        {
            rb.isKinematic = false;
        }
        else
        {
            return;
        }
        if (animator)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Sprint", true);
        }
        startRace = true;
        if (riderAnimator)
        {
            riderAnimator.SetBool("RGallop", true);
        }
    }

    // sprintdown method for increase speed
    public void SprintDown()
    {
        sprint = true;
        if (animator)
        {
            animator.SetFloat("SprintSpeed", 1f);
        }       
    }

    // sprintup method for decrease speed
    public void SprintUp()
    {
        sprint = false;
        if (animator)
        {
            animator.SetFloat("SprintSpeed", sprintAnimClipSpeed);
        }    
    }

    public void Jump()
    {
        if (animator)
        {
            animator.SetBool("SprintJump", true);
        }
        StartCoroutine(JumpAnimationOff());        
    }

    public void RightTurn()
    {
        isRotationLerp = true;
        rotatVal += rotatAngelValue;
        rotatAngel = new Vector3(0f, rotatVal, 0f);
    }

    public void LeftTurn()
    {
        isRotationLerp = true;
        rotatVal -= rotatAngelValue;
        rotatAngel = new Vector3(0f, rotatVal, 0f);
    }

    #endregion

    #region Coroutines

    IEnumerator JumpAnimationOff()
    {
        yield return new WaitForSeconds(0.1f);
        if (animator)
        {
            animator.SetBool("SprintJump", false);
        }
    }

    IEnumerator RightTilt()
    {
        rightTilt = false;
        RightTurn();
        yield return new WaitForSeconds(0.5f);
        rightTilt = true;
    }

    IEnumerator LeftTilt()
    {
        leftTilt = false;
        LeftTurn();
        yield return new WaitForSeconds(0.5f);
        leftTilt = true;
    }

    #endregion

}
