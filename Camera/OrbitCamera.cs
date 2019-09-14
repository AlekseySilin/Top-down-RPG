using UnityEngine;
using System.Collections;

//Attach this script to main camera object and choose the target //to rotate around in the Inspector field
public class OrbitCamera : MonoBehaviour {
    [SerializeField]
    private Transform target;
    public float rotSpeed = 1.5f;
    private float _rotY;
    private Vector3 _offset;
	// Use this for initialization
	void Start () {
        _rotY = transform.eulerAngles.y;
        //storing the starting position offset between the camera and the target
        _offset = target.position - transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
                _rotY -= Input.GetAxis("Horizontal") * rotSpeed;
        Quaternion rotation = Quaternion.Euler(0, _rotY, 0);
        transform.position = target.position - (rotation * _offset);
        transform.LookAt(target);
    }
}
