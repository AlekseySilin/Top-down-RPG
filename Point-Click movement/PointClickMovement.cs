using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Attach this script to Player Character object
public class PointClickMovement : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    public float moveSpeed = 6.0f;
    public float rotSpeed = 15.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -20.0f;
    public float minFall = -1.5f;

    public float deceleration = 25.0f;
    public float targetBuffer = 1.5f;
    private float _curSpeed = 0f;
    private Vector3 _targetPos = Vector3.one;

    private float _vertSpeed;
    private ControllerColliderHit _contact;

    private CharacterController _charController;
    private Animator _animator;

    public float pushForce = 3.0f;

    // Use this for initialization
    void Start()
    {
        _vertSpeed = minFall;

        _charController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        // start with zero and add movement components progressively
        Vector3 movement = Vector3.zero;


        //whne mouse clicks and pointer isnt over UI elements
        if (Input.GetMouseButton(0)&&!EventSystem.current.IsPointerOverGameObject())
        {
            //raycast at the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(ray, out mouseHit))
            {
                GameObject hitObject = mouseHit.transform.gameObject;
                //only set moving direction clicking on objects with the layer "Ground"
                if (hitObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    _targetPos = mouseHit.point;
                    _curSpeed = moveSpeed;
                }
            }
        }
        //move if target position is set
        if (_targetPos != Vector3.one)
        {
            //only rotate toward target while moving quickly
            if (_curSpeed > moveSpeed * .5f)
            {
                Vector3 adjustedPos = new Vector3(_targetPos.x,
                transform.position.y, _targetPos.z);
                Quaternion targetRot = Quaternion.LookRotation(
                adjustedPos - transform.position);
                //smooth rotation
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    targetRot, rotSpeed * Time.deltaTime);
            }

            movement = _curSpeed * Vector3.forward;
            //transform the forward direction from player's local to global coordinates
            movement = transform.TransformDirection(movement);

            //if player is close to target position
            if (Vector3.Distance(_targetPos, transform.position) < targetBuffer)
            {
                _curSpeed -= deceleration * Time.deltaTime;
                if (_curSpeed <= 0)
                {
                    _targetPos = Vector3.one;
                }
            }
        }

        _animator.SetFloat("Speed", movement.sqrMagnitude);

        // raycast down to address steep slopes and dropoff edge
        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f;
            hitGround = hit.distance <= check;  // to be sure check slightly beyond bottom of capsule
        }

        // y movement: possibly jump impulse up, always accel down
        // could _charController.isGrounded instead, but then cannot workaround dropoff edge
        if (hitGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
                _animator.SetBool("Jumping", false);
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }
            if (_contact != null)
            {   // not right at level start
                _animator.SetBool("Jumping", true);
            }

            // workaround for standing on dropoff edge
            if (_charController.isGrounded)
            {
                if (Vector3.Dot(movement, _contact.normal) < 0)
                {
                    movement = _contact.normal * moveSpeed;
                }
                else
                {
                    movement += _contact.normal * moveSpeed;
                }
            }
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        _charController.Move(movement);
    }

    // store collision to use in Update
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
        //check if the collided object has a rigidbody to receive physucs forces
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null && !body.isKinematic)
        {
            body.velocity = hit.moveDirection * pushForce;
        }

    }
}
