using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Diagnostics;
using System.Threading;




public class CarController : MonoBehaviour
{
    private const string HORIZONTAL="Horizontal";
    private const string VERTICAL="Vertical";
    
    public Stopwatch stopwatch = new Stopwatch();
    public TextMeshProUGUI scoreUI;
   
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    bool finished=false;
    public int score=0;

    private GameObject atachedVehicle;

    private GameObject cameraPositionFolder;
    private Transform[] camLocations;
    
    [Range(0,20)]public float smothTime = 5;
    public int locationIndicator = 2;
    public Transform player;
    private Rigidbody playerRB;
    public Vector3 Offset;
    public float speed;
    // Start is called before the first frame update


    private void Start() {
        
        playerRB = player.GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        stopwatch.Start();
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        stopwatch.Stop();
        scoreUI.text= "Score:" + score.ToString() + "\n" + "Time:" + stopwatch.ElapsedMilliseconds.ToString();
   
        

    }

    private void cameraBehavior(){
        Vector3 velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position,camLocations[locationIndicator].transform.position,ref velocity,smothTime * Time.deltaTime);
        transform.LookAt(camLocations[1].transform);
    }
    private void Update() {
        
    }

    void LateUpdate()
    {
        Vector3 playerForward = (playerRB.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position,
            player.position + player.transform.TransformVector(Offset)
            + playerForward * (-5f),
            speed * Time.deltaTime);
        transform.LookAt(player);
    }

    private void GetInput()
    {
      horizontalInput=Input.GetAxis(HORIZONTAL)  ;
      verticalInput=Input.GetAxis(VERTICAL) ;
      isBreaking=Input.GetKey(KeyCode.Space) ;
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce=isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
      frontLeftWheelCollider.brakeTorque=currentbreakForce;
      frontRightWheelCollider.brakeTorque=currentbreakForce;
      rearLeftWheelCollider.brakeTorque=currentbreakForce;
      rearRightWheelCollider.brakeTorque=currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle= maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle =currentSteerAngle;
        frontLeftWheelCollider.steerAngle=currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider,frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider,frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider,rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider,rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider,Transform wheelTransform) 
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos,out rot);
        wheelTransform.rotation=rot;
        wheelTransform.position=pos;
    }
    private void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        finished=false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag =="coin")
        {   
            score++;
            Destroy(collision.gameObject);
            
        }

        if (collision.collider.tag =="Finish")
        {   
            
            Invoke("restart",0.5f);
            finished=true;
        }
    }

}
